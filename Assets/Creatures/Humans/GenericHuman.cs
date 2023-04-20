using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHuman : MonoBehaviour
{
    //stay around the area of the home base
    //go after the monster when sighted
    //keep track of how many cows have been killed
    //if its > 0 they will attack the monster
    //if they see the monster attacking the cow, they will be hostile

    public int maxHealth;
    public int currentHealth;
    public float calories = 0.1f, consumed;
    //if they are hostile towards the player, will attack on sight
    public bool isHostile = false, isChasing = false;
    //base location
    public Vector2 baseLocation = new Vector2(22,22);
    //when not chasing, this is how far they an travel (not too far from the base)
    //public float baseZone = 50.0f;
    //how far they will chase the player away from the base
   // public float chaseZone = 150.0f;
    //3 human types: 1->Pitchfork human, 2->crossbow human, 3->ballista human
    public enum HumanType{
        normal,
        pitchfork,
        crossbow,
        ballista
    }
    public HumanType humanType = HumanType.normal;

    public float attackRange = 2.0f;

    //dont do anything if dead
    public bool isDead = false;

    //speeds
    public float walkSpeed = 2.0f, runSpeed = 3.5f, fleeSpeed = 4.5f;

    public float monsterVisibleRange = 25.0f, visibleAngle = 120.0f, ballistaVisibleRange = 30.0f;

    //public int wanderRange = 25;

    private Rigidbody2D human;
    
    //private float timer = 0.0f, start, end;
    //private float idleDelay = 3.0f;
    //private bool isMoving = false;
    //private int first = 0;
    // private bool canAttack = true;

    public bool isFleeing = false;
    public bool isAttacking = false;

    //private IEnumerator move, hunt, flee;
    //private Coroutine moveStart = null, huntStart = null, fleeStart = null;
    //private Coroutine moveStop = null, huntStop = null, fleeStop = null;

    //for the cow checks, to see if the humans should become hostile toward the monster
    //private Cow[] cows;
    void MonsterCheck(){
        //check if monster is visible
        //only gets agro when isHostile = true
        if(isHostile){
            GameObject player = GameObject.Find("Monster");
            Vector2 directionToPlayer = player.transform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            //check if player is in visible range
            if(distanceToPlayer <= monsterVisibleRange){
                //instead of raycast, using OverlapCircleAll
                //gets a list of all colliders that fall within a circular area, that area being the visible range of the human to see the monster
                //Debug.Log("in visible range");
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, monsterVisibleRange);

                foreach(Collider2D collider in colliders){
                    if(collider.gameObject.CompareTag("Monster") || collider.gameObject.CompareTag("Player")){
                        //check if player is within the visible angle
                        //Debug.Log("found in collider");
                        //Debug.Log(collider.gameObject);
                        Vector2 directionToCollider = collider.transform.position - transform.position;
                        float angle = Vector2.Angle(transform.up, directionToCollider);

                        if(angle <= visibleAngle/2f){
                            //Debug.Log("Starting chase");
                            isChasing = true;
                            //start to chase and attack the player
                            return;
                        }
                    }
                }
            }
            else if(distanceToPlayer >= 2.5 * monsterVisibleRange){
                Debug.Log("Distance too far, setting isChasing = false");
                isChasing = false;
            }
            //Debug.Log("not chasing");
            isChasing = false;
            StartCoroutine(MoveAround());
            // counter = false;
        }
    }
    
    void cowCheck(){
        //check to see if cow is currently being injured, or if cow is dead
        // for(int i=0; i<cows.length; i++){
        //     if(cows[i] == null) continue;

        //     if(cows[i].IsBeingAttacked || cows[i].isDead){
        //         //cow is being attacked or cow is dead, set isHostile to true
        //         isHostile = true;
        //     }
        // }
    }

    void RotateToPoint(Vector2 target){
        Vector2 direction = target - (Vector2)transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.Rotate(new Vector3(0,0,-90));  
    }

    void RotateTowardsDirection(Vector2 direction){
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //TODO: do smooth rotation, just snaps now
    }



    IEnumerator MoveAround(){
        if(isFleeing || isDead || isChasing) yield return null;

        while(true && !isDead && !isChasing && !isFleeing){
            //choose a random direction
            Vector2 direction = Random.insideUnitCircle.normalized;
            //Debug.Log("in move");
            //rotate in that direction
            RotateTowardsDirection(direction);

            //move in that direction
            float moveTime = Random.Range(2f, 5f);
            float elapsed = 0f;
            human.velocity = (-direction) * walkSpeed;
            while (elapsed < moveTime){
                //transform.Translate(Vector3.up * Time.deltaTime * walkSpeed);// * 0.01f);
                // Debug.Log("moving");
                //update elapsed time
                elapsed += Time.deltaTime;

                yield return null;
            }
            human.velocity = Vector3.zero;
            //stop moving and wait in place for a bit
            float waitTime = Random.Range(3f, 5f);
            elapsed = 0f;
            while(elapsed < waitTime){
                elapsed += Time.deltaTime;
                yield return null;
            }

            //check if still in base or not, if not return to base area
        }
        moveTime = false;
    }

    IEnumerator Hunting(){
        if(!isChasing) yield return null;
        
        //when the humans are chasing and trying to attack the player
        //when they get too far from the base they will give up.
        //Debug.Log("trying to hunt");
        if(isChasing){
            
            GameObject player = GameObject.Find("Player");
            if(!player){
                player = GameObject.Find("Monster");
            }
            if(!player){ //player/monster object still can't be found
                Debug.Log("Can't find the player entity with GameObject.Find in Hunting()");
                yield return null;
            }
            while(player && isChasing){
                //Debug.Log(player.transform.position);
                Vector2 direction = player.transform.position - transform.position;
                //check if player is in attack range
                if(direction.magnitude < attackRange && !isAttacking){
                    isAttacking = true;
                    //Debug.Log("trying to attack");
                    direction = player.transform.position - transform.position;
                    //weaponBehavior weapon = transform.GetChild(0).GetComponent<weaponBehavior>();
                    //weapon.Attack(direction);
                    AttackStart();
                    human.velocity = Vector2.zero;
                }
                else if (!isAttacking){
                    //rotate to player
                    RotateToPoint(player.transform.position);

                    if(direction.magnitude > 1.9f){
                        Vector2 moveDir = (Vector2)player.transform.position - (Vector2)transform.position;
                        //move towards the player
                        //transform.Translate(moveDir.normalized * runSpeed * Time.deltaTime);
                        human.velocity = moveDir.normalized * runSpeed;
                    }
                    
                    float distance = direction.magnitude;
                    //Debug.Log("Im trying to get in range for an attack: distance = "+distance);

                    if(distance > monsterVisibleRange || Vector2.Angle(transform.up, direction) > visibleAngle / 2.0f){
                        //isChasing = false;
                        MonsterCheck();
                        Debug.Log("Out of my sight range, stopping the chase");
                    }

                }
                yield return null;
            }
            //pause the coroutine for the next fixed update
            yield return new WaitForFixedUpdate();

            //delay the next chase update
            yield return new WaitForSeconds(1.0f);
            //check if too far from base or out of chase zone (trigger)
        }
    }

    void AttackStart()
    {
        GetComponent<Animator>().SetTrigger("Attacking");
    }
    void ActivatePitchfork()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
    void DeactivatePitchfork()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        isAttacking = false;
    }

    IEnumerator Flee(){
        if(!isFleeing) yield return null;

        GameObject player = GameObject.Find("Monster");

        //get direction to base
        Vector2 direction = (Vector2)transform.position - baseLocation;

        //get distance to base
        float distanceToBase = (baseLocation - (Vector2)transform.position).magnitude;
        human.velocity = (-direction.normalized) * fleeSpeed;
        //move towards the base
        while (distanceToBase > 5.0f){
            direction = (Vector2)transform.position - baseLocation;
            distanceToBase = (baseLocation - (Vector2)transform.position).magnitude;
            RotateTowardsDirection(direction);

            //Debug.Log("Human fleeing to base, distance: "+distanceToBase);
            //transform.Translate(Vector3.up * Time.deltaTime * fleeSpeed);

            yield return null;
        }
        human.velocity = Vector2.zero;
        isFleeing = false;
        moveTime = false;
    }

    void BallistaAim(Vector2 direction){
        // GameObject player = GameObject.Find("Monster");

        // Vector2 targetPos = (Vector2)player.transform.position;
        // Vector2 direction = targetPos - (Vector2)transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.Rotate(new Vector3(0,0,-90));
    }

    void MonsterCheckBallista(){
        //only enter this if isHostil is true
        //check to see if the monster is nearby,
        //if so call rotate function and,
        //if it is range then attack

        GameObject player = GameObject.Find("Monster");
        Vector2 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if(distanceToPlayer <= ballistaVisibleRange){
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ballistaVisibleRange);

            foreach(Collider2D collider in colliders){
                string tag = collider.gameObject.tag;
                if(tag == "Monster" || tag == "Player"){
                    Vector2 directionToCollider = collider.transform.position - transform.position;
                    float angle = Vector2.Angle(transform.up, directionToCollider);

                    if(angle <= visibleAngle/2f){
                        //Monster is within sight of the Ballista
                        directionToPlayer = player.transform.position - transform.position;
                        BallistaAim(directionToPlayer); //need a new ballista aim function
                        weaponBehavior weapon = transform.GetChild(0).GetComponent<weaponBehavior>();
                        weapon.Attack(directionToPlayer);
                    }
                }
            }
        }
        else if(distanceToPlayer >= 2.0 * ballistaVisibleRange){
            Debug.Log("Distance to far, ballista won't fire anymore");
            return;
        }        
    }


    // Start is called before the first frame update
    void Start()
    {
        human = GetComponent<Rigidbody2D>();
        //StartCoroutine(Hunting());
        switch(humanType){
            case HumanType.pitchfork:
                attackRange = 1.8f;
                break;
            case HumanType.crossbow:
                attackRange = 5.0f;
                break;
            case HumanType.ballista:
                attackRange = 7.5f;
                break;
        }
        currentHealth = maxHealth;
        //StartCoroutine(MoveAround());
        
        // moveStart = StartCoroutine(MoveAround());
        // huntStart = StartCoroutine(Hunting());
        // fleeStart = StartCoroutine(Flee());
        // huntStop = StopCoroutine(Hunting());
        // fleeStop = StopCoroutine(Flee());
        // moveStop = StopCoroutine(MoveAround());
        
        StopAllCoroutines();
        if(humanType != HumanType.ballista) StartCoroutine(MoveAround());
        moveTime = true;
    }

    // void OnBecameVisible(){
    //     if(humanType == HumanType.ballista){
    //         enabled = true;
    //     }
    // }
    // void OnBecameInvisible(){
    //     if(humanType == HumanType.ballista){
    //         enabled = false;
    //     }
    // }

    public void TakeDamage(int damage){
        int temp = currentHealth - damage;
        if(temp <= 0){
            //dead
            currentHealth = 0;
            isDead = true;
            gameObject.tag = "deadAnimal";
            gameObject.layer = LayerMask.NameToLayer("DeadAnimal");
            GetComponent<SpriteRenderer>().color = new Color32(121, 29, 29, 255);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            Debug.Log("A human has been killed.");
            if(humanType != HumanType.ballista)
                transform.position += new Vector3(0, 0, 0.2f);
        }
        else{
            currentHealth = temp;
            if(humanType == HumanType.normal || humanType == HumanType.pitchfork){
                //flee
                isFleeing = true;
            }
        }
    }



    // private bool counter = false;

    private bool moveTime = false;//, chaseTime = false, fleeTime = false;
    // Update is called once per frame
    void Update()
    {
        if(!isDead && humanType != HumanType.ballista){
            //timer += Time.deltaTime;
            
            if(!isChasing && !isFleeing && !isHostile){
                //normal move around
                if(!moveTime){ 
                    Debug.Log("move Check");
                    StartCoroutine(MoveAround());
                    moveTime = true;
                }
                else{
                    //already moving, don't call moveAround again
                }
            }
            else if(!isChasing && !isFleeing && isHostile){
                //human is now hostile
                //normal move around, but will check for the monsters presence
                if(!moveTime){
                    StartCoroutine(MoveAround());
                    moveTime = true;
                }
                MonsterCheck();
            }
            else if(isChasing && !isFleeing){
                StopAllCoroutines();

                StartCoroutine(Hunting());
            }
            else if(isFleeing){
                isChasing = false;
                isHostile = false;
                StopAllCoroutines();

                StartCoroutine(Flee());
            }

            //cows = FindObjectsOfType<cowCheck>();
        }
        else if(!isDead && humanType == HumanType.ballista && isHostile){
            //ballista behavior since its not normal behavior
            //check for monster nearby if isHostile, 
            //if monster nearby then rotate towards monster and fire
            MonsterCheckBallista();

        }
        else{ //isDead
            StopAllCoroutines();
            gameObject.tag = "deadAnimal";
        }
    }
}
