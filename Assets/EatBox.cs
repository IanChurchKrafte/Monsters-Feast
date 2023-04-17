using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatBox : MonoBehaviour
{
    public GameObject animal;
    private float timeToEat;
    private float eatingTimer = 0f;
    public static bool canEat = false;

    // * Check which animal to eat after death * //
    // * Time it takes for Monster to eat animal depends on calorie count * //
    private void OnTriggerEnter2D(Collider2D other)
    {
        // * DeerGoose * //
        if (other.gameObject.GetComponent<DeerGoose>())
        {
            animal = other.gameObject;
            timeToEat = other.gameObject.GetComponent<DeerGoose>().calories * 15;
        }
        // * ChameleToad * //
        else if (other.gameObject.GetComponent<ChameleToad>())
        {
            animal = other.gameObject;
            timeToEat = other.gameObject.GetComponent<ChameleToad>().calories * 15;
        }
        canEat = true;
    }

    void Update()
    {
        if (canEat)
        {
            // * Monster should remain stationary while eating another animal * //
            Debug.Log("Munch Munch Munch Chew Chew Chew");
            eatingTimer += Time.deltaTime;
            if (eatingTimer >= timeToEat)
            {
                MonsterBehavior.eatBox.SetActive(false);
                eatingTimer = 0;
                Destroy(animal);
                Debug.Log("Swallow");
                canEat = false;
            }
        }

    }

    // * Eat other animals * //
    public static void Eat()
    {
        canEat = true;
        MonsterBehavior.eatBox.SetActive(true);
    }
}