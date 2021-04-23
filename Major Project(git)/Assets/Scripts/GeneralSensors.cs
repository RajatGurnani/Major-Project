using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSensors : MonoBehaviour
{
    [Header("Sensor Information")]
    [Tooltip("No. of sensors to be used")]
    public int sensorCount = 3;
    [Tooltip("Maximum Detection Distance")]
    public float sensorSensingDistance = 5;
    [Range(-1f, 1f)]
    public float sensorHeightAboveGround;
    [Tooltip("Seperation Angle between two sensors")]
    public float sensorSpacingAngle = 30;

    public bool showRays=false;

    void Update()
    {
        SensorExection();
    }
    void SensorExection()
    {
        float sweepAngle = (sensorCount-1) * sensorSpacingAngle;
        for (int i = 0; i < sensorCount; i++)
        {
            Ray ray = new Ray(transform.position + transform.up * sensorHeightAboveGround, Quaternion.AngleAxis((-sweepAngle / 2) + i * sensorSpacingAngle, transform.up) * transform.forward);
            if (showRays)
            {
                if (Physics.Raycast(ray, out RaycastHit hitInfo, sensorSensingDistance))
                {
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * sensorSensingDistance, Color.red);
                }
                else
                {
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * sensorSensingDistance, Color.green);
                }
            }
        }
    }
}