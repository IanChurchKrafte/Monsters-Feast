using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Physics2D;

//struct of animal data
public struct AnimalData{
    public string animalType;
    public int animalHealth;
    public int awarenessLevel;
    public int runSpeed;
    public Vector2 position;
    public Vector2 rotation;
}

public class GenricAnimal : MonoBehaviour
{
    public string animalType;
    public int animalHealth;
    public int awarenessLevel;
    public int runSpeed;
    Vector2 position;
    Vector2 rotation;
    
    public AnimalData animalData;

    public void GenericAnimal(string type, int health, int awareness, int speed, Vector2 pos, Vector2 rot)
    {
        //basic constructer will probably change
        animalType = type;
        animalHealth = health;
        awarenessLevel = awareness;
        runSpeed = speed;
        position = pos;
        rotation = rot;
    }

    //setting values for a new animal with the struct
    public void SetAnimalData(string type, int health, int awareness, int speed, Vector2 pos, Vector2 rot){
        animalData.animalType = type;
        animalData.animalHealth = health;
        animalData.awarenessLevel = awareness;
        animalData.runSpeed = speed;
        animalData.position = pos;
        animalData.rotation = rot;
    }

    public void AnimalFlee(Vector2 direction){
        Rigidbody2D animal = GetComponent<Rigidbody2D>();
        animal.AddForce(direction * runSpeed, ForceMode2D.Impulse);
    }

    //AwarenessCheck is for the inner circle of the monster's, if the player is inside of that circle the animal will know it is there
    //can be changed to have a chance to be found
    public void AwarenessCheck(Vector2 position, Vector2 rotation, float distance)
    {
        RaycastHit2D hit = Physics2D.CircleCast(position, distance, rotation);
        if(hit.collider != null){
            GameObject hitObject = hit.collider.gameObject;
            if(hitObject.CompareTag("Player")){
                //player is within the inner circle of the animal, animal will flee
                Vector2 normal = hit.normal;
                AnimalFlee(normal);
            }
        }
    }

    //PerceptionCheck is for where the animal is looking
    //It will be many raycast in a cone shape, each checking to see if it sees the animal
    public void PerceptionCheck(Vector2 position, Vector2 rotation, float distance)
    {
        //setting up constants
        float coneAngle = 30.0f; //total angle of cone in degrees
        float coneRadius = distance; //distance cones should travel
        int numRays = 20; //number of rays to cast
        float angleStep = coneAngle / numRays; //angle between each ray

        //creating a list to store the hits
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        //loop through each ray, rotating the ray by angleStep degrees each iteration
        for(int i=0; i<=numRays; i++){
            //calculate the angle of ray for this iteration
            //initialized towards negative half of the cone angle and increased by angleStep
            float angle = -coneAngle / 2.0f + i*angleStep;

            //calculates the direction of each ray at that angle
            Vector2 direction = Quaternion.Euler(0,0, angle) * Vector2.right;
            RaycastHit2D hit = Physics2D.Raycast(position, rotation, coneRadius);
            if(hit.collider != null){
                //checking to see if collided with player
                GameObject hitObject = hit.collider.gameObject;
                if(hitObject.CompareTag("Player") && hit.distance <= distance){
                    //player is within the inner circle of the animal, animal will flee
                    Vector2 normal = hit.normal;
                    AnimalFlee(normal);
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //placeholder variables
        Vector2 position = new Vector2(0,0);
        Vector2 rotation = new Vector2(0,0);
        float distance = 5.0f;
        AwarenessCheck(position, rotation, distance);
        PerceptionCheck(position, rotation, distance);
    }


}
