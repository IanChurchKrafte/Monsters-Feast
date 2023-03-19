using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class Goat : GenericAnimal 
{   
    private Vector2 direction;
    void move()
    {
        //it moves!!!
        Rigidbody2D goat = GetComponent<Rigidbody2D>();
        if (!isDead) {
            if (goat == null){
                Debug.LogError("Goat is not attached to any Rigidbody2D");
                return;
            }

        int walkT = Random.Range(3, 7);
        direction = Random.insideUnitCircle.normalized;
        float counter = 0f;
        while (counter <= walkT)
        {
            counter+=Time.deltaTime;
            goat.AddForce(direction * runSpeed, ForceMode2D.Impulse);
            ChangeDirection();
        }
        counter = 0f;
        int idleTime = Random.Range(5,7);
        while (counter <= idleTime)
        {
            counter+= Time.deltaTime;
        }
        }
        else
        {
            goat.velocity = Vector2.zero;
        }
        
    }

    void ChangeDirection()
    {
        int directions = Random.Range(0,4);
        switch(directions)
        {
            case 0:
                direction = Vector2.right;
                break;
            case 1:
                direction = Vector2.up;
                break;
            case 2:
                direction = Vector2.left;
                break;
            case 3:
                direction = Vector2.down;
                break;
            default:
                break;
        }
    }

    void Start()
    {

        /*
        Stats: 

        Animal type = Goat
        Health =
        Awareness level =
        Run Speed = 
        Flee Speed Multiplier = 
        Calorie % = 
        */

        //animal will now exist
        
        SetAnimalData("Goat", 200, .8f, 0.00005f, 1.8f,.08f);

    }

    void Update()
    {
        move();
    }
}