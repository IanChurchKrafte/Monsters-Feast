using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBeahvior : MonoBehaviour
{
    public int health; // monster health
    public float stamina; // monster stamina
    public float speed = 0.025f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // basic movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 velocity = new Vector2 (h,v);
        velocity.Normalize();
        gameObject.transform.position += new Vector3(velocity.x, velocity.y,0) * speed;
    }
    // create 2DVector
}
