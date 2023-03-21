using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class Goat : GenericAnimal 
{   
    public float moveSpeed = goat.RunSpeed;
    public bool isWalking;
    public float walkTime;
    private float walkCounter;
    public float waitTime;
    private float waitCounter;

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
                        goat.velocity = new Vector2(0, moveSpeed);
                        break;
                    case 1:
                        goat.velocity = new Vector2(moveSpeed, 0);
                        break;
                    case 2:
                        goat.velocity = new Vector2(0, -moveSpeed);
                        break;
                    case 3:
                        goat.velocity = new Vector2(-moveSpeed, 0);
                        break;
                }
            }
            else
            {
                waitCounter -= Time.deltaTime;
                goat.velocity = Vector2.zero;
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

      void Start()
    {
        SetAnimalData("Goat", 200, 0.9f, 5f, 1.8f, 0.05f);
        
    }

    void ChooseDirection()
    {
        walkDirection = Random.Range(0, 3);
        isWalking = true;
        walkCounter = walkTime;
    }
    void Update()
    {
        move();

    }



}