using UnityEngine;
using System.Collections;
 
/// <summary>
/// Add this script to the Camera
/// </summary>
[RequireComponent(typeof(Camera))]
public class DrawFrustumGizmo : MonoBehaviour
{

    public virtual void OnDrawGizmos()
    {
        Matrix4x4 temp = Gizmos.matrix;

        float aspectRatio = Screen.width / Screen.height;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1.0f, aspectRatio, 1.0f));
        
        //Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        if (GetComponent<Camera>().orthographic)
        {
            float spread = GetComponent<Camera>().farClipPlane - GetComponent<Camera>().nearClipPlane;
            float center = (GetComponent<Camera>().farClipPlane + GetComponent<Camera>().nearClipPlane) * 0.5f;
            Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(GetComponent<Camera>().orthographicSize * 2 * GetComponent<Camera>().aspect, GetComponent<Camera>().orthographicSize * 2, spread));
        }
        else
        {
            //Gizmos.DrawFrustum(Vector3.zero, GetComponent<Camera>().fieldOfView, GetComponent<Camera>().farClipPlane, GetComponent<Camera>().nearClipPlane, GetComponent<Camera>().aspect);
            Gizmos.DrawFrustum(Vector3.zero, GetComponent<Camera>().fieldOfView, GetComponent<Camera>().farClipPlane, GetComponent<Camera>().nearClipPlane, aspectRatio);
        }
        Gizmos.matrix = temp;
    }
}
