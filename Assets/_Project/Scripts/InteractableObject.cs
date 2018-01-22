using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractableObject : MonoBehaviour, IPointerClickHandler
{
    public InteractableBluePrint interactableBluePrint;
    public float animationTime = 0.5f;

    public UnityEvent onTurnOn;
    public UnityEvent onTurnOff;

    private void Awake()
    {
        interactableBluePrint.energy.Value = (interactableBluePrint.isOn ? interactableBluePrint.kwh : 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        interactableBluePrint.isOn = !interactableBluePrint.isOn;

        StopAllCoroutines();
        if (interactableBluePrint.isOn)
        {
            onTurnOn.Invoke();
            StartCoroutine(ValueOverTime(interactableBluePrint.kwh));
        }
        else
        {
            onTurnOff.Invoke();
            StartCoroutine(ValueOverTime(0.0f));
        }
    }

    private IEnumerator ValueOverTime(float endValue)
    {
        float lenght = Mathf.Abs(interactableBluePrint.energy.Value - endValue);
        while (interactableBluePrint.energy.Value != endValue)
        {
            yield return null;
            interactableBluePrint.energy.Value = Mathf.MoveTowards(interactableBluePrint.energy.Value, endValue, (Time.deltaTime * lenght) / animationTime);
        }
    }
}