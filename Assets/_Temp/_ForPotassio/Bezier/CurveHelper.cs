using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurveHelper : MonoBehaviour
{
    public BezierCurve bezierCurve;

    public Transform[] points;
    
	void Update ()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != null)
                bezierCurve.points[i] = points[i].position;
        }
	}
}
