#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class Ruller : MonoBehaviour
{
    public Transform tip;
    public float crossWidth = 0.1f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        DrawCross(transform.position, crossWidth);

        if (tip == null)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, tip.position);

        Gizmos.color = Color.blue;
        DrawCross(tip.position, crossWidth);

        float distance = Vector3.Distance(transform.position, tip.position) * 10.0f;

        Handles.Label((transform.position + tip.position)/2, " "+ distance.ToString("0.00") + "cm");
    }


    public void DrawCross(Vector3 position, float size)
    {
        Gizmos.DrawLine(position - Vector3.up * size, position + Vector3.up * size);
        Gizmos.DrawLine(position - Vector3.right * size, position + Vector3.right * size);
        Gizmos.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size);
    }
}
#endif
