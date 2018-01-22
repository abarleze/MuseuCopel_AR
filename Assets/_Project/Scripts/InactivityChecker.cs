using UnityEngine;

public class InactivityChecker : MonoBehaviour
{
    public GameEvent inactivityEvent;
    public float maxInactiveTime = 40.0f;

    private bool _inactive;
    private float _time;

    private void OnEnable()
    {
        ActivityDetected();
    }

    private void Update()
    {
        if (_inactive)
            return;
        
        if (_time < Time.time)
        {
            _inactive = true;
            inactivityEvent.Raise();
        }
    }

    public void ActivityDetected()
    {
        _time = Time.time + maxInactiveTime;
        _inactive = false;
    }
}