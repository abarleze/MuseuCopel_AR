using UnityEngine;

public class BezierBase : MonoBehaviour
{
    public virtual Vector3 GetPoint(float t) { return Vector3.zero; }

    public virtual Vector3 GetLocalPoint(float t) { return Vector3.zero; }

    public virtual Vector3 GetVelocity(float t) { return Vector3.zero; }

    public virtual Vector3 GetDirection(float t) { return Vector3.zero; }
}
