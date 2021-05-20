 using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Balance_Bot : Agent
{
    public Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    float inputX, inputY;
    public Transform target;
    public float targetSpeed;

    public BotParameters parameters;
    public Movement botMovement;

    const float maxDistance = 10.0f; // Maximum possible distance
    float angleReward; // Reward for angle position
    float distanceReward; // closer to be for more reward
    float velocityDiscount; // forces velocity to be lower if distance decreases
    float prevDistance;
    

    public override void OnEpisodeBegin()                                                    
    {
        parameters.EpisodeReset();                                                           //Reset the scene
        prevDistance = 10;
        distanceReward = 0;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);                                         // Target direction
        sensor.AddObservation(targetSpeed);                                                  // Target speed --Not used
        sensor.AddObservation(transform.localPosition);                                      // Current direction

        sensor.AddObservation(parameters.GetVelocity());                                     // Current velocity  
        sensor.AddObservation(parameters.GetSignedAngularVelocity());                        // Current angular velocity 
        sensor.AddObservation(parameters.GetSignedPitchAngle());                             // Pitch Angle
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        botMovement.leftSpeed = Mathf.Clamp(vectorAction[0], -1f, 1f);
        botMovement.rightSpeed = Mathf.Clamp(vectorAction[1], -1f, 1f);

        float distanceError = Vector3.Distance(target.localPosition, transform.localPosition);
        float velocityError = Mathf.Lerp(0.0f,1.0f,Mathf.InverseLerp(0.0f,3.0f,rBody.velocity.magnitude)); // scaling value to 0 to 1
        float angleError = Mathf.Abs(parameters.GetSignedPitchAngle());
        float angularVelocityError = Mathf.Abs(parameters.GetSignedAngularVelocity());  
        //Debug.Log(velocityError);
        // Reward
        if (angleError >= 90 || transform.localPosition.y < -1.0f) // Falling Condition and if bot falls from platform
        {
            //Debug.Log("fall");
            SetReward(-10000.0f);    
            EndEpisode();
        }
        //Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 0)
        if (distanceError <= 0.1f)
        {
            SetReward(10000.0f); //high positive reward if it reaches goal
            EndEpisode();
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

        velocityDiscount = Mathf.Pow((1 - Mathf.Max(velocityError,0.1f)),(1/Mathf.Max(distanceError/maxDistance,0.2f)));
        angleReward = Mathf.Clamp((1.0f - (0.1f) * (angleError) - 0.01f * ((angularVelocityError))),0.0f,1.0f);
        //Debug.Log(distanceReward*angleReward*velocityDiscount - 0.01f);
        AddReward(distanceReward*angleReward*velocityDiscount - 0.01f);
            
           
    }

    public override void Heuristic(float[] actionsOut)
    {
        //Debug.Log("hue");
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        actionsOut[0] = inputX;
        actionsOut[1] = inputY;
    }
}