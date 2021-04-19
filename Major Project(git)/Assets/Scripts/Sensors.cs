using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensors : MonoBehaviour
{
    public Transform leftSensor,middleSensor,rightSensor;
    public bool gizmoEnable;
    private void Update()
    {
        Ray sensorLeft = new Ray(leftSensor.position, leftSensor.forward);
        Ray sensorCenter = new Ray(middleSensor.position, middleSensor.forward);
        Ray sensorRight=new Ray(rightSensor.position, rightSensor.forward);
        RaycastHit hitInfoLeft, hitInfoCenter, hitInfoRight;

        if (Physics.Raycast(sensorLeft,out hitInfoLeft, 5f))
        {
            Debug.DrawRay(leftSensor.position,leftSensor.forward*5f,Color.red);
        }
        else
        {
            Debug.DrawRay(leftSensor.position, leftSensor.forward * 5f, Color.green);
        }

        if (Physics.Raycast(sensorCenter,out hitInfoCenter, 5f))
        {
            Debug.DrawRay(middleSensor.position, middleSensor.forward * 5f, Color.red);
        }
        else
        {
            Debug.DrawRay(middleSensor.position, middleSensor.forward * 5f, Color.green);
        }

        if (Physics.Raycast(sensorRight,out hitInfoRight ,5f))
        {
            Debug.DrawRay(rightSensor.position, rightSensor.forward * 5f, Color.red);
        }
        else
        {
            Debug.DrawRay(rightSensor.position, rightSensor.forward * 5f, Color.green);
        }
    }
    private void OnDrawGizmos()
    {
        if (gizmoEnable)
        {
            Transform[] sensorPos = { leftSensor, middleSensor, rightSensor };
            foreach (Transform pos in sensorPos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pos.position, 0.03f);
                Gizmos.DrawRay(pos.position, pos.forward * 10f);
            }
        }
    }
}