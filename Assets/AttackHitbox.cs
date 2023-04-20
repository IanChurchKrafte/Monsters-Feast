using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class AttackHitbox : MonoBehaviour
{
    public GameObject animal;
    public static int damage = MonsterBehavior.attackDMG;       // from default bite
    public static int dmg_scale;                                // from lunge attack
    public static bool attacking;
    public static bool lunging;
    public GameObject blood;

    // TRIGGERS WHEN MONSTER TOUCHES ENEMY
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("deadAnimal"))
        {
            MonsterBehavior.eatBox.SetActive(true);
            EatBox.Eat();
        }
        if (other.gameObject.CompareTag("Animal"))
        {
            GameObject spray = Instantiate(blood, transform.position - transform.up * 0.5f - Vector3.forward, new Quaternion());
            if (lunging)
            {
                if (other.gameObject.GetComponent<GenericAnimal>() != null)
                    other.gameObject.GetComponent<GenericAnimal>().AnimalDamage(dmg_scale);
                if (other.gameObject.GetComponent<GenericHuman>() != null)
                    other.gameObject.GetComponent<GenericHuman>().TakeDamage(dmg_scale);
                transform.parent.gameObject.GetComponent<MonsterBehavior>().lungeTimer = 0;
                transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                spray.transform.localScale = Vector2.one * (1+ (dmg_scale - 20) / 40);
            }
            else
            {
                if(other.gameObject.GetComponent<GenericAnimal>() != null)
                    other.gameObject.GetComponent<GenericAnimal>().AnimalDamage(damage);
                if (other.gameObject.GetComponent<GenericHuman>() != null)
                    other.gameObject.GetComponent<GenericHuman>().TakeDamage(damage);
            }
            if (other.gameObject.GetComponent<GenericAnimal>().animalHealth <= 0)
            {
                MonsterBehavior.eatBox.SetActive(true);
                EatBox.Eat();
            }
            gameObject.SetActive(false);
            /*
             // * Monster makes contact with DeerGoose * //
            if (other.gameObject.GetComponent<DeerGoose>())
            {
                // * ONLY bite DeerGoose when it is ALIVE * //
                if (other.gameObject.GetComponent<DeerGoose>().isDead == false)
                {
                    if (lunging) other.gameObject.GetComponent<DeerGoose>().animalHealth -= dmg_scale;      // lunge is active
                    else other.gameObject.GetComponent<DeerGoose>().animalHealth -= damage;                 // else, do standard attack only

                    // * Check if DeerGoose dies after next attack * //
                    if (other.gameObject.GetComponent<DeerGoose>().animalHealth <= 0)
                    {
                        other.gameObject.GetComponent<DeerGoose>().setDead();
                        Debug.Log("Deergoose is dead");
                    }
                }
                else
                {
                    MonsterBehavior.eatBox.SetActive(true);
                }
            }
            // * Monster makes contact with ChameleToad * //
            else if (other.gameObject.GetComponent<ChameleToad>())
            {
                // * ONLY bite ChameleToad when it is ALIVE * //
                if (other.gameObject.GetComponent<ChameleToad>().isDead == false)
                {
                    if (lunging) other.gameObject.GetComponent<ChameleToad>().animalHealth -= dmg_scale;      // lunge is active
                    else other.gameObject.GetComponent<ChameleToad>().animalHealth -= damage;                 // else, do standard attack only

                    // * Check if ChameleToad dies after next attack * //
                    if (other.gameObject.GetComponent<ChameleToad>().animalHealth <= 0)
                    {
                        other.gameObject.GetComponent<ChameleToad>().setDead();
                        Debug.Log("ChameleToad is dead");
                    }
                }
                else
                {
                    MonsterBehavior.eatBox.SetActive(true);
                }
            }
            // * Monster makes contact with Sangoat * //
            else if (other.gameObject.GetComponent<GoatBehavior>())
            {
                // * ONLY bite ChameleToad when it is ALIVE * //
                if (other.gameObject.GetComponent<GoatBehavior>().isDead == false)
                {
                    if (lunging) other.gameObject.GetComponent<GoatBehavior>().animalHealth -= dmg_scale;      // lunge is active
                    else other.gameObject.GetComponent<GoatBehavior>().animalHealth -= damage;                 // else, do standard attack only

                    // * Check if ChameleToad dies after next attack * //
                    if (other.gameObject.GetComponent<GoatBehavior>().animalHealth <= 0)
                    {
                        other.gameObject.GetComponent<GoatBehavior>().setDead();
                        Debug.Log("ChameleToad is dead");
                    }
                }
                else
                {
                    MonsterBehavior.eatBox.SetActive(true);
                }
            }
            // * Waiting on other animals * //
            */
        }
    }
}