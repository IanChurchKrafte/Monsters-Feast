using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Camera cameraComp;
    public GameObject focus;
    public Vector2 mapHalfSize = new Vector2(50, 50);
    // Start is called before the first frame update
    void Start()
    {
        cameraComp = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!focus) { return; }
        Vector3 newPos = focus.transform.position;
        if (newPos.x - cameraComp.orthographicSize * cameraComp.aspect < -mapHalfSize.x)
            newPos = new Vector3(-mapHalfSize.x + cameraComp.orthographicSize * cameraComp.aspect, newPos.y);
        if (newPos.x + cameraComp.orthographicSize * cameraComp.aspect > mapHalfSize.x)
            newPos = new Vector3(mapHalfSize.x - cameraComp.orthographicSize * cameraComp.aspect, newPos.y);
        if (newPos.y - cameraComp.orthographicSize < -mapHalfSize.y)
            newPos = new Vector3(newPos.x, -mapHalfSize.y + cameraComp.orthographicSize);
        if (newPos.y + cameraComp.orthographicSize > mapHalfSize.y)
            newPos = new Vector3(newPos.x, mapHalfSize.y - cameraComp.orthographicSize);
        newPos = new Vector3(newPos.x, newPos.y, cameraComp.transform.position.z);
        cameraComp.transform.position = newPos;
    }
}
