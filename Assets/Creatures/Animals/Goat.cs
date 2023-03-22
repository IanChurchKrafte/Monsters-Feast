using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class Goat : GenericAnimal 
{   
    public float moveSpeed = 0.5f; 
    public bool isWalking;
    public float walkTime;
    private float walkCounter;
    public float waitTime;
    private float waitCounter;
    public float rotationSpeed = 720;
    private int walkDirection;
   



    void move()
    {
        Rigidbody2D goat = GetComponent<Rigidbody2D>();
        
        if(!isDead){
            if (isWalking)
            {
                ChooseDirection(); 
                walkCounter -= Time.deltaTime;
                if (walkCounter < 0)
                {
                    isWalking = false;
                    waitCounter = waitTime; 
                }
                switch(walkDirection)
                {
                    case 0:
                        goat.velocity = new Vector2(-moveSpeed, moveSpeed);
                        break;
                    case 1:
                        goat.velocity = new Vector2(moveSpeed, -moveSpeed);
                        break;
                    case 2:
                        goat.velocity = new Vector2(-moveSpeed, -moveSpeed);
                        break;
                    case 3:
                        goat.velocity = new Vector2(moveSpeed, moveSpeed);
                        break;
                }

            }
            else
            {
                waitCounter -= Time.deltaTime;
                if(waitCounter < 0)
                {
                    ChooseDirection();
                }
            } 
        }
        else 
        { 
            goat.velocity = Vector2.zero;
        }
    }



    void MonsterCheck(float adist, float pdist) 
    {
        AwarenessCheck(adist);
        PerceptionCheck(pdist);
    }

      void Start()
    {
        SetAnimalData("Goat", 100, 0.9f, moveSpeed, 1.8f, 0.05f);
         
        
    }

    void ChooseDirection()
    {
        walkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }
    void Update()
    {
        move();
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 velocity = new Vector2 (h,v);
        velocity.Normalize();
        Vector3 movement = new Vector3(velocity.x, velocity.y, 0) * moveSpeed;

        // rotation handled here
        if (velocity != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90;
            Quaternion toRotate = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
        
        //float adist, pdist, dist;
        //MonsterCheck();

    }



}