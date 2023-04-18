using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TailStatus
{
    Idle,
    LeftTurn,
    RightTurn
}
public class TailSegBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    public TailStatus status = TailStatus.Idle;
    public float speed = 1, acc = 2, rotPoint = 0, maxAngle = 15;
    public float wait, maxWait = 0;

    void Start()
    {
        wait = maxWait;
    }

    // Update is called once per frame
    void Update()
    {   
        if(wait > 0)
        {
            wait -= maxAngle * Mathf.Abs(speed) * Time.deltaTime;
        }
        else
        {
            if (rotPoint < -maxAngle && speed < 0)
            {
                rotPoint = -maxAngle;
                speed *= -1;
            }
            else if (rotPoint > maxAngle && speed > 0)
            {
                rotPoint = maxAngle;
                speed *= -1;
            }
            rotPoint = rotPoint + maxAngle * speed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotPoint));
        }
        
    }
}
