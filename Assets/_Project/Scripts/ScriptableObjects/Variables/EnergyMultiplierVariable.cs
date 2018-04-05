using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnergyMultiplierVariable : ScriptableObject {

    public Action onValueChange;
    [SerializeField]
    protected float _value;
    [SerializeField]
    protected bool _useDefaultValue = false;
    [SerializeField]
    protected float _defaultValue;

    private void OnEnable()
    {
        if (_useDefaultValue)
            _value = _defaultValue;
    }

    public float Value
    {
        get { return _value; }
        set
        {
            if (_value != value)
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
