using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

 
public class GoatBehavior : GenericAnimal
{

    public float awareDistance = 5.0f, perceptionDistance = 30.0f;
    public float walkSpeed = 1.5f, sprintSpeed = 3.0f;
    public float fleeDistance = 10.0f;
    


    Rigidbody2D goat;
    public bool isFleeing = false;
    private GameObject player;
    IEnumerator move;
    private bool isMoving = false;

    
    IEnumerator Flee(){
        

        Vector2 direction = transform.position - player.transform.position;
        float elapsed = 0f;

        float distanceToPlayer = (player.transform.position - transform.position).magnitude;

        goat.velocity = Vector2.zero;

        while(elapsed < 5.0f && isFleeing){
            
            StopCoroutine(move);

            direction = player.transform.position - transform.position;

            distanceToPlayer = (player.transform.position - transform.position).magnitude;

            if(distanceToPlayer < 5.0f){
                RotateTowardsDirection(direction);
                direction *= -1.0f;
                goat.velocity = direction.normalized * walkSpeed * fleeSpeedMultiplyer;
            }
            else{
                RotateTowardsDirection(direction);
                direction *= -1.0f;
                goat.velocity = direction.normalized * walkSpeed * fleeSpeedMultiplyer / 2.0f;
                
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        goat.velocity = Vector2.zero;
        isFleeing = false;
    }

    public void FleeCheck(){ 
        float distanceToPlayer = (player.transform.position - transform.position).magnitude;
        
        if(distanceToPlayer < fleeDistance){
            
            StopCoroutine(move);
            goat.velocity = Vector2.zero;

            StartCoroutine(Flee());
            
            isFleeing = true;
        }
        if(distanceToPlayer >= fleeDistance && isFleeing){
            StopCoroutine(Flee());
            goat.velocity = Vector2.zero;
            isFleeing = false;
        }
    }

    void RotateTowardsDirection(Vector2 direction){
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    IEnumerator MoveAround(){
        Collider2D levelCollider = GameObject.Find("Level 1 Bounds").GetComponent<Collider2D>();
        Bounds levelBounds = levelCollider.bounds;
        goat.velocity = Vector2.zero;
        while(true && !isDead){
       
            isMoving = true;
     
            Vector2 direction = Random.insideUnitCircle.normalized;

            RotateTowardsDirection(direction);

            float moveTime = Random.Range(2f, 5f);
            float elapsed = 0f;
            
            while(elapsed < moveTime){
                RotateTowardsDirection(-direction);
      
                goat.velocity = (direction) * walkSpeed * 0.65f;
                
    
                elapsed += Time.deltaTime;

                yield return null;
            }
            goat.velocity = Vector2.zero;
 
            float waitTime = Random.Range(3f, 5f);
            elapsed = 0f;
            while(elapsed < waitTime){
                elapsed += Time.deltaTime;

                yield return null;
            }
            yield return null;


        }
        if(isDead) goat.velocity = Vector2.zero;
    }

    void MonsterCheck(float Adistance, float Pdistance){ 
        if(AwarenessCheck(Adistance) || PerceptionCheck(Pdistance)){
       
            FleeCheck();
        }
    }



    void Start()
    {

        goat = GetComponent<Rigidbody2D>();
        SetAnimalData("Goat", 100, 0.8f, 5.0f, 3.0f, 0.18f); 
        move = MoveAround();
        StartCoroutine(move);
        player = GameObject.Find("Monster"); 
    }

    
    
    
 
    void Update()
    {
        if(!isDead){ 
            if(isFleeing){
                StopCoroutine(move);
                isMoving = false;
                FleeCheck();
            }
            else{
                if(!isMoving){
                 
                    isMoving = true;
                    StartCoroutine(move);
                } 
            
                MonsterCheck(awareDistance, perceptionDistance);

            }
        }
        else{
            StopAllCoroutines();
            isMoving = false;
        }
    }
}