using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody rbBot;
    public HingeJoint hjRight, hjLeft;
    public float maxMoveSpeed;
    public float rightSpeed = 0, leftSpeed = 0;
    public void MoveMotors()
    {
        JointMotor leftMotor = hjLeft.motor;
        JointMotor rightMotor = hjRight.motor;
        leftMotor.targetVelocity = leftSpeed * maxMoveSpeed;
        rightMotor.targetVelocity = rightSpeed * maxMoveSpeed;
        hjLeft.motor = leftMotor;
        hjRight.motor = rightMotor;
    }
    /*private void Update()
    {
        rightSpeed = Input.GetAxisRaw("Horizontal");
        leftSpeed = Input.GetAxisRaw("Vertical");
    }*/
    private void FixedUpdate()
    {
        MoveMotors();
    }
}