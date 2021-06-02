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
    bool targetReached = false;
    float matchingVelocityReward = 0f;
    float cases;
    float alignmentReward;
    //float reward_sum = 0;
    const float velocityGoal = 0.8f;  

    [SerializeField]
    Transform bot, rightWheel, leftWheel;
    Rigidbody rbBot, rbRightWheel, rbLeftWheel;
    HingeJoint rightJoint, leftJoint;
    Vector3 initialPosBot, initialPosRightWheel, initialPosLeftWheel;
    Vector3 initialRotBot, initialRotRightWheel, initialRotLeftWheel;
    JointMotor resetMotor;

    void Start()
    {
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
        if (targetReached)
        {
            SetTarget();
            targetReached = false;
        }                                                         
        else
        {
            // Reset the scene
            EpisodeReset();
        }
        prevDistance = maxDistance;
        distanceReward = 0;
        //reward_sum = 0;
    }
    public override void CollectObservations(VectorSensor sensor)
    {

        /******* THIS IS THE FINAL SCRIPT

       MAKE CHANGES IN THIS SCRIPT ONLY

       AND DO THE TRAINING ON IMPROVED BALANCE SCENE  *****/

        // Distance to waypoint
        sensor.AddObservation(GetDistance());                         
        // Calculate the direction to incoming checkpoint
        sensor.AddObservation((CurrentTargetPosition() - CurrentBotPosition()).normalized);
        // Current velocity  
        sensor.AddObservation(GetVelocity().normalized);
        // Current angular velocity   -305 to 298
        sensor.AddObservation(GetSignedAngularVelocity()/360.0f);
        // Pitch & Yaw Angle 
        sensor.AddObservation(GetSignedAngle()/180.0f);
        // Alignment
        sensor.AddObservation(GetAlingment());                        
    }                          
        
    public override void OnActionReceived(float[] vectorAction)
    {
        botMovement.leftSpeed = Mathf.Clamp(vectorAction[0], -1f, 1f);
        botMovement.rightSpeed = Mathf.Clamp(vectorAction[1], -1f, 1f);

        float distanceError = GetDistance();
        float velocityError = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(0.0f, 3.0f, GetVelocity().magnitude)); // scaling value to 0 to 1
        float angleError = Mathf.Abs(GetSignedAngle().x);
        float angularVelocityError = Mathf.Abs(GetSignedAngularVelocity().x);
        // Reward
        if (angleError >= 45 || transform.localPosition.y < -1.0f) // Falling Condition and if bot falls from platform and also it will not far if it is not balanced
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        // Alignment reward
        alignmentReward = GetAlingment();

        //Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 0)
        if (distanceError <= 0.5f)
        {
            targetReached = true;
            AddReward(1.0f); //high positive reward if it reaches goal
            SetTarget();
            targetReached = false;
            
        }

        //distanceReward = 1 - Mathf.Pow(distanceError/maxDistance,0.4f); // decreasing function
        if (distanceError < prevDistance)
        {
            distanceReward = 0.1f;
        }
        else
        {
            distanceReward = -0.1f;
        }
        //distanceReward *= Mathf.Abs(DistanceExponent(distanceError) - DistanceExponent(prevDistance));  // value aroun e-06
        AddReward(distanceReward);
        prevDistance = distanceError;
        //Debug.Log(GetVelocity().magnitude);
        //velocityDiscount = Mathf.Pow(1 - Mathf.Max(velocityError, 0.1f), 1 / Mathf.Max(distanceError / maxDistance, 0.2f))/10;
        angleReward = Mathf.Clamp(1.0f - 0.1f * angleError - 0.01f * angularVelocityError, 0.0f, 1.0f)/10;
        // reward_sum += alignmentReward * angleReward * velocityDiscount - 0.1f;
        //Debug.Log(reward_sum);
        
        matchingVelocityReward = GetMatchingVelocityReward();
        //Debug.Log(angleError);
        AddReward(alignmentReward * angleReward * matchingVelocityReward - 0.1f);
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
    public void SetTarget()
    {
        cases = (float)Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 5);
        target.localPosition = new Vector3(Random.Range(-cases, cases), -0.25f, Random.Range(-cases, cases));
    }
    public void EpisodeReset()
    {
        ResetBot();
        SetTarget();
    }
    public float GetMatchingVelocityReward()   // Forces bot to match velocity of 1.5
    {
        float velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(GetVelocity(),transform.forward.normalized * velocityGoal),0,1);
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / velocityGoal, 2), 2);
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
    public Vector2 GetSignedAngularVelocity()
    {
        return Mathf.Rad2Deg * new Vector2(transform.InverseTransformDirection(rbBot.angularVelocity).x,
                                           transform.InverseTransformDirection(rbBot.angularVelocity).y);
    }

    public float GetAlingment()
    {
        Vector2 vecForward = new Vector2(transform.forward.x,transform.forward.z);
        Vector2 vec2Direction = (CurrentTargetPosition() - CurrentBotPosition()).normalized;
        

        return Vector2.Dot(vec2Direction,vecForward);
    }
    public override void Heuristic(float[] actionsOut)
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        actionsOut[0] = inputY + inputX;
        actionsOut[1] = inputY - inputX;
    }
    public float DistanceExponent(float val)
    {
        return 1 - Mathf.Pow(val/maxDistance,0.4f);
    }
}