using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Physics2D;


//struct of animal data
//dead
public struct AnimalData{
    public string animalType;
    public int animalHealth;
    public int awarenessLevel;
    //public int speed;
    public Vector2 position;
    public Vector2 rotation;
    public int isDead;
}

namespace Animal{
    public class GenericAnimal : MonoBehaviour
    {
        public bool awake;
        public string animalType = "Generic";
        public int animalHealth = 150;
        public float awarenessLevel = 0.5f;
        public float speed = 1.0f;
        public bool isDead = false;
        public float fleeSpeedMultiplyer = 1.5f;
        public float calories = 0.05f;
        public float consumed = 0;

        public bool startFlee = false;
        public bool tookDamage = false;

        public AnimalData animalData;

        // public void GenericAnimal(string type, int health, int awareness, int speed, Vector2 pos, Vector2 rot)
        // {
        //     //basic constructer will probably change
        //     animalType = type;
        //     animalHealth = health;
        //     awarenessLevel = awareness;
        //     speed = speed;
        //     position = pos;
        //     rotation = rot;
        // }

        //setting values for a new animal
        public void SetAnimalData(string type, int health, float awareness, float Speed, float fleeSpeed, float cal){
            animalType = type;
            animalHealth = health;
            awarenessLevel = awareness;
            speed = Speed;
            fleeSpeedMultiplyer = fleeSpeed;
            calories = cal;
        }

        public void AnimalFlee(Vector2 direction){ //make the animal run in a given direction
            Rigidbody2D animal = GetComponent<Rigidbody2D>();
            animal.AddForce(direction * (speed * fleeSpeedMultiplyer), ForceMode2D.Impulse);
            Debug.Log("Animal trying to flee");
        }

        

        //AwarenessCheck is for the inner circle of the monster's, if the player is inside of that circle the animal will know it is there
        //can be changed to have a chance to be found
        public bool AwarenessCheck(float distance)
        {
            Vector2 position = transform.position;
            Vector2 rotation = transform.rotation.eulerAngles;
            RaycastHit2D hit = Physics2D.CircleCast(position, distance, rotation);
            if(hit.collider != null){
                GameObject hitObject = hit.collider.gameObject;
                if(hitObject.CompareTag("Player")){
                    //player is within the inner circle of the animal, animal will flee
                    // Vector2 normal = hit.normal;
                    // AnimalFlee(normal);
                    //startFlee = true;
                    return true;
                }
            }
            return false;
        }

        //PerceptionCheck is for where the animal is currently looking
        //It will be many raycast in a cone shape, each checking to see if it sees the animal
        public bool PerceptionCheck(float distance)
        {
            Vector2 position = transform.position;
            Vector2 rotation = transform.rotation.eulerAngles;
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
                        // Vector2 normal = hit.normal;
                        // AnimalFlee(normal);
                        //startFlee = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public void AnimalDamage(int dmg)
        {
            Debug.Log(animalType + " has taken " + dmg + " damage.");
            if (animalHealth - dmg <= 0){
                //dead
                animalHealth = 0;
                setDead();
            }
            else{
                animalHealth -= dmg;
                tookDamage = true;
            }
        }

        

        void setDead(){
            isDead = true;
            gameObject.tag = "deadAnimal";
            gameObject.layer = LayerMask.NameToLayer("DeadAnimal");
            GetComponent<SpriteRenderer>().color = new Color32(121, 29, 29, 255);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            Debug.Log(animalType + " has been killed.");
            transform.position += new Vector3(0, 0, 0.1f);
        }
        // Start is called before the first frame update
        void Start()
        {
            //player = GameObject.Find("Monster");
        }

        // Update is called once per frame
        void Update()
        {
            if(isDead){
                gameObject.tag = "deadAnimal";
            }
            //placeholder variables
            // Vector2 position = new Vector2(0,0);
            // Vector2 rotation = new Vector2(0,0);
            // float distance = 5.0f;
            //AwarenessCheck(distance);
            //PerceptionCheck(distance);
        }

        private void OnBecameVisible()
        {
            if (!awake)
            {
                awake = true;
                Vector2 point = Random.insideUnitCircle.normalized;
                GetComponent<Rigidbody2D>().rotation = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;
            }
        }

    }
}
