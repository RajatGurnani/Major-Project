using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Balance_Bot balanceRef;
    private void Start()
    {
        balanceRef = transform.parent.GetComponentInChildren<Balance_Bot>();   
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Obstacle") || collision.collider.CompareTag("Wall"))
        {
            balanceRef.botCollided = true;
        }
    }
}
