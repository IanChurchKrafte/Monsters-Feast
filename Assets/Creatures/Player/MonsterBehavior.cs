using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{
    public int health;                           // monster health
    public float stamina;                        // monster stamina
    public float speed = 0.025f;                 // base speed
    public float acceleration = 3.5f;            // sprint factor
    public float pounce = 0.75f;            // time limit for pounce
    
    // POUNCE TIMER: 6 second limit

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // monster movement - WASD or arrow keys
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 velocity = new Vector2 (h,v);
        velocity.Normalize();
        Vector3 movement = new Vector3(velocity.x, velocity.y, 0) * speed;

        // SPRINT - Left Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement *= acceleration;
        }

        // STEALTH - Left Control
        if (Input.GetKey(KeyCode.LeftControl))
        {
            movement /= 4.0f;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                Debug.Log("pounce timer activated");
                if (pounce > 0)
                {
                    pounce -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Released");
                    pounce = 0.75f;
                }
            }
        }
        gameObject.transform.position += movement;
    }

}
