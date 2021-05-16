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

    

    public override void OnEpisodeBegin()                                                    // to edit this
    {
        parameters.EpisodeReset();                                                           //Reset the scene
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);                                         // Target direction
        sensor.AddObservation(targetSpeed);                                                  // Target speed
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
        float velocityError = Mathf.Abs(targetSpeed - rBody.velocity.magnitude);
        float angleError = Mathf.Abs(parameters.GetSignedPitchAngle());
        float angularVelocityError = Mathf.Abs(parameters.GetSignedAngularVelocity());  // make it unsigned
        //Debug.Log(distanceError);
        // Reward
        if (angleError >= 90) // Falling Condition
        {
            //Debug.Log("end");
            SetReward(-1.0f);
            EndEpisode();
        }
        else
        {
            AddReward(1.0f*(1.0f - (0.1f) * (angleError) - 0.01f * ((angularVelocityError))));
        }
        AddReward((1.0f - (distanceError) * 0.1f - (velocityError) * 0.01f - 0.01f));
        //Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 0)
        if (Unity.MLAgents.Academy.Instance.EnvironmentParameters.GetWithDefault("cases", 0) == 1)  // ciruculam learning
        {
            if (distanceError <= 0.75f)
            {
                SetReward(1.5f); 
                EndEpisode();
            }
        }
        else
            {
                AddReward(1.5f*(1.0f - (distanceError) * 0.7f - (velocityError) * 0.5f - 1.50f*StepCount/5000));
            }
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