using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HelpScreenSwipe : MonoBehaviour {

    public Transform screens;
    public Transform[] spheres;
    private int screenCount = 0;
    private int totalScreens = 0;
    private bool swiping = false;
    private int screenSize = 2048;
    private int lastPosition = 0;

    private void Awake()
    {
        SpheresResetScale();
        totalScreens = spheres.Length - 1;
    }

    // Use this for initialization
    void Start () {
        spheres[0].localScale = new Vector3(1.5f, 1.5f, 1f);
    }
	
	// Update is called once per frame
	void Update () {
        if (!swiping)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && screenCount < totalScreens)
            {
                swiping = true;
                MoveLeft();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) && screenCount > 0)
            {
                swiping = true;
                MoveRight();
            }
        }
    }

    private void SpheresResetScale()
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            spheres[i].localScale = Vector3.one;
        }
    }

    private int GetPositionToMove()
    {
        int positionToMove = 0;
        if (screenCount != 0)
        {
            positionToMove = screenSize * screenCount;
        }
        switch (screenCount)
        {
            case 1:
                
                break;
        }
        return positionToMove;
    }

    private void MoveRight()
    {
        SpheresResetScale();
        lastPosition += screenSize;
        screens.DOLocalMove(new Vector3(lastPosition, 0, 0), 1f, true);
        DownScale();
    }

    private void MoveLeft()
    {
        SpheresResetScale();
        lastPosition -= screenSize;
        screens.DOLocalMove(new Vector3(lastPosition, 0, 0), 1f, true);
        UpScale();
    }

    private void UpScale()
    {
        screenCount += 1;
        spheres[screenCount].DOScale(1.5f, 0.5f).OnComplete(()=> swiping = false);
    }

    private void DownScale()
    {
        screenCount -= 1;
        spheres[screenCount].DOScale(1.5f, 0.5f).OnComplete(() => swiping = false);
    }
}
