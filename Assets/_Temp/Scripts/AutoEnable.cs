using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoEnable : MonoBehaviour
{
    public GameObject obj;

    private void OnEnable()
    {
        if (!obj.activeInHierarchy)
            obj.SetActive(true);
    }
}
