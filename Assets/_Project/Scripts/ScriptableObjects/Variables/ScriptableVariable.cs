using System;
using UnityEngine;

public abstract class ScriptableVariable<T> : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField, TextArea]
    private string _designNotes;
#endif

    public Action onValueChange;
    [SerializeField]
    protected T _value;
    [SerializeField]
    protected bool _useDefaultValue = false;
    [SerializeField]
    protected T _defaultValue;

    private void OnEnable()
    {
        if (_useDefaultValue)
            _value = _defaultValue;
    }

    public T Value
    {
        get { return _value; }
        set
        {
            if (_value != null && !_value.Equals(value))
            {
                _value = value;
                if (onValueChange != null)
                    onValueChange();
            }
        }
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}