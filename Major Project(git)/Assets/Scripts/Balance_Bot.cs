using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Balance_Bot : Agent
{
    public Transform target;
    public Movement botMovement;

    const float maxDistance = 15.0f;     // Maximum possible distance

    float angleReward;                   // Reward for angle position
    float distanceReward;                // closer to be for more reward
    float velocityDiscount;              // forces velocity to be lower if distance decreases
    float prevDistance;
    public bool targetReached = false, botFell = false, botCollided = false;
    int goalcount = 0;

    float alignmentReward;
    float matchingVelocityReward = 0f;
    const float velocityGoal = 0.8f;

    Vector3 navigationGoal = Vector3.zero;
    public bool isNavigation = false;
    public Navigation_Bot navRef;
    //float reward_sum = 0;
    #region BotInfo - Rigidbody ,Transforms
    [SerializeField]
    Transform bot, rightWheel, leftWheel;
    Rigidbody rbBot, rbRightWheel, rbLeftWheel;
    HingeJoint rightJoint, leftJoint;
    Vector3 initialPosBot, initialPosRightWheel, initialPosLeftWheel;
    Vector3 initialRotBot, initialRotRightWheel, initialRotLeftWheel;
    JointMotor resetMotor;
    #endregion
    void Start()
    {
        navRef = GetComponentInChildren<Navigation_Bot>();
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
    public override void OnEpisodeBegin()
    {
        if (isNavigation)
        {

        }
        else
        {
            Unity.MLAgents.Academy.Instance.StatsRecorder.Add("goalcount",goalcount,StatAggregationMethod.Average);
            if (targetReached)
            {
                SetRandomTarget();
                targetReached = false;
            }
            else
            {
                // when bot falls or time expires 
                EpisodeReset();
            }
            prevDistance = maxDistance;
            distanceReward = 0;
            goalcount = 0;
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (isNavigation)
        {
            sensor.AddObservation(navRef.intermediateGoal.magnitude*5);   //Distance to intermediateGoal
            sensor.AddObservation(navRef.intermediateGoal);        // Calculate the direction to incoming checkpoint
        }
        else
        {
            sensor.AddObservation(GetDistance());                                                    // Distance to Target in case of no Navigation
            sensor.AddObservation((CurrentTargetPosition() - CurrentBotPosition()).normalized);      // Calculate the direction to Target in case of no Navigation
        }
        sensor.AddObservation(GetVelocity().magnitude/3.0f);
        sensor.AddObservation(GetVelocity().normalized);                  // Current Bot Velocity in Local Space  ??
        sensor.AddObservation(GetSignedAngularVelocity() / 360.0f);       // Current angular velocity   -305 to 298
        sensor.AddObservation(GetSignedAngle() / 180.0f);                 // Pitch & Yaw Angle 
        //Debug.Log(GetVelocity().magnitude);
        if (isNavigation)
        {
            sensor.AddObservation(Vector2.Dot(navRef.intermediateGoal,new Vector2(transform.forward.x,transform.forward.z)));                            // Alignment between bot forward direction and target direction
        }
        else
        {
            sensor.AddObservation(GetAlignment());                            // Alignment between bot forward direction and target direction
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        botMovement.leftSpeed = Mathf.Clamp(vectorAction[0], -1f, 1f);
        botMovement.rightSpeed = Mathf.Clamp(vectorAction[1], -1f, 1f);
        

        if (!isNavigation)
        {
            GetRewards();
        }
    }
    void GetRewards()
    {
        float distanceError = GetDistance();
        float velocityError = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(0.0f, 3.0f, GetVelocity().magnitude)); // scaling value to 0 to 1
        float angleError = Mathf.Abs(GetSignedAngle().x);
        float angularVelocityError = Mathf.Abs(GetSignedAngularVelocity().x);
        // Reward
        if (angleError >= 45 || transform.localPosition.y < -1.0f) // Falling Condition and if bot falls from platform and also it will not far if it is not balanced
        {
            //Debug.Log("The fall");
            botFell = true;
            AddReward(-1.0f);
            EndEpisode();
        }
        // Alignment reward
        alignmentReward = GetAlignment();

        //Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 0)
        if (distanceError <= 0.5f)
        {
            goalcount+=1;
            Debug.Log("Balance");
            AddReward(1.0f); //high positive reward if it reaches goal
            SetRandomTarget();
        }

        //distanceReward = 1 - Mathf.Pow(distanceError/maxDistance,0.4f); // decreasing function

        if (prevDistance > distanceError)
        {
            distanceReward = 0.5f;
        }
        else
        {
            distanceReward = -0.5f;
        }
        AddReward(distanceReward);
        prevDistance = distanceError;
        //velocityDiscount = Mathf.Pow(1 - Mathf.Max(velocityError, 0.1f), 1 / Mathf.Max(distanceError / maxDistance, 0.2f)) / 10;
        angleReward = Mathf.Clamp(1.0f - 0.1f * angleError - 0.01f * angularVelocityError, 0.0f, 1.0f) / 10;
        matchingVelocityReward = GetMatchingVelocityReward();
        AddReward(alignmentReward * angleReward * matchingVelocityReward - 0.1f);
    }
    public void ResetBot(Vector3 startPos)
    {
        Vector3 offset = new Vector3(startPos.x, 0, startPos.z) - new Vector3(initialPosBot.x, 0, initialPosBot.z);
        ResetBotCommon(offset);
    }
    public void ResetBot()
    {
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ResetBotCommon(offset);
    }
    void ResetBotCommon(Vector3 offset)
    {
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
    public void SetRandomTarget()
    {
        float cases = (float)Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 5);
        target.localPosition = new Vector3(Random.Range(-cases, cases), -0.25f, Random.Range(-cases, cases));
    }
    public void EpisodeReset()
    {
        ResetBot();
        SetRandomTarget();
    }
    public Vector2 CurrentBotPosition()
    {
        return new Vector2(transform.localPosition.x, transform.localPosition.z);
    }
    public Vector2 CurrentTargetPosition()
    {
        return new Vector2(target.localPosition.x, target.localPosition.z);
    }
    public float GetMatchingVelocityReward()   // Forces bot to match velocity of 1.5
    {
        float velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(GetVelocity(), transform.forward * velocityGoal), 0, 1);
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / velocityGoal, 2), 2);
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
        float pitchAngle = Vector3.SignedAngle(Vector3.up, transform.up, transform.right);
        float yawAngle = Vector3.SignedAngle(Vector3.right, transform.right, transform.up);
        return new Vector2(pitchAngle, yawAngle);
    }
    public Vector2 GetSignedAngularVelocity()
    {
        Vector3 temp = transform.InverseTransformDirection(rbBot.angularVelocity);
        return Mathf.Rad2Deg * new Vector2(temp.x, temp.y);
    }
    public float GetAlignment()
    {
        Vector2 vecForward = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 vec2Direction = (CurrentTargetPosition() - CurrentBotPosition()).normalized;
        return Vector2.Dot(vec2Direction, vecForward);
    }
    public override void Heuristic(float[] actionsOut)
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        actionsOut[0] = inputY + inputX;
        actionsOut[1] = inputY - inputX;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle") || collision.collider.CompareTag("Wall"))
        {
            Debug.Log("Hit");
            botCollided = true;
        }
        if (collision.collider.CompareTag("Ground"))
        {
            botFell = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall")|| other.CompareTag("Obstacle"))
        {
            botCollided = true;
        }
    }
}