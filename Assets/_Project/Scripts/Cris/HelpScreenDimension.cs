using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpScreenDimension : MonoBehaviour {

    public RectTransform[] screens; 

	// Use this for initialization
	void Awake () {
        SetScreensDimension();

    }
    void SetScreensDimension()
    {
        int screenWidth = Screen.width * 2;
        for (int i = 0; i < screens.Length; i++)
        {
            if (i > 0)
            {
                screens[i].localPosition = new Vector3(screenWidth * i, 0, 0);
            }
        }
    }
}
