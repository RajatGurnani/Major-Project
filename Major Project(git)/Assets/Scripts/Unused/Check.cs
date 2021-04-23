using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
    void Update()
    {
        //Test1();
        Test2();
    }
    void Test1()
    {

        float angle = transform.localEulerAngles.x;

        //Debug.Log(UnityEditor.TransformUtils.GetInspectorRotation(transform));
        if (angle > 180)
        {
            angle = angle - 360;
        }
        Debug.Log(angle);
    }
    void Test2()
    {
        Debug.Log(Vector3.SignedAngle(Vector3.up,transform.up,transform.right));
    }
}
