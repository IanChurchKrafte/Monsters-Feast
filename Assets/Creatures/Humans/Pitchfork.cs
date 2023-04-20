using Animal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitchfork : MonoBehaviour
{
    public int damage = 10;
    public GameObject owner, blood;
    private Rigidbody2D rb;
    // Start is called before the first frame update

    private void OnTriggerStay2D(Collider2D collision)
    {

        MonsterBehavior monster = collision.gameObject.GetComponent<MonsterBehavior>();
        GenericHuman human = collision.gameObject.GetComponent<GenericHuman>();
        GenericAnimal animal = collision.gameObject.GetComponent<GenericAnimal>();

        if (monster != null)
        {
            GameObject spray = Instantiate(blood, transform.position - Vector3.forward, new Quaternion());
            monster.TakeDamage(damage);
            gameObject.SetActive(false);
        }
        if (human != null)
        {
            if (collision.gameObject != owner)
            {
                GameObject spray = Instantiate(blood, transform.position - Vector3.forward, new Quaternion());
                human.TakeDamage(damage);
                gameObject.SetActive(false);
            }
        }
        if (animal != null)
        {
            GameObject spray = Instantiate(blood, transform.position - Vector3.forward, new Quaternion());
            animal.AnimalDamage(damage);
            gameObject.SetActive(false);
        }

        }
    // Start is called before the first frame update
    void Start()
    {
        owner = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
