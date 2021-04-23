using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotParameters : MonoBehaviour
{
    Rigidbody rbBot;
    public Rigidbody[] wheels;
    Vector3[] initialPosition;

    private void Start()
    {
        rbBot = GetComponent<Rigidbody>();
        initialPosition = new Vector3[wheels.Length];
        for (int i = 0; i < initialPosition.Length; i++)
        {
            initialPosition[i] = wheels[i].position;
        }
    }
    public Vector3 GetVelocity()
    {
        return transform.InverseTransformDirection(rbBot.velocity);
    }
    /// <returns>Signed Pitch Angle</returns>
    public float GetSignedPitchAngle()
    {
        return Vector3.SignedAngle(Vector3.up, transform.up, transform.right);
    }
    /// <returns>Signed Angular Velocity along X-axis</returns>
    public float GetSignedAngularVelocity()
    {
        return transform.InverseTransformDirection(rbBot.angularVelocity).x;
    }
    public void EpisodeReset()
    {
        rbBot.angularVelocity = Vector3.zero;
        rbBot.velocity = Vector3.zero;

        foreach (Rigidbody wheelRB in wheels)
        {
            wheelRB.angularVelocity = Vector3.zero;
            wheelRB.velocity = Vector3.zero;
        }
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        for (int i = 0; i < initialPosition.Length; i++)
        {
            wheels[i].position = initialPosition[i];
        }
    }
}