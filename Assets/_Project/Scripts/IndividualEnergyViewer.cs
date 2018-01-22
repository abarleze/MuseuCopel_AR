using UnityEngine;
using UnityEngine.UI;

public class IndividualEnergyViewer : MonoBehaviour
{
    public FloatVariable energy;
    public Text text;
    public FloatVariable multiplier;
    public bool useMultiplier;

    private void OnEnable()
    {
        energy.onValueChange += UpdateView;
        if (useMultiplier)
        {
            multiplier = Resources.Load<FloatVariable>("GlobalVariables/EnergyMultiplier");
            multiplier.onValueChange += UpdateView;
        }
        UpdateView();
    }

    private void OnDisable()
    {
        energy.onValueChange -= UpdateView;
        if (useMultiplier)
        {
            multiplier.onValueChange -= UpdateView;
        }
    }

    private void UpdateView()
    {
        float energyValue = energy.Value * (useMultiplier ? multiplier.Value : 1);
        text.text = energyValue.ToString("0.00");	
	}
}
