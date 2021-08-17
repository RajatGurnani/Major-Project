using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    Vector3 initialPos;
    Vector3 endPos;
    public float speed=1;
    void Start()
    {
        initialPos = transform.localPosition;
        endPos = new Vector3(-initialPos.x,initialPos.y,initialPos.z);
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(initialPos,endPos,Mathf.PingPong(Time.time*speed,1));
    }
}
