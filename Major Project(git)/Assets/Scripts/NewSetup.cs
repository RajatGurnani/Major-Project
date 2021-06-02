using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSetup : MonoBehaviour
{
    public GameObject goalPrefab;
    Transform[] targets;
    int currentTargetCount = -1;
    public bool allTargetsReached =false;
    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<NewBalance>().ResetEpisode += RandomizeTargetPositions;
        
        targets = new Transform[10];
        for (int i = 0; i < 10; i++)
        {
            GameObject newTarget = Instantiate(goalPrefab, new Vector3(Random.Range(-45f, 45f), 1f, Random.Range(-45f, 45f)), Quaternion.identity);
            newTarget.transform.parent = gameObject.transform;
            targets[i] = newTarget.transform;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GetNewTarget();
        }
    }
    public void RandomizeTargetPositions()
    {
        allTargetsReached = false;
        foreach (Transform traPos in targets)
        {
            traPos.localPosition = new Vector3(Random.Range(-45f, 45f), 1f, Random.Range(-45f, 45f));
        }
    }
    public Vector3 GetNewTarget()
    {
        if (++currentTargetCount == targets.Length)                  // When last target is reached
        {
            allTargetsReached = true;
        }
        currentTargetCount = currentTargetCount % targets.Length;    // When all targets are covered count resets to zero
        Debug.Log(currentTargetCount);
        return targets[currentTargetCount].localPosition;            // return new position
    }
}