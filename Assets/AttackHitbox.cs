using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animal;

public class AttackHitbox : MonoBehaviour
{
    public static int damage = MonsterBehavior.attackDMG;       // from default bite
    public static int dmg_scale;                                // from lunge attack
    public static bool attacking;
    public static bool lunging;

    // TRIGGERS WHEN MONSTER TOUCHES ENEMY
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Animal"))
        {   
             // * Monster bites Deergoose (only when it's ALIVE) * //
            if (other.gameObject.GetComponent<DeerGoose>().isDead == false)
            {
                if (lunging) other.gameObject.GetComponent<DeerGoose>().animalHealth -= dmg_scale;      // lunge is active
                else other.gameObject.GetComponent<DeerGoose>().animalHealth -= damage;                 // else, do standard attack only

                // * Check if Deergoose dies after next attack * //
                if (other.gameObject.GetComponent<DeerGoose>().animalHealth <= 0)
                {
                    other.gameObject.GetComponent<DeerGoose>().isDead = true;
                    MonsterBehavior.eatBox.SetActive(true);
                    EatBox.Eat();
                }
            }

            // * Monster bites ChameleToad * //
            if (other.gameObject.GetComponent<ChameleToad>())
            {
                if (other.gameObject.GetComponent<ChameleToad>().animalHealth <= 0)
                {
                    other.gameObject.GetComponent<ChameleToad>().isDead = true;
                }
                else
                {
                    other.gameObject.GetComponent<ChameleToad>().animalHealth -= damage;
                }
            }
        }
    }
}