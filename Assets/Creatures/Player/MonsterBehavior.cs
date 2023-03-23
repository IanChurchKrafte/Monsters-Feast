using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{
    public int health;                           // monster health
    public float stamina;                        // monster stamina
    public float speed = 0.025f;                 // base speed
    public float rotationSpeed = 720;
    public float acceleration = 3.5f;            // sprint factor
    public bool stealth_active = false;
    public float pounce = 0.75f;                // time limit for pounce
    public bool pounce_active = false;
    bool collided;
    // public float pounce_offset = 5.0f;
    public Vector3 goalPosition;
    Rigidbody2D rb;
    
    // POUNCE TIMER: 6 second limit

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        float real_speed = speed;
        // monster movement - WASD or arrow keys
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 velocity = new Vector2 (h,v);
        if(h != 0 && v != 0)
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
        
        // if (movement != Vector3.zero)
        // {
        //     gameObject.transform.rotation = Quaternion.AngleAxis(30, Vector3.forward);
        // }

        // SPRINT - Left Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            real_speed *= acceleration;
        }

        // STEALTH - Left Control       
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetAxis("Stalk") > 0) && !stealth_active)
        {
            stealth_active = true;
        }
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetAxis("Stalk") > 0) && stealth_active)
        {
            Debug.Log("stealth activated");
            real_speed /= 4.0f;
            // POUNCE - left mouse button (LMB)
            if ((Input.GetMouseButtonDown(0) || Input.GetAxis("Attack") > 0) && !pounce_active)
            {
                pounce_active = true;
            }

            if ((Input.GetMouseButtonDown(0) || Input.GetAxis("Attack") > 0) && pounce_active)
            {
                Debug.Log("pounce timer activated");
                if (pounce > 0)
                {
                    pounce -= Time.deltaTime;
                    real_speed = 0;       // remain stationary when in pounce state
                }
                else
                {
                    Debug.Log("Released");
                    pounce_active = false;
                    stealth_active = false;
                    
                    // Make the "POUNCE" state; cancels stealth key held down by player
                }
            }
            else
            {
                pounce = 0.75f;
            }
        }
        float currentAngle = transform.rotation.eulerAngles.z - 90;

        // directional influence
        Vector2 trueMovement = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * real_speed;       // trig
        rb.MovePosition(rb.position + trueMovement);
    }
    
    void Pounce_Timer(float timeStart)
    {
        if ((pounce) > 0)
        {
            Debug.Log(" ButtonUp ");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collided = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collided = false;
    }
}