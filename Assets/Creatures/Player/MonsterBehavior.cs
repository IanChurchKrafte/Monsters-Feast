using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{
    public int health;                           // monster health
    public float stamina;                        // monster stamina

    public float speed = 0.025f;                 // base speed
    public float real_speed;
    public float rotationSpeed = 720;
    public float acceleration = 3.5f;            // sprint factor

    public bool attacking = false;
    public float timeToAttack = 2;
    public float attackTimer = 0;
    public float attackDMG = 20;
    public float attackDMG_scale;

    public bool stealth_active = false;
    public float timeToPounce = 2.5f;                // time limit for pounce
    public float pounceTimer;
    public bool pounce_active = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        real_speed = speed;
        // monster movement - WASD or arrow keys
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 velocity = new Vector2 (h,v);
        velocity.Normalize();
        Vector3 movement = new Vector3(velocity.x, velocity.y, 0) * real_speed;

        // rotation handled here
        if (velocity != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90;
            // ISSUE: rotation is only being handled on x and y axes
            Quaternion toRotate = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
        else
        {
            real_speed = 0;
        }

        // MONSTER SPRINT - Left Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            real_speed *= acceleration;
        }

         // MONSTER ATTACK - LMB
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            attacking = true;
        }

        if (attacking)
        {
            Debug.Log("damage: " + attackDMG);
            attacking = false;
        }

        // STEALTH - Left Control

        // IF USER HOLDS DOWN LEFT CTRL, STEALTH IS ACTIVE
        if (Input.GetKeyDown(KeyCode.LeftControl) && !stealth_active)
        {
            stealth_active = true;
        }

        // once stealth is active, decrease speed
        if (Input.GetKey(KeyCode.LeftControl) && stealth_active)
        {
            //Debug.Log("stealth activated");
            real_speed /= 4.0f;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                pounceTimer = Time.time;
                //Debug.Log("pounce timer activated");
                pounce_active = true;
            }

            if (pounce_active)
            {
                real_speed = 0;
                attackDMG_scale = 0;
                // IF THE KEY HAS BEEN HELD AND 2.5 SECONDS HAVE PASSED
                if (Input.GetKey(KeyCode.Mouse0) && pounce_active && Time.time - pounceTimer > 2.5f)
                {
                    // CANCEL OUT BOTH STEALTH AND POUNCE STATE
                    pounce_active = false;
                    stealth_active = false;
                    //Debug.Log("released");
                    
                    // SET MAX DAMAGE TO 175
                    attackDMG_scale = 175;
                    //Debug.Log("attack damage scale " + attackDMG_scale);
                }
                else
                {
                    // DETECT TIME FOR HOW LONG POUNCE IS HELD
                    // THE LONGER THE POUNCE, THE MORE DAMAGING AN ATTACK IS
                    attackDMG_scale = attackDMG + (Time.time - pounceTimer) * 62;
                    //Debug.Log("attackDMG_scale " + attackDMG_scale);

                    // IF POUUNCE IS RELEASED BEFORE THE TIME LIMIT

                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        Debug.Log("release");
                        // RESET POUNCE TIMER
                        pounceTimer = 0;
                        pounce_active = false;
                    }
                }
            }
            else
            {
                pounce_active = false;
            }
            //Debug.Log("DMG after pounce: " + attackDMG);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            stealth_active = false;
        }

        float currentAngle = transform.rotation.eulerAngles.z - 90;
        Vector3 trueMovement = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0) * real_speed;
        gameObject.transform.position += trueMovement;
    }
}