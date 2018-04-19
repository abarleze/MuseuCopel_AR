using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 45.0f;

    public void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}