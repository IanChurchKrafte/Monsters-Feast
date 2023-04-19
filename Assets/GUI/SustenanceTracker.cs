using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SustenanceTracker : MonoBehaviour
{
    MonsterBehavior toTrack;
    float startY;
    float zero;
    float yPos;
    float maxDistance;
    // Start is called before the first frame update
    void Start()
    {
        toTrack= GameObject.Find("Monster").GetComponent<MonsterBehavior>();
        startY = GetComponent<RectTransform>().localPosition.y;
        zero = GetComponent<RectTransform>().localPosition.y - GetComponent<RectTransform>().rect.height;
        yPos = zero;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().localPosition = new Vector3(0, Mathf.Lerp(zero, startY, toTrack.sustenance), 0);
        transform.GetChild(0).gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -Mathf.Lerp(zero, startY, toTrack.sustenance), 0);
    }
}
