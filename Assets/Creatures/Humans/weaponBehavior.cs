using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class weaponBehavior : GenericHuman
{
    //so all the public variables dont show up in the inspector in unity
    // [HideInInspector]
    // public int maxHealth;
    // public int currentHealth;
    // public bool isHostile = false, isChasing = false;
    // public Vector2 baseLocation = new Vector2(22,22);
    // public HumanType humanType = HumanType.normal;
    // public float attackRange = 2.0f;
    // public bool isDead = false;
    // public float walkSpeed = 2.0f, runSpeed = 3.5f, fleeSpeed = 4.5f;
    // public float monsterVisibleRange =  35.0f, visibleAngle = 120.0f;
    // public bool isFleeing = false;

    //on trigger, if it comes into contact with something do damage to it
        //either an animal, the player, or another human
    
    // public enum HumanType{
    //     normal,
    //     pitchfork,
    //     crossbow,
    //     ballista
    // }
    public enum WeaponType{
        pitchfork,
        crossbow,
        ballista,
        nothing
    }
    public WeaponType weaponType;
    public GameObject weapon;
    public GameObject boltPrefab, bigBoltPrefab;
    private GameObject player;
    public float pitchforkCooldown = 5.0f, crossbowCooldown = 1.5f, ballistaCooldown = 12.5f;
    private bool pitchOnCooldown = false, crossOnCooldown = false, ballistaOnCooldown = false;
    public float crossbowRange = 20.0f, ballistaRange = 30.0f;
    public Vector2 offsetVector;
 
    
    //public Vector3 pitchOffsetVector, crossOffsetVector, ballistaOffsetVector;
    // Start is called before the first frame update

    private IEnumerator PitchforkAttack(Vector2 attackDirection){
        //attack in the given direction, 
        //jab the pitchfork forward
        //Debug.Log("attacking 2");
        if(pitchOnCooldown) yield break; //attack on cooldown
        float jabDistance = 2.0f;
        Debug.Log("in pitchfork attack"); //for testing
        pitchOnCooldown = true;
        yield return StartCoroutine(PitchforkReload(pitchforkCooldown));

        Vector2 attackPosition = (Vector2)transform.position + attackDirection.normalized * jabDistance;
        // weapon.transform.position = attackPosition;
        Vector3 startPosition = weapon.transform.position;
        Vector3 endPosition = attackPosition;

        float jabDuration = 0.25f;
        float elapsedTime = 0f;

        while(elapsedTime < jabDuration){
            weapon.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / jabDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackPosition, new Vector2(0.5f, 0.5f), 0);
        foreach (Collider2D hitCollider in hitColliders){
            MonsterBehavior monster = hitCollider.gameObject.GetComponent<MonsterBehavior>();
            GenericHuman human = hitCollider.gameObject.GetComponent<GenericHuman>();
            GenericAnimal animal = hitCollider.gameObject.GetComponent<GenericAnimal>(); 

            if(monster != null) monster.TakeDamage(10);
            if(human != null) human.TakeDamage(10);
            if(animal != null) animal.AnimalDamage(10);
        }
        
        elapsedTime = 0f;

        while(elapsedTime < jabDuration){
            weapon.transform.position = Vector3.Lerp(endPosition, startPosition, elapsedTime / jabDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weapon.transform.position = startPosition;
        weapon.transform.position = (Vector2)transform.parent.position + offsetVector;
    }

    // private IEnumerator MoveWeaponTo(Vector2 tar)

    private IEnumerator PitchforkReload(float cooldown){
        //cooldown for the pitchfork, lowest cooldown
        yield return new WaitForSeconds(cooldown);
        pitchOnCooldown = false;
    }

    private IEnumerator CrossbowAttack(Vector2 attackDirection){
        //attack in the given direction,
        //fires a crossbow bolt

        if(boltPrefab == null){
            Debug.Log("bolt prefab not assigned");
            yield break;
        }

        if (crossOnCooldown) yield break;
        Debug.Log("in crossbow attack");

        crossOnCooldown = true;
        yield return StartCoroutine(CrossbowReload(crossbowCooldown));

        //instantiate the bolt prefab
        GameObject bolt = Instantiate(boltPrefab, transform.position, Quaternion.identity);
        bolt.GetComponent<HitboxBehavior>().owner = transform.parent.gameObject;

        //rotate the arrow to face the attack direction
        bolt.transform.rotation = transform.parent.rotation;

        //add velocity to the arrow in the right direction
        Rigidbody2D boltRB = bolt.GetComponent<Rigidbody2D>();
        boltRB.velocity = attackDirection.normalized * 20f;

        Destroy(bolt, 10.0f);
    }

    private IEnumerator CrossbowReload(float cooldown){
        //cooldown for the crossbow, while it reloads
        yield return new WaitForSeconds(cooldown);
        crossOnCooldown = false;
    }

    private IEnumerator BallistaAttack(Vector2 attackDirection){
        //attack in the given direction,
        //fires a slower ballista shot that deals more damage

        if(bigBoltPrefab == null){
            Debug.Log("bigBolt prefab not assigned");
            yield break;
        }

        if(ballistaOnCooldown) yield break;

        ballistaOnCooldown = true;
        yield return StartCoroutine(BallistaReload(ballistaCooldown));

        //instantiate the bigBolt prefab
        GameObject bigBolt = Instantiate(bigBoltPrefab, transform.position, Quaternion.identity);
        bigBolt.GetComponent<HitboxBehavior>().owner = transform.parent.gameObject;

        bigBolt.transform.rotation = transform.parent.rotation;

        Rigidbody2D bigBoltRB = bigBolt.GetComponent<Rigidbody2D>();
        bigBoltRB.velocity = attackDirection.normalized * 15f;
    }

    private IEnumerator BallistaReload(float cooldown){
        //cooldown for ballista
        yield return new WaitForSeconds(cooldown);
        ballistaOnCooldown = false;
    }

    public void Attack(Vector2 direction){
        //Debug.Log("getting into attack function");
        switch(weaponType){
            case WeaponType.pitchfork:
                StartCoroutine(PitchforkAttack(direction));
                break;
            case WeaponType.crossbow:
                StartCoroutine(CrossbowAttack(direction));
                break;
            case WeaponType.ballista:
                StartCoroutine(BallistaAttack(direction));
                break;
        }
    }
    void Start()
    {
        player = GameObject.Find("Monster");
        //set weapons position and rotation relative to the human


        //get weapon type automatically
        GenericHuman humanComp = GetComponentInParent<GenericHuman>();
        HumanType humanType = humanComp.humanType;

        switch(humanType){
            case HumanType.pitchfork:
                weaponType = WeaponType.pitchfork;
                offsetVector = new Vector3(0.36f, 0.38f, 0f);
                break;
            case HumanType.crossbow:
                weaponType = WeaponType.crossbow;
                break;
            case HumanType.ballista:
                weaponType = WeaponType.ballista;
                break;
            case HumanType.normal:
                weaponType = WeaponType.nothing;
                break;
        }


        weapon.transform.parent = transform;
        weapon.transform.localPosition = offsetVector;
        weapon.transform.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
