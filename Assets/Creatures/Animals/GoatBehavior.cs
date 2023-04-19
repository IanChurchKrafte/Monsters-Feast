using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class GoatBehavior : GenericAnimal 
{   
    
    private Rigidbody2D goat;
    public float awareDistance = 5.0f, perceptionDistance = 50.0f;
    internal Transform thisTransform;
    public float moveSpeed = 1.25f;
    public float fleeSpeed = 1.15f;
    public float rotationSpeed;
    public Vector2 decisionTime = new Vector2(1, 4);
    internal float decisionTimeCount = 0;
    internal Vector3[] moveDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.zero, Vector3.zero };
    internal int currentMoveDirection;
    Vector2 moveDirection; 
    private Transform player;
 
    void Start()
    {
        goat = GetComponent<Rigidbody2D>();

         /*
        Animal type 
        Health 
        awareness level
        run speed 
        flee speed multiplyer
        calorie %  
        */


        SetAnimalData("Goat", 100, 0.9f, moveSpeed, fleeSpeed, 0.05f);
        thisTransform = this.transform;
        decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
        ChooseMoveDirection();
        player = GameObject.FindWithTag("Player").transform;

    }
    void ChooseMoveDirection()
    {
        currentMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));
    }

    
<<<<<<< Updated upstream
    // apart of the fleeing function 
=======
    private void OnCollisionEnter2D(Collision2D collision)
     {
         if (collision.gameObject.CompareTag("Player"))
         {

             goat.constraints = RigidbodyConstraints2D.FreezeRotation;
             goat.constraints = RigidbodyConstraints2D.None;
         }
     }


>>>>>>> Stashed changes
    private void FixedUpdate()
    {
        if(player)
        {
            goat.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed * fleeSpeedMultiplyer; 
            
        }   
    }

    void Update()
    {   
       
        if (!isDead)
        {
<<<<<<< Updated upstream
=======
        // move    
>>>>>>> Stashed changes
            thisTransform.position += moveDirections[currentMoveDirection] * Time.deltaTime * moveSpeed;
            if (decisionTimeCount > 0) decisionTimeCount -= Time.deltaTime;
            else
            {
                decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
                ChooseMoveDirection();
<<<<<<< Updated upstream
            } 
=======
            }
>>>>>>> Stashed changes
        
        //flee function, currently moves faster than it does now 
        if (player)
        {
            Vector3 direction = (transform.position - player.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            goat.rotation = angle;
            moveDirection = direction; 
            Debug.Log("Animal is fleeing");
        }
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
        }
        else
        {

            moveSpeed = 0;
            isDead = true;
        
        }
    }
   

    }
    
    