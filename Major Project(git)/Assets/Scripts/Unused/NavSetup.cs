using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NavSetup : MonoBehaviour
{
    public Transform[] obstacles;
    public Transform startPoint, endPoint;
    public Vector3[] sortedCheckpoints;
    public int checkPointCount=-1;
    private void Start()
    {
        ResetSetup();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSetup();
        }
    }
    public void ResetSetup()
    {
        ResetStartEndPoints();
        ResetObstacles();
        GenerateCheckPoints();
        checkPointCount = -1;
    }
    void ResetObstacles()
    {
        foreach (Transform obs in obstacles)
        {
            int rand = Random.Range(0, 2) == 0 ? -1 : 1;
            float xScale = Random.Range(5f, 11f);
            obs.localPosition = new Vector3(obs.localPosition.x * rand, obs.localPosition.y, obs.localPosition.z);
            if (obs.localPosition.x > 0)
            {
                xScale *= -1;
            }
            obs.localScale = new Vector3(xScale, obs.localScale.y, obs.localScale.z);
        }
    }
    void ResetStartEndPoints()
    {
        startPoint.localPosition = new Vector3(Random.Range(-5f, 5f), startPoint.localPosition.y, Random.Range(-12f, -11f));
        endPoint.localPosition = new Vector3(Random.Range(-5f, 5f), endPoint.localPosition.y, Random.Range(3f, 5f));
    }
    public Vector3 ProvideTargetPosition()
    {
        return endPoint.localPosition;
    }
    public Vector3 ProvideStartPosition()
    {
        return startPoint.localPosition;
    }
    public void GenerateCheckPoints()
    {
        Vector3[] unsortedChecpoints = new Vector3[obstacles.Length];
        for (int i = 0; i < obstacles.Length; i++)
        {
            unsortedChecpoints[i] = obstacles[i].position;
        }
        sortedCheckpoints = unsortedChecpoints.OrderBy(v => v.z).ToArray();
        /*for (int i = 0; i < sortedCheckpoints.Length; i++)
        {
            Debug.Log(sortedCheckpoints[i]);
        }*/
    }
    public Vector3 GetNextCheckpoint()
    {
        ++checkPointCount;
        Debug.Log(checkPointCount);
        return sortedCheckpoints[checkPointCount];
    }
}