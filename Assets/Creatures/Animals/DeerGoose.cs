//flees once it sees a corpse
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GenericAnimal;
using Animal;

 
public class DeerGoose : GenericAnimal
{
    // public int maxHealth = 150;
    // public int currentHealth;
    public float awareDistance = 5.0f, perceptionDistance = 50.0f;
    public float corpseDistance = 30.0f;
    public float walkSpeed = 1.25f, sprintSpeed = 1.25f;
    public float fleeDistance = 20.0f;
    


    Rigidbody2D deerGoose;
    
    private float spottedCorpse;
    public bool isFleeing = false;
    private GameObject player;
    IEnumerator move;
    private bool isMoving = false;

    void CorpseCheck(float distance){ //checks for any nearby corpses
        Vector2 position = transform.position;
        Vector2 rotation = transform.rotation.eulerAngles;
        RaycastHit2D hit = Physics2D.CircleCast(position, distance, rotation);
        if(hit.collider != null){
            GameObject hitObject = hit.collider.gameObject;
            if(hitObject.CompareTag("deadAnimal")){
                //player is within the inner circle of the animal, animal will flee
                // Vector2 normal = hit.normal;
                // spottedCorpse = timer;
                // isFleeing = true;
                // AnimalFlee(normal);
                //Debug.Log("Flee Check 3");
                FleeCheck();
                return;
            }
        }
        // else{
        //     if(isFleeing && (timer - spottedCorpse) > 2.0f){
        //         //stop fleeing
        //         deerGoose.velocity = Vector2.zero;
        //         isFleeing = false;
        //         return;
        //     }
        // }
    }
        IEnumerator Flee(){
            Vector2 direction = transform.position - player.transform.position;
            float elapsed = 0f;

            float distanceToPlayer = (player.transform.position - transform.position).magnitude;

            //Debug.Log("start of flee, elapsed: "+elapsed+", isFleeing: "+isFleeing);
            while(elapsed < 5.0f && isFleeing){
                //stop moving
                StopCoroutine(move);

                direction = player.transform.position - transform.position;

                distanceToPlayer = (player.transform.position - transform.position).magnitude;

                RotateTowardsDirection(direction);

                //Debug.Log("A deergoose is fleeing");
                if(distanceToPlayer < 5.0f){
                    transform.Translate(Vector3.up * Time.deltaTime * sprintSpeed * 0.05f);
                }
                else{
                    transform.Translate(Vector3.up * Time.deltaTime * sprintSpeed * 0.01f);
                }
                //Debug.Log("Speed: "+deerGoose.velocity.magnitude);
                //update elapsed time
                elapsed += Time.deltaTime;

                yield return null;
            }
            //Debug.Log("end of flee");
            //StartCoroutine(MoveAround());
            isFleeing = false;
        }

        public void FleeCheck(){ //checks distance from player befor starting/stopping the flee
            float distanceToPlayer = (player.transform.position - transform.position).magnitude;
            
            if(distanceToPlayer < fleeDistance){
                //Debug.Log("starting flee in FleeCheck()");
                StartCoroutine(Flee());
                StopCoroutine(move);
                isFleeing = true;
            }
            if(distanceToPlayer >= fleeDistance && isFleeing){
                //Debug.Log("Stoping flee in FleeCheck()");
                StopCoroutine(Flee());
                isFleeing = false;
                startFlee = false;
            }
        }

    void RotateTowardsDirection(Vector2 direction){
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    IEnumerator MoveAround(){
        while(true && !isDead){
            // if(isFleeing) yield return null;
            isMoving = true;
            //choose a random direction
            Vector2 direction = Random.insideUnitCircle.normalized;
            
            //rotate in that direction
            RotateTowardsDirection(direction);

            //move in that direction
            float moveTime = Random.Range(2f, 5f);
            float elapsed = 0f;
            
            while(elapsed < moveTime){
                transform.Translate(Vector3.up * Time.deltaTime * walkSpeed);

                //update elapsed time
                elapsed += Time.deltaTime;

                yield return null;
            }
            // isMoving = false;
            //Debug.Log("im still in move around");
            //stop moving and wait in place for a bit
            float waitTime = Random.Range(3f, 5f);
            elapsed = 0f;
            while(elapsed < waitTime){
                elapsed += Time.deltaTime;

                yield return null;
            }
            yield return null;
            //isMoving = false;

        }
        if(isDead) deerGoose.velocity = Vector2.zero;
    }
    private IEnumerator walkAroundDelay;
    IEnumerator WalkAroundDelay(){
        yield return new WaitForSeconds(3.5f);
        walkAroundDelay = null;
    }

    void MonsterCheck(float Adistance, float Pdistance){ //do the monster checks
        if(AwarenessCheck(Adistance) || PerceptionCheck(Pdistance)){
            //Debug.Log("flee check 2");
            FleeCheck();
        }
    }

    // public void TakeDamage(int damage){
    //     int temp = currentHealth - damage;
    //     if(temp <= 0){
    //         //dead
    //         currentHealth = 0;
    //     }
    //     else
    //         currentHealth = temp;
    // }


    // Start is called before the first frame update
    void Start()
    {
        /*
        Animal type = DeerGoose
        Health = 200
        awareness level = 0.8
        run speed = 1.5
        flee speed multiplyer
        calorie % = 0.08 = 8% 
        */
        deerGoose = GetComponent<Rigidbody2D>();
        SetAnimalData("DeerGoose", 150, 0.8f, 0.5f, 1.8f, 0.18f); //setting the animal data for a DeerGoose
        //currentHealth = maxHealth;
        move = MoveAround();
        StartCoroutine(move);
        player = GameObject.Find("Monster"); 
    }

    
    
    
    // Update is called once per frame
    void Update()
    {
        if(!isDead){ 
            if(isFleeing){
                StopCoroutine(move);
                isMoving = false;
                //start flee
                //Debug.Log("Flee Check 1");

                FleeCheck();
            }
            else{
                if(!isMoving){
                    //Debug.Log("isMoving: "+isMoving);
                    isMoving = true;
                    StartCoroutine(move);
                } 
                //check if monster is nearby
                MonsterCheck(awareDistance, perceptionDistance);

                //check if there are nearby corpses
                CorpseCheck(corpseDistance);
            }
        }
        else{
            StopCoroutine(move);
            isMoving = false;
        }
    }
}