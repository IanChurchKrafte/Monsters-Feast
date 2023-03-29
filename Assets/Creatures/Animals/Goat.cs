using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class Goat : GenericAnimal 
{   
    
    private Rigidbody2D goat;

    internal Transform thisTransform;
    public float moveSpeed = 0.5f;
    public Vector2 decisionTime = new Vector2(1, 4);
    internal float decisionTimeCount = 0;
    internal Vector3[] moveDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.zero, Vector3.zero };
    internal int currentMoveDirection;
 
    void Start()
    {
        goat = GetComponent<Rigidbody2D>();
        SetAnimalData("Goat", 100, 0.9f, moveSpeed, 1.8f, 0.05f);
        thisTransform = this.transform;
        decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
        ChooseMoveDirection();

    }
 
    
    void Update()
    {
       
        thisTransform.position += moveDirections[currentMoveDirection] * Time.deltaTime * moveSpeed;
        if (decisionTimeCount > 0) decisionTimeCount -= Time.deltaTime;
        else
        {
            decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
            ChooseMoveDirection();
        }
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

}