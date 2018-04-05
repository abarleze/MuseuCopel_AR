using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class AutoFocus : MonoBehaviour
{
	void Start ()
    {
        bool autofocusOK = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        if (autofocusOK)
        {
            Debug.Log("Successfully enabled Continuous Autofocus mode");
        }
        else
        {
            Debug.Log("Failed to enable Continuous Autofocus mode");
        }
    }
}
