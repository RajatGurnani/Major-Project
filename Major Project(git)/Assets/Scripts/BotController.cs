using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public WheelCollider wheelColRight, wheelColLeft;
    public float motorForce, steerForce, brakeForce;
    float brakeValue;
    float inputX, inputY;

    void Update()
    {
        inputY = Input.GetAxisRaw("Vertical") * motorForce;
        inputX = Input.GetAxisRaw("Horizontal") * steerForce;
        if (Input.GetKey(KeyCode.Space))
        {
            brakeValue = brakeForce;
        }
        else
        {
            brakeValue = 0;
        }
    }
    private void FixedUpdate()
    {
        wheelColLeft.motorTorque = inputY + inputX;
        wheelColRight.motorTorque = inputY + inputX * -1;
        wheelColLeft.brakeTorque = brakeValue;
        wheelColRight.brakeTorque = brakeValue;
    }
}