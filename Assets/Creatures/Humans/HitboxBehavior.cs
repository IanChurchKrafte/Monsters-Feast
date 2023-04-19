using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class HitboxBehavior : MonoBehaviour
{
    //on trigger, if it comes into contact with something do damage to it
    
    public int damage = 25;
    public GameObject owner;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    
    private void OnTriggerEnter2D(Collider2D collision){
        
        MonsterBehavior monster = collision.gameObject.GetComponent<MonsterBehavior>();
        GenericHuman human = collision.gameObject.GetComponent<GenericHuman>();
        GenericAnimal animal = collision.gameObject.GetComponent<GenericAnimal>();

        if(monster != null) monster.TakeDamage(damage);
        if(human != null){
            if(collision.gameObject != owner)
                human.TakeDamage(damage);
            //check if its hitting the person that shot it
            //right now only going to check if its another crossbow users
            // if(human.humanType != GenericHuman.HumanType.crossbow)
            //     human.TakeDamage(damage);
        }
        if(animal != null) animal.AnimalDamage(damage);

        // Destory(gameObject);
        // rb.velocity = Vector2.zero;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // owner = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
