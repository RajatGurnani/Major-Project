using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple : MonoBehaviour
{
    public Rigidbody rbRight, rbLeft;
    public float moveForce, steerForce, breakForce;
    float inputX, inputY;
    public HingeJoint hjRight, hjLeft;
    bool slowMo = false;
    private void Start()
    {
        slowMo = false;
        //GetComponent<Rigidbody>().maxAngularVelocity = 0;
    }
    void Update()
    {
        UserInput();
    }
    private void FixedUpdate()
    {
        JointMotor leftMotor = hjLeft.motor;
        JointMotor rightMotor = hjRight.motor;
        leftMotor.targetVelocity = inputY + (inputX * 1);
        rightMotor.targetVelocity = inputY + (inputX * -1);
        hjLeft.motor = leftMotor;
        hjRight.motor = rightMotor;
    }
    void UserInput()
    {
        inputX = Input.GetAxisRaw("Horizontal") * steerForce;
        inputY = Input.GetAxisRaw("Vertical") * moveForce;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            slowMo = !slowMo;
        }
        if (slowMo)
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}