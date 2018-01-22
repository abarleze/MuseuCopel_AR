using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InteractableBluePrint : ScriptableObject {

    public bool isOn = false;
    public FloatVariable energy;
    public float kwh = 0.12f;
    public Sprite icon;

    private void OnEnable()
    {
        isOn = false;
    }
}
