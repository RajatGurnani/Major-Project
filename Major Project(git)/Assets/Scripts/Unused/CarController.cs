using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider wheelCol_FR, wheelCol_FL, wheelCol_RR, wheelCol_RL;
    public float moveForce, steerForce, breakForce;
    float inputX, inputY;
    float breakValue;

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal") * steerForce;
        inputY = Input.GetAxisRaw("Vertical") * moveForce;
        if (Input.GetKey(KeyCode.Space))
        {
            breakValue = breakForce;
        }
        else
        {
            breakValue = 0;
        }
        if (wheelCol_RR.GetGroundHit(out WheelHit hitInfo))
        {
            if (hitInfo.collider.name == "ramp")
            {
                Debug.Log("coll");
            }
        }
    }
    private void FixedUpdate()
    {
        wheelCol_FL.steerAngle = inputX;
        wheelCol_FR.steerAngle = inputX;
        wheelCol_RL.motorTorque = inputY;
        wheelCol_RR.motorTorque = inputY;
        wheelCol_RL.brakeTorque = breakValue;
        wheelCol_RR.brakeTorque = breakValue;
    }
}
