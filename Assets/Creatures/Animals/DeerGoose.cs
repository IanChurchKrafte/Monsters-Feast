//flees once it sees a corpse
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GenericAnimal;
using Animal;

 
public class DeerGoose : GenericAnimal
{
    public float awareDistance = 5.0f, perceptionDistance = 50.0f;
    public float corpseDistance = 30.0f;
    public float idleDelay = 4.0f;
    public float walkTime = 5.0f;
    private float timer = 0.0f, start;

    Rigidbody2D deerGoose;
    
    private float spottedCorpse;
    private bool isFleeing = false;

    void CorpseCheck(float distance){ //checks for any nearby corpses
        if(isDead) return;

        Vector2 position = transform.position;
        Vector2 rotation = transform.rotation.eulerAngles;
        RaycastHit2D hit = Physics2D.CircleCast(position, distance, rotation);
        if(hit.collider != null){
            GameObject hitObject = hit.collider.gameObject;
            if(hitObject.CompareTag("deadAnimal")){
                //player is within the inner circle of the animal, animal will flee
                Vector2 normal = hit.normal;
                spottedCorpse = timer;
                isFleeing = true;
                AnimalFlee(normal);
                return;
            }
        }
        else{
            if(isFleeing && (timer - spottedCorpse) > 2.0f){
                //stop fleeing
                deerGoose.velocity = Vector2.zero;
                isFleeing = false;
                return;
            }
        }
    }
    // void MoveAround(){
    //     //randomly move around the map, not going through terrain
    //     //walk in a dingle direction for an ammount of time, stop for a bit, then continue on in the same or different direction
    //     Rigidbody2D deerGoose = GetComponent<Rigidbody2D>();
    //     if(!isDead){
    //         if(deerGoose == null){
    //             Debug.LogError("No Rigidbody2D attached to DeerGoose");
    //             return;
    //         }
    //         int walkTime = Random.Range(3, 7); //run for a random time between 3-7 seconds
    //         Vector2 direction = Random.insideUnitCircle.normalized; //pick a random direction to run in
    //         float counter = 0f;
    //         while(counter <= walkTime){
    //             //walk
    //             counter += Time.deltaTime;
    //             deerGoose.AddForce(direction * runSpeed, ForceMode2D.Impulse);
    //             //Debug.Log("counter1: "+counter+" walkTime: "+walkTime);
    //             //Debug.Log("in time test loop");
    //         }  
    //         counter = 0f;
    //         int idleTime = Random.Range(5,7);
    //         while(counter <= idleTime){
    //             //do nothing
    //             counter += Time.deltaTime;
    //             //Debug.Log("counter2: "+counter+" idleTime: "+idleTime);
    //         }
    //     }
    //     else{ //it dead
    //         deerGoose.velocity = Vector2.zero;
    //     }
    // }

    IEnumerator BetterMoveAround(){
        if(deerGoose == null){
            Debug.LogError("No Rigidbody2D attached to DeerGoose");
            yield break;
        }
        if(!isDead){
            start = timer;
            //pick walk direction
            Vector2 target = RandomMoveTarget();
            
            //rotate to that direction
            RotateToPoint(target);
            
            //walk
            float counter = 0f;
            while(Vector2.Distance(transform.position, target) > 0.1f){
                float currentDistance = Vector2.Distance(transform.position, target);

                float step = runSpeed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, target, step);

                //something went wrong in the loop and the 2 if statements help to prevent an infinite loop
                    //check if distance is somehow getting bigger
                    if (Vector2.Distance(transform.position, target) >= currentDistance)
                        break;

                    //check if the frog has been trying to jump to the same target for too long, and break out of the loop if it has
                    if (timer - start > idleDelay * 2)
                        break;
                    
                    if(timer - start > walkTime){
                        deerGoose.velocity = Vector2.zero;
                        break;
                    }

                //wait for the next fram to continue the movement
                yield return null;
            }

            deerGoose.velocity = Vector2.zero; //Stop moving when walkTime has elapsed
        }
        else{ //it dead
            Debug.Log("IM DEAD");
            deerGoose.velocity = Vector2.zero;
        }
    }

    void TimeCheck(){
        if(timer - start > idleDelay){//if elapsed time is over 2 seconds
            StartCoroutine(BetterMoveAround());
        } 
    }

    Vector2 RandomMoveTarget(){
        Vector2 target = new Vector2(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(-3f, 3f));
        return target;
        
    }

    void RotateToPoint(Vector2 target){
        //Debug.Log("Target: "+target);
        Vector2 frogPos = gameObject.transform.position;
        Vector3 frogPos3D = new Vector3(frogPos.x, frogPos.y, 0);
        Vector3 target3D = new Vector3(target.x, target.y, 0);
        
        Vector3 direction = target3D - frogPos3D;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.Rotate(new Vector3(0,0,-90));  
    }
    void MonsterCheck(float Adistance, float Pdistance){ //do the monster checks
        AwarenessCheck(Adistance);
        PerceptionCheck(Pdistance);
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        Animal type = DeerGoose
        Health = 200
        awareness level = 0.8
        run speed = 0.00005f //testing
        flee speed multiplyer = 1.8x
        calorie % = 0.08 = 8% 
        */
        deerGoose = GetComponent<Rigidbody2D>();
        SetAnimalData("DeerGoose", 150, 0.8f, 0.5f, 1.8f, 0.18f); //setting the animal data for a DeerGoose
    }
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        //move around randomly
        if(!isDead)
            TimeCheck();
        else{
            deerGoose.velocity = Vector2.zero;
        }
        //check for monster
        
        MonsterCheck(awareDistance, perceptionDistance);
        //check for corpses
        
        CorpseCheck(corpseDistance * gameObject.transform.localScale.x);
        //check for humans?
    }
}
