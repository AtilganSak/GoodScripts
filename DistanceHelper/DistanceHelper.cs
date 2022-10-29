using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DistanceHelper : MonoBehaviour
{
    public GameObject target;

    public Color lineColor = new Color(0, 0, 0, 1);

    public DistanceCalculateMethod calculateMethod;

    public bool Draw = true;
    public bool showDistance = true;
    public bool showCenter;

    public float centerSize;

    private float Distance;   

    float CalcualteDistance()
    {
        Vector3 offset = transform.position - target.transform.position;
        float result = 0;
       
        switch (calculateMethod)
        {
            case DistanceCalculateMethod.SqrMagnitude:
                result = offset.sqrMagnitude;
                break;
            case DistanceCalculateMethod.Magnitude:
                result = offset.magnitude;
                break;
            case DistanceCalculateMethod.Vector3Distance:
                result = Vector3.Distance(transform.position, target.transform.position);
                break;
            default:
                break;
        }       
        return Mathf.Round(result);                
    }

    Vector3 CalculateVector()
    {
        Vector3 vectorResult = Vector3.zero;
        switch (calculateMethod)
        {     
            case DistanceCalculateMethod.Vector:
                vectorResult = transform.position - target.transform.position;
                break;
            default:
                break;
        }
        return vectorResult;
    }

    private void OnDrawGizmos()
    {    
        if (Draw)
        {
            Vector3 center = transform.position + target.transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.transform.position);
            if (showCenter)
            {
                Gizmos.DrawSphere(center / 2, centerSize);
            }
            if (showDistance)
            {
                if (calculateMethod == DistanceCalculateMethod.Vector)
                {
                    Vector3 vectorResult = CalculateVector();
                    Handles.Label(center / 2, vectorResult.ToString());
                }
                else
                {
                    float distance = CalcualteDistance();
                    Handles.Label(center / 2, distance.ToString() + "m");
                }
            }            
        }
    }

    public enum DistanceCalculateMethod
    {
        SqrMagnitude,
        Magnitude,
        Vector3Distance,
        Vector
    }
}
