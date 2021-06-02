using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotParameters : MonoBehaviour
{
    Rigidbody rbBot;
    public Rigidbody[] wheels;
    Vector3[] initialPosition;
    public Transform target;
    float cases;
    Vector3 botStartPos;
    public NavSetup navSetup;
    Vector3 currentCheckpoint;
    public bool checkpointsCompleted = false;
    public bool botCollided = false;
    public BotStatus reference;
    private void Start()
    {
        rbBot = GetComponent<Rigidbody>();
        initialPosition = new Vector3[wheels.Length];
        for (int i = 0; i < initialPosition.Length; i++)
        {
            initialPosition[i] = wheels[i].position;
        }
        botStartPos = transform.localPosition;
    }
    public Vector2 CurrentBotPosition()
    {
        return new Vector2(transform.localPosition.x, transform.localPosition.z);
    }
    public Vector2 CurrentTargetPosition()
    {
        return new Vector2(target.localPosition.x, target.localPosition.z);
    }
    public float GetDistance()
    {
        return Vector2.Distance(CurrentTargetPosition(), CurrentBotPosition());
    }
    public Vector3 GetVelocity()
    {
        return transform.InverseTransformDirection(rbBot.velocity);
    }
    public Vector2 GetSignedAngle()
    {
        return new Vector2(Vector3.SignedAngle(Vector3.up, transform.up, transform.right), 
                            Vector3.SignedAngle(Vector3.right, transform.right, transform.up));
    }
    //Vector3.right, transform.up, transform.right
    public Vector2 GetSignedAngularVelocity()
    {
        return Mathf.Rad2Deg *  new Vector2(transform.InverseTransformDirection(rbBot.angularVelocity).x,
                                            transform.InverseTransformDirection(rbBot.angularVelocity).y);
    }
    public void EpisodeReset()
    {
        Vector3 randomPositionOffset = new Vector3(Random.Range(-1f, 1f), 0,Random.Range(-1f, 1f)); 
        rbBot.angularVelocity = Vector3.zero;
        rbBot.velocity = Vector3.zero;

        foreach (Rigidbody wheelRB in wheels)
        {
            wheelRB.angularVelocity = Vector3.zero;
            wheelRB.velocity = Vector3.zero;
        }
        transform.localPosition = Vector3.zero + randomPositionOffset;
        transform.localEulerAngles = Vector3.right * Random.Range(-15f, 15f);
        for (int i = 0; i < initialPosition.Length; i++)
        {
            wheels[i].position = initialPosition[i] + randomPositionOffset;
        }
        cases = (float)Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 5);
        target.localPosition = new Vector3(Random.Range(-cases, cases), -0.25f, Random.Range(-cases, cases));
    }
    public void SetTarget()
    {
        target.localPosition = new Vector3(Random.Range(-cases, cases), -0.25f, Random.Range(-cases, cases));
    }
    public void NavigationEpisodeReset1()
    {
        botCollided = false;
        navSetup.ResetSetup();
        rbBot.angularVelocity = Vector3.zero;
        rbBot.velocity = Vector3.zero;

        foreach (Rigidbody wheelRB in wheels)
        {
            wheelRB.angularVelocity = Vector3.zero;
            wheelRB.velocity = Vector3.zero;
        }
        /*Vector3 possi = navSetup.ProvideStartPosition();
        possi = new Vector3(possi.x, botStartPos.y, possi.z);*/
        //transform.localPosition = Vector3.zero + possi;
        transform.localPosition = botStartPos;
        transform.localEulerAngles = Vector3.zero;

        for (int i = 0; i < initialPosition.Length; i++)
        {
            //wheels[i].position = (initialPosition[i] - botStartPos) + possi;
            wheels[i].position = initialPosition[i];
            //wheels[i].rotation = Quaternion.Euler(Vector3.forward * 90);
        }
        checkpointsCompleted = false;
    }
    public void NavigationEpisodeReset()
    {
        botCollided = false;
        navSetup.ResetSetup();
        reference.ResetBot();
    }

    public bool CheckpointPassed()
    {
        if (navSetup.checkPointCount == -1)
        {
            currentCheckpoint = navSetup.GetNextCheckpoint();
        }
        if (transform.position.z > currentCheckpoint.z && !checkpointsCompleted)
        {
            currentCheckpoint = navSetup.GetNextCheckpoint();
            Debug.Log("new ");
            if (navSetup.checkPointCount == navSetup.sortedCheckpoints.Length - 1)
            {
                checkpointsCompleted = true;
            }
            return true;
        }
        return false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle") || collision.collider.CompareTag("Wall"))
        {
            botCollided = true;
            Debug.Log("HIT");
        }
    }
    public void ClickCheckpoint()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
    }
}