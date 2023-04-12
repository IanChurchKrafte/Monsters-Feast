using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{
    // * Health and Stamina * //
    public int maxHealth = 100; 
    public int currentHealth;
    public float stamina;
    
    // * Movement Parameters * //
    public float speed = 0.025f;
    public float real_speed;
    public float rotationSpeed = 720;
    public float acceleration = 1.5f;

    // * Attacking * //
    public GameObject attackArea = default;
    public static GameObject eatBox = default;
    public static int attackDMG = 20;
    private float timeToAttack = 0.25f;
    private float attackTimer = 0f;

    // * Stealth * //
    public static bool stealth_active = false;
    
    // * Pounce * //
    public static bool pounce_active = false;
    public float maxTimeToPounce = 2.5f;
    public float pounceTimer;               // time limit for pounce
    
    // * Lunge * //
    public float lungeTimer;

    public bool collided;
    public Vector3 goalPosition;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackArea = transform.GetChild(0).gameObject;
        eatBox = transform.GetChild(1).gameObject;
        currentHealth = maxHealth;
        attackArea.SetActive(false);
        eatBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        real_speed = speed;
        // monster movement - WASD or arrow keys
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 velocity = new Vector2 (h,v);
        if (h != 0 && v != 0) velocity.Normalize();
        Vector3 movement = new Vector3(velocity.x, velocity.y, 0) * real_speed;

        // rotation handled here
        if (velocity != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90;
            Quaternion toRotate = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
        else
        {
            real_speed = 0;
        }

        // * Monster sprints  * //
        if (Input.GetKey(KeyCode.LeftShift))
        {
            real_speed *= acceleration;
        }

        // * Bite - only works if stealth is NOT active * //
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown("joystick button 0")) && !stealth_active)
        {
            Bite();
        }

        // * Bite functionality * //
        if (AttackHitbox.attacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= timeToAttack)
            {
                AttackHitbox.attacking = false;
                attackArea.SetActive(AttackHitbox.attacking);
                attackTimer = 0;
            }
        }

        // * Lunging functionality * //
        if (AttackHitbox.lunging)
        {
            lungeTimer -= Time.deltaTime;
            if (lungeTimer <= 0)
            {
                Debug.Log("lunge is over");
                // Debug.Log("lunge lasted for " + lungeTimer + " seconds");
                AttackHitbox.lunging = false;
                attackArea.SetActive(AttackHitbox.lunging);
                lungeTimer = 0;
            }

        }

        // * Monster activates stealth * //
        if (((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown("joystick button 6")) || Input.GetAxis("Stalk") > 0) && !stealth_active)
        {
            stealth_active = true;
        }

        // * Decrease Monster's speed if stealth is active * //
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetAxis("Stalk") > 0) && stealth_active)
        {
            real_speed /= 4.0f;
            // * Pounce is only active under stealth * //
            if ((Input.GetMouseButtonDown(0) || Input.GetAxis("Attack") > 0))
            {
                pounce_active = true;
            }
            if (pounce_active)
            {
                Pounce();
            }
            else
            {
                pounce_active = false;
            }
        }
        float currentAngle = transform.rotation.eulerAngles.z - 90;

        // directional influence
        Vector2 trueMovement = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * real_speed;       // trig
        rb.MovePosition(rb.position + trueMovement);
    }

    // * Standard Bite Attack * //
    private void Bite()
    {
        AttackHitbox.attacking = true;
        attackArea.SetActive(AttackHitbox.attacking);
    }

    // * Monster enters Pounce state * //
    private void Pounce()
    {
        real_speed = 0;
        pounceTimer += Time.deltaTime;
        // * If pounce is held AND timer exceeds limit * //
        if (Input.GetMouseButton(0) && pounceTimer >= maxTimeToPounce)
        {   
            AttackHitbox.dmg_scale = 175;           // max out Monster's damage
            lungeTimer = maxTimeToPounce;           // set lunge timer to max time
            pounceTimer = 0;                        // reset pounce timer

            pounce_active = false;
            stealth_active = false;

            Lunge();
        }
        else
        {
            AttackHitbox.dmg_scale = (int)(attackDMG + (pounceTimer) * 62);
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("release");
                lungeTimer = pounceTimer;           // lunge timer is set depending on how long Monster stays in pounce
                pounceTimer = 0;                    // reset pounce timer
                pounce_active = false;
                stealth_active = false;
                Lunge();
            }
        }
    }

    // * Lunge Attack (only from Pounce) * //
    private void Lunge()
    {
        AttackHitbox.lunging = true;
        attackArea.SetActive(AttackHitbox.lunging);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collided = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collided = false;
    }

    // What if monster takes damage?
    public void TakeDamage(int damage)
    {
        int temp = currentHealth - damage;
        if(temp <= 0){
            //dead
            currentHealth = 0;
        }
        else
            currentHealth = temp;
    }
}