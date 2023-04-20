using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class Cow : GenericAnimal
{
    public float awareDistance = 5.0f, perceptionDistance = 50.0f;
    public float corpseDistance = 30.0f;
    public float walkSpeed = 1.5f, sprintSpeed = 3.0f;
    public float fleeDistance = 10.0f;

    public bool collided;
    public static bool isHit;
    public static bool cowInjured;

    Rigidbody2D cow;
    private Animator anim;
    
    public bool isFleeing = false;
    private GameObject player;
    IEnumerator move;
    private bool isWandering = false;

    // void CorpseCheck(float distance){ //checks for any nearby corpses
    //     Vector2 position = transform.position;
    //     Vector2 rotation = transform.rotation.eulerAngles;
    //     RaycastHit2D hit = Physics2D.CircleCast(position, distance, rotation);
    //     if(hit.collider != null){
    //         GameObject hitObject = hit.collider.gameObject;
    //         if(hitObject.CompareTag("deadAnimal")){
    //             //player is within the inner circle of the animal, animal will flee
    //             FleeCheck();
    //             return;
    //         }
    //     }
    // }

    // * when fleeing, cow should move in a straight line * //
    IEnumerator Flee(){
        
        // * pick a random direction for the cow to flee * //
        Vector2 dir = Random.insideUnitCircle.normalized;
        float elapsed = 0f;
        cow.velocity = Vector2.zero;

        //Debug.Log("start of flee, elapsed: "+elapsed+", isFleeing: "+isFleeing);
        // * cow will flee for 5 seconds * //
        while(elapsed < 5.0f && isFleeing){
            //stop moving
            StopCoroutine(move);
            RotateTowardsDirection(-dir);
            cow.velocity = dir.normalized * runSpeed * fleeSpeedMultiplyer;
            yield return null;
        }
        cow.velocity = Vector2.zero;
        isFleeing = false;
    }


    public void FleeCheck(){ //checks distance from player befor starting/stopping the flee
        float distanceToPlayer = (player.transform.position - transform.position).magnitude;
        
        // * cow flees once it gets hit * //
        if(isHit){
            // Debug.Log("starting flee in FleeCheck()");
            Debug.Log("Cow is injured. MOO-ve out!");
            StopCoroutine(move);
            cow.velocity = Vector2.zero;
            StartCoroutine(Flee());
            cowInjured = true;
            isFleeing = true;
        }
        if(distanceToPlayer >= fleeDistance && isFleeing){
            //Debug.Log("Stoping flee in FleeCheck()");
            StopCoroutine(Flee());
            cow.velocity = Vector2.zero;
            isFleeing = false;
            isHit = false;

            // * cow takes an extended break after fleeing * //

        }
    }

    void RotateTowardsDirection(Vector2 direction){
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // * Cow moves around here * //
    IEnumerator MoveAround(){
        if(cow == null){
            Debug.LogError("No Rigidbody2D attached to Cow");
            yield break;
        }
        // start = timer;
        cow.velocity = Vector2.zero;
        // choose a random direction to rotate to

        // * Cow is not dead * //
        while(!isDead)
        {
            isWandering = true;
            // * rotate towards this direction
            Vector2 dir = Random.insideUnitCircle.normalized;
            RotateTowardsDirection(dir);            

            float moveTime = Random.Range(2f, 5f);
            float elapsed = 0f;
            
            while(elapsed < moveTime){
                RotateTowardsDirection(-dir);
                // direction *= -1.0f;
                cow.velocity = dir.normalized * runSpeed;
                
                //update elapsed time
                elapsed += Time.deltaTime;

                yield return null;
            }

            cow.velocity = Vector2.zero;
            float waitTime = Random.Range(3f, 5f);
            elapsed = 0f;
            while(elapsed < waitTime){
                elapsed += Time.deltaTime;

                yield return null;
            }
            yield return null;
            // wait for the next frame to continue the movement
        }
    }

    void MonsterCheck(float Adistance, float Pdistance){ //do the monster checks
        if(AwarenessCheck(Adistance) || PerceptionCheck(Pdistance)){
            //Debug.Log("flee check 2");
            FleeCheck();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        Animal type = cow
        Health = 200
        awareness level = 0.8
        run speed = 1.5
        flee speed multiplyer
        calorie % = 0.08 = 8% 
        */
        cow = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SetAnimalData("Cow", 250, 0.3f, 2.0f, 1.5f, 0.35f); //setting the animal data for a cow
        //currentHealth = maxHealth;
        move = MoveAround();
        StartCoroutine(move);
        player = GameObject.Find("Monster"); 
    }
    
    // Update is called once per frame
    void Update()
    {
        // Debug.Log(cow.velocity);
        if(!isDead){ 
            if(isFleeing){
                StopCoroutine(move);
                isWandering = false;
                //start flee
                //Debug.Log("Flee Check 1");

                FleeCheck();
            }
            else{
                if(!isWandering){
                    //Debug.Log("isWandering: "+isWandering);
                    isWandering = true;
                    StartCoroutine(move);
                } 
                //check if monster is nearby
                MonsterCheck(awareDistance, perceptionDistance);

                //check if there are nearby corpses
                // CorpseCheck(corpseDistance);
            }
        }
        else{
            StopAllCoroutines();
            isWandering = false;
            anim.enabled = false;
        }
    }
}