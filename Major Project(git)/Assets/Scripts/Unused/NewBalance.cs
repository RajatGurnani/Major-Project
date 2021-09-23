using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class NewBalance : Agent
{
    public Rigidbody rBody;

    public Transform target;
    public float targetSpeed;
    Vector3 intermediateTarget = Vector3.zero;
    public BotStatus botStat;
    public Movement botMovement;

    const float maxDistance = 10.0f; // Maximum possible distance
    float angleReward; // Reward for angle position
    float distanceReward; // closer to be for more reward
    float velocityDiscount; // forces velocity to be lower if distance decreases
    float prevDistance;

    public NewSetup newSetupRef;
    public event System.Action ResetEpisode;       // Event to notify scripts of bot reset
    public override void Initialize()
    {
        intermediateTarget = newSetupRef.GetNewTarget();
    }
    public override void OnEpisodeBegin()
    {
        if (ResetEpisode != null)
        {
            ResetEpisode();
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(botStat.CurrentBotPosition());                              // Current Bot Direction only x and z
        sensor.AddObservation(botStat.CurrentTargetPosition(intermediateTarget));                           // Current Target Direction only x and z
        sensor.AddObservation(targetSpeed);                                                  // Target speed --Not used

        sensor.AddObservation(botStat.GetVelocity());                                     // Current velocity  
        sensor.AddObservation(botStat.GetSignedAngularVelocity());                        // Current angular velocity 
        sensor.AddObservation(botStat.GetSignedPitchAngle());                             // Pitch Angle
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        botMovement.leftSpeed = Mathf.Clamp(vectorAction[0], -0.5f, 1f);
        botMovement.rightSpeed = Mathf.Clamp(vectorAction[1], -0.5f, 1f);

        float distanceError = Vector2.Distance(botStat.CurrentTargetPosition(intermediateTarget), botStat.CurrentBotPosition());
        float velocityError = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(0.0f, 3.0f, botStat.GetVelocity().magnitude)); // scaling value to 0 to 1
        float angleError = Mathf.Abs(botStat.GetSignedPitchAngle());
        float angularVelocityError = Mathf.Abs(botStat.GetSignedAngularVelocity());
        //Debug.Log(velocityError);
        // Reward
        if (angleError >= 30 || transform.localPosition.y < -1.0f) // Falling Condition and if bot falls from platform
        {
            AddReward(-100000.0f);
            EndEpisode();
        }
        //Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 0)
        if (distanceError <= 0.5f )
        {
            if (newSetupRef.allTargetsReached)
            {
                AddReward(100000.0f); //high positive reward if it reaches goal
                EndEpisode();

            }
            else
            {
                AddReward(1000.0f);
            }
            
        }

        //distanceReward = 1 - Mathf.Pow(distanceError/maxDistance,0.4f); // decreasing function
        if (distanceError < prevDistance)
        {
            distanceReward += 0.01f;
            prevDistance = distanceError;
        }
        else
        {
            distanceReward -= 0.01f;
            prevDistance = distanceError;
        }

        //Debug.Log(distanceError);
        velocityDiscount = Mathf.Pow(1 - Mathf.Max(velocityError, 0.1f), 1 / Mathf.Max(distanceError / maxDistance, 0.2f));
        angleReward = Mathf.Clamp(1.0f - 0.1f * angleError - 0.01f * angularVelocityError, 0.0f, 2.0f);
        Debug.Log(1.0f - 0.1f * angleError - 0.01f * angularVelocityError);
        AddReward(distanceReward * angleReward * velocityDiscount - 0.01f);
    }

    public override void Heuristic(float[] actionsOut)
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        actionsOut[0] = inputX;
        actionsOut[1] = inputY;
    }
    void ForBalanceReset()
    {
        newSetupRef.RandomizeTargetPositions();
        intermediateTarget = newSetupRef.GetNewTarget();
    }
}