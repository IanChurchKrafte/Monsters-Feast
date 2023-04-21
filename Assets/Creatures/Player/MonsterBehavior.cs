using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;

public class MonsterBehavior : MonoBehaviour
{
    // * Health and Stamina * //
    public int maxHealth = 100; 
    public int currentHealth;
    public float stamina;
    public float sustenance = 0;
    
    // * Movement Parameters * //
    public float speed = 0.025f;
    public static float real_speed;
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
    
    // * Stalk * //
    public static bool pounce_active = false;
    public float maxTimeToPounce = 2.5f;
    public float stalkTimer;               // time limit for pounce
    
    // * Lunge * //
    public float lungeTimer;
    public bool canStalk = true;
    bool bitePressed = false;

    public bool collided;
    public Vector3 goalPosition;
    Rigidbody2D rb;
    public GameObject readyPanel, deadPanel;
    bool startHostility;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackArea = transform.GetChild(0).gameObject;
        eatBox = transform.GetChild(1).gameObject;
        currentHealth = maxHealth;
        attackArea.SetActive(false);
        //eatBox.SetActive(false);
        startHostility = Cow.cowInjured;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0)
        {
            deadPanel.SetActive(true);
            eatBox.SetActive(false);
            GetComponent<Animator>().enabled = false;
            rb.velocity = Vector2.zero;
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                Cow.cowInjured = startHostility;
            }
            return;
        }
        if(sustenance >= 0.75f)
        {
            readyPanel.SetActive(true);
        }
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
            // only rotate if the monster is not in eating mode
            if (!(EatBox.canEat && Input.GetKey(KeyCode.JoystickButton2)))
            {
                float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90;
                Quaternion toRotate = Quaternion.AngleAxis(targetAngle, Vector3.forward);
                gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
                GetComponent<Animator>().SetBool("Walking", true);
            }
            else
            {
                rb.velocity = Vector2.zero;
                GetComponent<Animator>().SetBool("Walking", false);
            }
        }
        else
        {
            real_speed = 0;
            GetComponent<Animator>().SetBool("Walking", false);
        }

        // * Monster sprints  * //
        if (Input.GetKey(KeyCode.LeftShift))
        {
            real_speed *= acceleration;
        }

        if (bitePressed && attackTimer >= timeToAttack && lungeTimer == 0)
        {
            AttackHitbox.attacking = false;
            attackArea.SetActive(AttackHitbox.attacking);
        }
        // * Bite - only works if stealth is NOT active * //
        if (!bitePressed && (Input.GetMouseButtonDown(0) || Input.GetAxis("Attack") > 0) && !stealth_active && lungeTimer == 0)
        {
            Bite();
        }
        else if(!(Input.GetMouseButton(0) || Input.GetAxis("Attack") > 0))
        {
            bitePressed = false;
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
        if (AttackHitbox.lunging && (Input.GetMouseButton(0) || Input.GetAxis("Attack") > 0))
        {
            stalkTimer = 0;
            lungeTimer -= Time.deltaTime;
            if (lungeTimer <= 0 || collided)
            {
                Debug.Log("lunge is over");
                // Debug.Log("lunge lasted for " + lungeTimer + " seconds");
                AttackHitbox.lunging = false;
                attackArea.SetActive(AttackHitbox.lunging);
                lungeTimer = 0;
            }
            real_speed *= acceleration * 1.25f;
        }
        else if(!(Input.GetMouseButton(0) || Input.GetAxis("Attack") > 0))
        {
            //Debug.Log("lunge is over");
            // Debug.Log("lunge lasted for " + lungeTimer + " seconds");
            AttackHitbox.lunging = false;
            attackArea.SetActive(AttackHitbox.lunging);
            lungeTimer = 0;
        }

        // * Monster activates stealth * //
        if (((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown("joystick button 6")) || Input.GetAxis("Stalk") > 0))
        {
            stealth_active = true;
        }
        else
        {
            stealth_active = false;
        }

        // * Decrease Monster's speed if stealth is active * //
        if (!AttackHitbox.lunging && (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(1) || Input.GetAxis("Stalk") > 0) && stealth_active)
        {
            if (!(Input.GetMouseButton(0) || Input.GetAxis("Attack") > 0))
                real_speed /= 4.0f;
            // * Stalk is only active under stealth * //
            //if ((Input.GetMouseButtonDown(0) || Input.GetAxis("Attack") > 0)) pounce_active = true;


            if (canStalk)
                Stalk();

            //else
            //{
            //    pounce_active = false;
            //}
        }
        else if (!AttackHitbox.lunging)
        {
            canStalk = true;
        }
        float currentAngle = transform.rotation.eulerAngles.z - 90;

        // directional influence
        Vector2 trueMovement = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * real_speed;       // trig
        GetComponent<Animator>().SetFloat("WalkSpeed", real_speed);

        // Stop moving and rotating when monster enters the "eating" process
        if (EatBox.canEat == true && Input.GetKey(KeyCode.JoystickButton2)) rb.velocity = Vector2.zero;
        else rb.MovePosition(rb.position + trueMovement);
    }

    // * Standard Bite Attack * //
    private void Bite()
    {
        bitePressed = true;
        AttackHitbox.attacking = true;
        attackArea.SetActive(AttackHitbox.attacking);
    }

    // * Monster enters Stalk state * //
    private void Stalk()
    {
        //real_speed = 0;
        // * If pounce is held AND timer exceeds limit * //
        Debug.Log("Stalk: " + (Input.GetAxis("Stalk") > 0) + ", Attack: " + (Input.GetAxis("Attack") > 0));
        if(stalkTimer > 0 && (Input.GetMouseButton(1) || Input.GetAxis("Stalk") > 0) && (Input.GetMouseButton(0) || Input.GetAxis("Attack") > 0))
        {
            Debug.Log("Starting lunge");

            lungeTimer = stalkTimer;           
            stalkTimer = 0;                        // reset pounce timer

            pounce_active = false;
            stealth_active = false;
            canStalk = false;
            Lunge();
        }
        else if ((Input.GetMouseButton(1) || Input.GetAxis("Stalk") > 0) && stalkTimer >= maxTimeToPounce)
        {    
            AttackHitbox.dmg_scale = 175;           // max out Monster's damage
            lungeTimer = maxTimeToPounce;           // set lunge timer to max time
            //stalkTimer = 0;                        // reset pounce timer

            pounce_active = false;
            stealth_active = false;

            //Lunge();
        }
        else
        {
            stalkTimer += Time.deltaTime; 
            AttackHitbox.dmg_scale = (int)(attackDMG + (stalkTimer) * 62);
            if (!(Input.GetMouseButton(1) || Input.GetAxis("Stalk") > 0))
            {
                Debug.Log("release");
                //lungeTimer = stalkTimer;           // lunge timer is set depending on how long Monster stays in pounce
                stalkTimer = 0;                    // reset pounce timer
                pounce_active = false;
                stealth_active = false;
                //Lunge();
            }
        }
    }

    // * Lunge Attack (only from Stalk) * //
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
        Debug.LogWarning("Player took " + damage + " damage.");
        int temp = currentHealth - damage;
        if(temp <= 0){
            //dead
            currentHealth = 0;
        }
        else
            currentHealth = temp;
    }
}