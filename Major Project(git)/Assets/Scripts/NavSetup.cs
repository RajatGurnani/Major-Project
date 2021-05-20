using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavSetup : MonoBehaviour
{
    public Transform[] obstacles;
    public Transform startPoint, endPoint;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSetup();
        }
    }
    void ResetSetup()
    {
        startPoint.localPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-10f, -11f));
        endPoint.localPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(10f, 11f));
        foreach (Transform obs in obstacles)
        {
            int rand = Random.Range(0, 2) == 0 ? -1 : 1;
            float xScale = Random.Range(5f, 11f);
            Debug.Log(rand);
            obs.localPosition = new Vector3(obs.localPosition.x * rand, obs.localPosition.y, obs.localPosition.z);
            if (obs.localPosition.x > 0)
            {
                xScale *= -1;
            }
            obs.localScale = new Vector3(xScale, obs.localScale.y, obs.localScale.z);
        }
    }
}
