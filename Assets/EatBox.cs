using Animal;
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
        if (other.gameObject.tag != "deadAnimal")
            return;
        if (other.gameObject.GetComponent<GenericAnimal>() || other.gameObject.GetComponent<GenericHuman>())
        {
            animal = other.gameObject;
            //timeToEat = other.gameObject.GetComponent<GenericAnimal>().calories * 100 / 15;
        }
        canEat = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == animal)
        {
            canEat = false;
            //MonsterBehavior.eatBox.SetActive(false);
        }
    }

    void Update()
    {
        //Debug.Log("Button pressed: " + Input.GetKey(KeyCode.JoystickButton2));
        if (canEat && (Input.GetKey(KeyCode.JoystickButton2) || Input.GetKey(KeyCode.Space)))
        {
            // * Monster should remain stationary while eating another animal * //
            Debug.Log("Munch Munch Munch Chew Chew Chew");
            //eatingTimer += Time.deltaTime;
            if (animal.gameObject.GetComponent<GenericAnimal>())
            {
                if (animal.GetComponent<GenericAnimal>().consumed + Time.deltaTime / 15 >= animal.GetComponent<GenericAnimal>().calories)
                {
                    transform.parent.gameObject.GetComponent<MonsterBehavior>().sustenance += animal.GetComponent<GenericAnimal>().calories - animal.GetComponent<GenericAnimal>().consumed;
                    //MonsterBehavior.eatBox.SetActive(false);
                    //eatingTimer = 0;
                    Destroy(animal);
                    Debug.Log("Swallow");
                    canEat = false;
                }
                else
                {
                    float amount = Time.deltaTime / 15;
                    animal.GetComponent<GenericAnimal>().consumed += amount;
                    transform.parent.gameObject.GetComponent<MonsterBehavior>().sustenance += amount;
                }
            }
            if (animal.gameObject.GetComponent<GenericHuman>())
            {
                if (animal.GetComponent<GenericHuman>().consumed + Time.deltaTime / 15 >= animal.GetComponent<GenericHuman>().calories)
                {
                    transform.parent.gameObject.GetComponent<MonsterBehavior>().sustenance += animal.GetComponent<GenericHuman>().calories - animal.GetComponent<GenericHuman>().consumed;
                    //MonsterBehavior.eatBox.SetActive(false);
                    //eatingTimer = 0;
                    Destroy(animal);
                    Debug.Log("Swallow");
                    canEat = false;
                }
                else
                {
                    float amount = Time.deltaTime / 15;
                    animal.GetComponent<GenericHuman>().consumed += amount;
                    transform.parent.gameObject.GetComponent<MonsterBehavior>().sustenance += amount;
                }
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