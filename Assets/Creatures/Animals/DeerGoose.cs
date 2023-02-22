//flees once it sees a corpse
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GenericAnimal;
using Animal;

 
public class DeerGoose : GenericAnimal
{
    void CorpseCheck(float distance){ //checks for any nearby corpses
        Vector2 position = transform.position;
        Vector2 rotation = transform.rotation.eulerAngles;
        RaycastHit2D hit = Physics2D.CircleCast(position, distance, rotation);
        if(hit.collider != null){
            GameObject hitObject = hit.collider.gameObject;
            if(hitObject.CompareTag("deadAnimal")){
                //player is within the inner circle of the animal, animal will flee
                Vector2 normal = hit.normal;
                AnimalFlee(normal);
            }
        }
    }
    void MoveAround(){
        //randomly move around the map, not going through terrain
        //walk in a dingle direction for an ammount of time, stop for a bit, then continue on in the same or different direction
        Rigidbody2D deerGoose = GetComponent<Rigidbody2D>();
        if(!isDead){
            if(deerGoose == null){
                Debug.LogError("No Rigidbody2D attached to DeerGoose");
                return;
            }
            int walkTime = Random.Range(3, 7); //run for a random time between 3-7 seconds
            Vector2 direction = Random.insideUnitCircle.normalized; //pick a random direction to run in
            float counter = 0f;
            while(counter <= walkTime){
                //walk
                counter += Time.deltaTime;
                deerGoose.AddForce(direction * runSpeed, ForceMode2D.Impulse);
                //Debug.Log("counter1: "+counter+" walkTime: "+walkTime);
                //Debug.Log("in time test loop");
            }  
            counter = 0f;
            int idleTime = Random.Range(5,7);
            while(counter <= idleTime){
                //do nothing
                counter += Time.deltaTime;
                //Debug.Log("counter2: "+counter+" idleTime: "+idleTime);
            }
        }
        else{ //it dead
            deerGoose.velocity = Vector2.zero;
        }
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
        run speed = 1.5
        flee speed multiplyer
        calorie % = 0.08 = 8% 
        */
        SetAnimalData("DeerGoose", 200, 0.8f, 0.000005f, 1.8f, 0.08f); //setting the animal data for a DeerGoose
    }
    
    // Update is called once per frame
    void Update()
    {
        //move around randomly
        MoveAround();
        //check for monster
        float awareDistance = 50.0f, perceptionDistance = 100.0f;
        MonsterCheck(awareDistance, perceptionDistance);
        //check for corpses
        float corpseDistance = 30.0f;
        CorpseCheck(corpseDistance);
        //check for humans?
    }
}
