using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FloatSum : FloatVariable
{
    public List<FloatVariable> references;
    
    private void OnEnable()
    {
        if (references != null)
            foreach (var reference in references)
                reference.onValueChange += CalculateValue;

        if (!_useDefaultValue)
        {
            CalculateValue();
        }
        else
        {
            _value = _defaultValue;
        }
    }
    
    private void CalculateValue()
    {
        float temp = 0;
        if (references != null)
            foreach (var item in references)
                temp += item.Value;

        Value = temp;
    }
}