using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class ChameleToad : GenericAnimal
{
    // when player is seen go low alpha, doesnt flee
    // when spots carcass, goes low alpha and flees
    // Start is called before the first frame update
    public float alpha = 1.0f;
    public float jumpDelay = 3.0f;
    public float jumpSpeed = 0.5f;
    private Vector2 jumpTarget;
    // private bool jumping = false;
    //public float jumpHeight = 10f;
    private float timer = 0.0f, start;
    public float corpseDistance = 5.0f;
    public bool isFleeing = false;
    
    private GameObject player;

    private Rigidbody2D rb;
    private Animator anim;

    IEnumerator move, flee;

    //alpha logic
    /*

    */

    void changeAlpha(bool val){
        //if val == 1, set alpha to normal 100%
        //else, set alpha to 5%
        //set alpha value
        if(val)
            alpha = 1.0f;
        else
            alpha = 0.2f;

        if(!val){
            StartCoroutine(WaitAndChangeAlpha(20.0f));
        }
    }

    IEnumerator WaitAndChangeAlpha(float time){
        yield return new WaitForSeconds(time);
        alpha = 1.0f;
        checkAlpha();
    }

    void checkAlpha(){
        SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
        Color spriteColor = spriteRender.material.color;

        //set alpha value
        spriteColor.a = alpha;

        //update material color
        spriteRender.material.color = spriteColor;
    }

    IEnumerator newFrogFlee(Vector2 position){
        int numJumps = 5;
        float jumpInterval = 1.0f;
        Vector2 fleeDirection = ((Vector2)transform.position - position);
        fleeDirection.Normalize();

        float distanceToCorpse = (position - (Vector2)transform.position).magnitude;

        for(int i=0; i<numJumps; i++){
            jumpTarget =  (Vector2)transform.position + fleeDirection * 0.5f;

            distanceToCorpse = (position - (Vector2)transform.position).magnitude;

            start = timer;

            RotateToPoint(jumpTarget);

            // Debug.Log("set velocity: "+(fleeDirection) * jumpSpeed * 7.5f);
            rb.velocity = (fleeDirection) * jumpSpeed * 7.5f;

            while(Vector2.Distance(transform.position, jumpTarget) > 0.1f){
                float currentDistance = Vector2.Distance(jumpTarget, transform.position);

                float step = jumpSpeed * Time.deltaTime;
                // transform.position = Vector2.MoveTowards(transform.position, jumpTarget, step);
                // Debug.Log("in while loop");
                //something went wrong in the loop and the 2 if statements help to prevent an infinite loop
                //check if distance is somehow getting bigger
                if (Vector2.Distance(transform.position, jumpTarget) >= currentDistance)
                    break;

                //check if the frog has been trying to jump to the same target for too long, and break out of the loop if it has
                if (timer - start > jumpDelay * 2)
                    break;

                yield return null;
            }

            // Debug.Log("end of while");

            
            //Debug.Log("jump complete");
            // rb.velocity *= 0.5f;
            yield return new WaitForSeconds(jumpInterval);
        }
        rb.velocity = Vector2.zero;
        isFleeing = false;
    }

    void CorpseCheck(float distance){ //checks for any nearby corpses
        Vector2 position = transform.position;
        Vector2 rotation = transform.rotation.eulerAngles;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(position, distance, rotation);
        
        int hitCheck = 0;
        foreach (RaycastHit2D hit in hits){
            //Debug.Log("hit collider: "+hit.collider.tag);
           // Debug.Log("hit: "+hit.collider);
            if(hit.rigidbody != null && hit.collider.CompareTag("deadAnimal")){
                //Debug.Log("hit a dead animal");
                Vector2 normal = hit.normal;
                // changeAlpha(false);
                //AnimalFlee(normal);
                isFleeing = true;
                StartCoroutine(newFrogFlee(hit.rigidbody.position));

                hitCheck++;
                //Debug.Log("Alpha: "+alpha);
            }
        }
        if(hitCheck == 0){
            //changeAlpha(true);
        }
    }

    private IEnumerator FrogJump(){
        if(rb == null){
            Debug.LogError("No Rigidbody2D attached to ChameleoToad");
            yield break;
        }
        //pick new jump target
        // Debug.Log("position: "+transform.position);
        jumpTarget = RandomJumpTarget();
        // Debug.Log("im getting into jump");
        

        //reset the start time to compare against the running timer
        start = timer;
        
        //rotate towards that direction
        RotateToPoint(jumpTarget);

        //calculate jump direction and distance
        Vector2 jumpDirection = (jumpTarget - (Vector2)transform.position).normalized;
        float jumpDistance = Vector2.Distance(jumpTarget, transform.position);
        rb.velocity = (jumpDirection) * jumpSpeed * 5.0f;
        while(Vector2.Distance(transform.position, jumpTarget) > 0.1f){
            float currentDistance = Vector2.Distance(transform.position, jumpTarget);

            //transform.position += (Vector3)(jumpDirection * jumpSpeed * Time.deltaTime);
            float step = jumpSpeed * Time.deltaTime;
            // transform.position = Vector2.MoveTowards(transform.position, jumpTarget, step);
            
            // Debug.Log("in frog jump, Distance: "+currentDistance);

            //something went wrong in the loop and the 2 if statements help to prevent an infinite loop
                //check if distance is somehow getting bigger
                if (Vector2.Distance(transform.position, jumpTarget) > currentDistance){
                    // Debug.Log("breaking at 1st if: distance"+Vector2.Distance(transform.position, jumpTarget));
                    break;
                }
                    

                //check if the frog has been trying to jump to the same target for too long, and break out of the loop if it has
                if (timer - start > jumpDelay * 2){
                    // Debug.Log("breaking at 2nd if");
                    break;
                }
                    

            //wait for the next fram to continue the movement
            yield return null;
        }
        rb.velocity = Vector2.zero;
    }

    // void NewFrogJump(){
    //     Debug.Log("im getting into jump");
    //     if(rb == null){
    //         Debug.LogError("No Rigidbody2D attached to ChameleoToad");
    //         //yield break;
    //         return;
    //     }
    //     //pick new jump target
    //     // Debug.Log("position: "+transform.position);
    //     jumpTarget = RandomJumpTarget();
        
        

    //     //reset the start time to compare against the running timer
    //     start = timer;
        
    //     //rotate towards that direction
    //     RotateToPoint(jumpTarget);

    //     //calculate jump direction and distance
    //     Vector2 jumpDirection = (jumpTarget - (Vector2)transform.position).normalized;
    //     float jumpDistance = Vector2.Distance(jumpTarget, transform.position);
    //     rb.velocity = (jumpDirection) * jumpSpeed * 5.0f;
    //     while(Vector2.Distance(transform.position, jumpTarget) > 0.1f){
    //         float currentDistance = Vector2.Distance(transform.position, jumpTarget);

    //         //transform.position += (Vector3)(jumpDirection * jumpSpeed * Time.deltaTime);
    //         float step = jumpSpeed * Time.deltaTime;
    //         // transform.position = Vector2.MoveTowards(transform.position, jumpTarget, step);
            
    //         // Debug.Log("in frog jump, Distance: "+currentDistance);

    //         //something went wrong in the loop and the 2 if statements help to prevent an infinite loop
    //             //check if distance is somehow getting bigger
    //             if (Vector2.Distance(transform.position, jumpTarget) > currentDistance){
    //                 Debug.Log("breaking at 1st if: distance"+Vector2.Distance(transform.position, jumpTarget));
    //                 break;
    //             }
                    

    //             //check if the frog has been trying to jump to the same target for too long, and break out of the loop if it has
    //             if (timer - start > jumpDelay * 2){
    //                 Debug.Log("breaking at 2nd if");
    //                 break;
    //             }
                    

    //         //wait for the next fram to continue the movement
    //         // yield return null;
    //     }
    //     rb.velocity = Vector2.zero;
    // }

    void FrogTimeCheck(){
        if((timer - start > jumpDelay) && !isFleeing){//if elapsed time is over 2 seconds
            // StopAllCoroutines();
            // Debug.Log("starting jump");
            // StartCoroutine(move);
            StartCoroutine(FrogJump());
            // NewFrogJump();
            // Debug.Log("ending jump");
        }
        else if (isFleeing){
            StopCoroutine(FrogJump());
            // StartCoroutine(Frog)
            // Debug.Log("im starting to flee");
        }
        else{
            // Debug.Log("im landing here");
        } 
    }

    void RotateToPoint(Vector2 target){
        //Debug.Log("Target: "+target);
        Vector2 frogPos = gameObject.transform.position;
        Vector3 frogPos3D = new Vector3(frogPos.x, frogPos.y, 0);
        Vector3 target3D = new Vector3(target.x, target.y, 0);
        
        Vector3 direction = target3D - frogPos3D;


        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Vector3 rotFix = new Vector3(0,0,-90);
        // Quaternion desiredRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // StartCoroutine(RotateTowardsPoint(target, desiredRotation));
        gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.Rotate(new Vector3(0,0,-90));  
    }

    Vector2 RandomJumpTarget(){
        // jumpTarget = Random.insideUnitCircle.normalized * 3.0f;
        float x = Random.Range(-3.0f, 3.0f);
        float y = Random.Range(-3.0f, 3.0f);
        jumpTarget = new Vector2(transform.position.x + x, transform.position.y + y);
        return jumpTarget;
        
    }

    void Start()
    {
        /*
        Animal type = ChameleToad
        Health = 150
        awareness level = 0.8
        run speed = 0.00005f
        flee speed multiplyer = 1.8x
        calorie % = 0.08 = 7% 
        */
        SetAnimalData("ChameleToad", 15, 0.8f, 0.00005f, 1.8f, 0.07f); //setting the animal data for a DeerGoose
        changeAlpha(true);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // FrogJump();
        //StartCoroutine(FrogJump());
        player = GameObject.Find("Monster");
        move = FrogJump();
    }

    // bool check = false;
    // Update is called once per frame
    void Update()
    {
        if(!isDead){
            timer += Time.deltaTime;

            //make sure it has the right alpha value
            checkAlpha();

            //for regular move
            // FrogTimeCheck();
            
            //for fleeing
            CorpseCheck(corpseDistance * gameObject.transform.localScale.x);

            if(isFleeing){
                // Debug.Log("here");
                StartCoroutine(newFrogFlee(player.transform.position));
                isFleeing = false;
            }
        }
        else if (!isDead && isFleeing){
            
        }
        else{
            rb.velocity = Vector2.zero;
            anim.enabled = false;
        }
    }
}
