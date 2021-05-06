using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotParameters : MonoBehaviour
{
    Rigidbody rbBot;
    public Rigidbody[] wheels;
    Vector3[] initialPosition;
    public Transform targetTransform;

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
        return Mathf.Rad2Deg * transform.InverseTransformDirection(rbBot.angularVelocity).x;
    }
    public void EpisodeReset()
    {
        Vector3 randomPositionOffset = new Vector3(1, 0, 1) * Random.Range(-1f, 1f);
        rbBot.angularVelocity = Vector3.zero;
        rbBot.velocity = Vector3.zero;

        foreach (Rigidbody wheelRB in wheels)
        {
            wheelRB.angularVelocity = Vector3.zero;
            wheelRB.velocity = Vector3.zero;
        }
        transform.localPosition = Vector3.zero + randomPositionOffset;
        //transform.localEulerAngles = Vector3.zero;
        transform.localEulerAngles = Vector3.right * Random.Range(-15f, 15f);
        for (int i = 0; i < initialPosition.Length; i++)
        {
            wheels[i].position = initialPosition[i] + randomPositionOffset;
        }
        targetTransform.localPosition = new Vector3(Random.Range(-5f, 5f), -0.25f, Random.Range(-5f, 5f));
    }
}