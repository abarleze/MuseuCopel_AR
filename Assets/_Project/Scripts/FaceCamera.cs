using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera targetCamera;

    private void LateUpdate()
    {
        Vector3 direction = targetCamera.transform.position - transform.position;
        transform.LookAt(transform.position - direction);
    }
}