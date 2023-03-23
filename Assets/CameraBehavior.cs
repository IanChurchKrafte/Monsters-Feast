using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Camera camera;
    public GameObject focus;
    public Vector2 mapHalfSize = new Vector2(50, 50);
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!focus) { return; }
        Vector3 newPos = focus.transform.position;
        if (newPos.x - camera.orthographicSize * camera.aspect < -mapHalfSize.x)
            newPos = new Vector3(-mapHalfSize.x + camera.orthographicSize * camera.aspect, newPos.y);
        if (newPos.x + camera.orthographicSize * camera.aspect > mapHalfSize.x)
            newPos = new Vector3(mapHalfSize.x - camera.orthographicSize * camera.aspect, newPos.y);
        if (newPos.y - camera.orthographicSize < -mapHalfSize.y)
            newPos = new Vector3(newPos.x, -mapHalfSize.y + camera.orthographicSize);
        if (newPos.y + camera.orthographicSize > mapHalfSize.y)
            newPos = new Vector3(newPos.x, mapHalfSize.y - camera.orthographicSize);
        newPos = new Vector3(newPos.x, newPos.y, camera.transform.position.z);
        camera.transform.position = newPos;
    }
}
