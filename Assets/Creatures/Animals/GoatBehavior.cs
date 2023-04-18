using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class GoatBehavior : GenericAnimal 
{   
    
    private Rigidbody2D goat;
    public float awareDistance = 5.0f, perceptionDistance = 50.0f;
    internal Transform thisTransform;
    public float moveSpeed = 0.75f;
    public float rotationSpeed;
    public Vector2 decisionTime = new Vector2(1, 4);
    internal float decisionTimeCount = 0;
    internal Vector3[] moveDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.zero, Vector3.zero };
    internal int currentMoveDirection;
    private GameObject player;
 
    void Start()
    {
        goat = GetComponent<Rigidbody2D>();
        SetAnimalData("Goat", 100, 0.9f, moveSpeed, 1.8f, 0.05f);
        thisTransform = this.transform;
        decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
        ChooseMoveDirection();
        player = GameObject.Find("Monster");

    }
    void ChooseMoveDirection()
    {
        currentMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));
    }

    void MonsterCheck(float adist, float pdist) 
    {
        AwarenessCheck(adist);
        PerceptionCheck(pdist);
    }
    

    void Update()
    {   
       
        if (!isDead){
        thisTransform.position += moveDirections[currentMoveDirection] * Time.deltaTime * moveSpeed;
        if (decisionTimeCount > 0) decisionTimeCount -= Time.deltaTime;
        else
        {
            decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
            ChooseMoveDirection();
        }
        
        MonsterCheck(awareDistance, perceptionDistance);
        }
        else{

            moveSpeed = 0;
            isDead = true;
        
        }
    }
   

    }
    
    