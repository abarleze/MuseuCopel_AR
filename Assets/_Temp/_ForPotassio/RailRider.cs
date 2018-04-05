using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailRider : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public BezierCurve curve;
    public ParticleSystem pSystem;
    
    public void OnEnable()
    {
        StartCoroutine(Ride());
    }

    IEnumerator Ride()
    {
        float t = 0.0f;
        pSystem.Stop();
        transform.position = curve.GetPoint(t);
        pSystem.Play();

        while (t < 1.0f)
        {
            t += Time.deltaTime * speed;
            t = Mathf.Clamp01(t);
            transform.position = curve.GetPoint(t);
            transform.Rotate(Vector3.forward, 360 * Time.deltaTime * turnSpeed);
            yield return null;
        }

        StartCoroutine(Ride());
    }
}
