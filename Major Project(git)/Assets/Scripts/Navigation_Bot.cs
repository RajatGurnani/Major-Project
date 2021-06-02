using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Navigation_Bot : Agent
{
    [SerializeField] Transform endPoint, startPoint;
    public Transform[] obstacles;
    Balance_Bot balanceRef;

    public Vector2 intermediateGoal;
    Vector3 targetInitialPos, startInitialPos;

    public bool seeIntermediatePoint = false;
    public Vector2 obstacleScaleMinMax = new Vector2(5f, 11f);
    float previousDistance;

    private void Start()
    {
        balanceRef = transform.parent.GetComponent<Balance_Bot>();

        targetInitialPos = endPoint.localPosition;
        startInitialPos = startPoint.localPosition;
        intermediateGoal = new Vector2(startInitialPos.x, startInitialPos.z);
    }
    public override void OnEpisodeBegin()
    {
        if (balanceRef.isNavigation)
        {
            balanceRef.botFell = false;
            balanceRef.botCollided = false;
            ResetNavigationEpisode();
            previousDistance = GetDistance();
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((intermediateGoal - CurrentBotPosition()).normalized); //TBD          // Normalized Direction towards IntermediateGoal
        sensor.AddObservation((EndPointPosition() - CurrentBotPosition()).normalized);        // Normalized Direction towards Target
        sensor.AddObservation(EndPointPosition());                                            // Target Local Position
        sensor.AddObservation(CurrentBotPosition());                                          // Balance bot Local position
        sensor.AddObservation(GetDistance());                                                 // Distance between Bot and Target
        sensor.AddObservation(balanceRef.GetVelocity().normalized);
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        intermediateGoal = new Vector2(vectorAction[0], vectorAction[1]).normalized;          // Next Intermediate Goal which will go to Balance model

        if (GetDistance() <= 0.5f)       // Reward for successfully reaching Target
        {
            Debug.Log("Navigation");     // Reaching Endpoint
            AddReward(1f);
            EndEpisode();
        }

        if (balanceRef.botFell)          // Falling Penalty =-1
        {
            Debug.Log("Fall");
            EndEpisode();
        }

        if (balanceRef.botCollided)      // Obstacle/Wall hit Penalty= -0.1
        {
            Debug.Log("Collided");
            AddReward(-0.1f);
            EndEpisode();
        }
        // checkpoint on wall cross/empty gameobject with trigger collider

        AddReward(0.1f * (previousDistance - GetDistance()));           // increment on z axis
        previousDistance = GetDistance();
    }
    void ResetNavigationEpisode()
    {
        ResetStartPosition();
        ResetEndPosition();
        balanceRef.ResetBot(startPoint.localPosition);
        ResetObstacles();
    }
    void ResetStartPosition()
    {
        startPoint.localPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-1f, 1f)) + startInitialPos;
    }
    void ResetEndPosition()
    {
        endPoint.localPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-1f, 1f)) + targetInitialPos;
    }
    void ResetObstacles()
    {
        foreach (Transform obs in obstacles)
        {
            int rand = Random.Range(0, 2) == 0 ? -1 : 1;
            float xScale = Random.Range(obstacleScaleMinMax.x, obstacleScaleMinMax.y);
            obs.localPosition = new Vector3(obs.localPosition.x * rand, obs.localPosition.y, obs.localPosition.z);
            if (obs.localPosition.x > 0)
            {
                obs.localEulerAngles = Vector3.up * 180;
            }
            else
            {
                obs.localEulerAngles = Vector3.zero;
            }
            obs.localScale = new Vector3(xScale, obs.localScale.y, obs.localScale.z);
        }
    }
    public Vector2 CurrentBotPosition()
    {
        return new Vector2(transform.parent.localPosition.x, transform.parent.localPosition.z);
    }
    public Vector2 EndPointPosition()
    {
        return new Vector2(endPoint.localPosition.x, endPoint.localPosition.z);
    }
    public float GetDistance()
    {
        return Vector2.Distance(EndPointPosition(), CurrentBotPosition());
    }
    void DrawIntermediatePoint()
    {
        Debug.DrawLine(transform.position, intermediateGoal, Color.red);
    }
    public override void Heuristic(float[] actionsOut)
    {
        Vector2 mousePosition=Input.mousePosition;
        Vector3 mouseWorldPosition= Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 temp = transform.parent.parent.InverseTransformDirection(mouseWorldPosition - transform.position);
        //Debug.Log(mouseWorldPosition);
        Debug.DrawRay(transform.position,temp,Color.red);
        actionsOut[0] = temp.x;
        actionsOut[1] = temp.z;
    }
}