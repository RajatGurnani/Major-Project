using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotStatus : MonoBehaviour
{
    [SerializeField]
    Transform bot, rightWheel, leftWheel;
    Rigidbody rbBot, rbRightWheel, rbLeftWheel;
    HingeJoint rightJoint, leftJoint;
    Vector3 initialPosBot, initialPosRightWheel, initialPosLeftWheel;
    Vector3 initialRotBot, initialRotRightWheel, initialRotLeftWheel;
    JointMotor resetMotor;
    void Start()
    {
       // GameObject.FindGameObjectWithTag("Player").GetComponent<NewBalance>().ResetEpisode += ResetBot;

        rbBot = bot.GetComponent<Rigidbody>();
        rbRightWheel = rightWheel.GetComponent<Rigidbody>();
        rbLeftWheel = leftWheel.GetComponent<Rigidbody>();

        rightJoint = rightWheel.GetComponent<HingeJoint>();
        leftJoint = leftWheel.GetComponent<HingeJoint>();

        initialPosBot = bot.localPosition;
        initialPosRightWheel = rightWheel.localPosition;
        initialPosLeftWheel = leftWheel.localPosition;

        initialRotBot = bot.localEulerAngles;
        initialRotRightWheel = rightWheel.localEulerAngles;
        initialRotLeftWheel = leftWheel.localEulerAngles;

        resetMotor.force = 5000;
        resetMotor.targetVelocity = 0;
    }
    public void ResetBot()
    {
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        rbBot.velocity = Vector3.zero;
        rbRightWheel.velocity = Vector3.zero;
        rbLeftWheel.velocity = Vector3.zero;

        rbBot.angularVelocity = Vector3.zero;
        rbRightWheel.angularVelocity = Vector3.zero;
        rbLeftWheel.angularVelocity = Vector3.zero;

        bot.localEulerAngles = initialRotBot;
        rightWheel.localEulerAngles = initialRotRightWheel;
        leftWheel.localEulerAngles = initialRotLeftWheel;

        bot.localPosition = initialPosBot + offset;
        rightWheel.localPosition = initialPosRightWheel + offset;
        leftWheel.localPosition = initialPosLeftWheel + offset;

        rightJoint.motor = resetMotor;
        leftJoint.motor = resetMotor;
    }
    public Vector2 CurrentBotPosition()
    {
        return new Vector2(transform.localPosition.x, transform.localPosition.z);
    }
    public Vector2 CurrentTargetPosition(Vector3 target)
    {
        return new Vector2(target.x,target.z);
    }
    public Vector3 GetVelocity()
    {
        return transform.InverseTransformDirection(rbBot.velocity);
    }
    public float GetSignedPitchAngle()
    {
        return Vector3.SignedAngle(Vector3.up, transform.up, transform.right);
    }
    public float GetSignedAngularVelocity()
    {
        return Mathf.Rad2Deg * transform.InverseTransformDirection(rbBot.angularVelocity).x;
    }
}