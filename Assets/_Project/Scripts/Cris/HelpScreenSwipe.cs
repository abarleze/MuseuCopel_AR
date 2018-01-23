using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HelpScreenSwipe : MonoBehaviour
{

    public Transform screens;
    public Transform[] spheres;
    private int screenCount = 0;
    private int totalScreens = 0;
    private bool swiping = false;
    private int screenSize = 2048;
    private int lastPosition = 0;

    #region mobile
    private Vector3 firstTouchPosition;
    private Vector3 lastTouchPosition;
    private float dragDistance;
    #endregion

    private void Awake()
    {
        SpheresResetScale();
        screenSize = Screen.width;
        totalScreens = spheres.Length - 1;
        dragDistance = screenSize * 5 / 100; //dragDistance is 15% height of the screen
        //Debug.Log("Width: " + screenSize);
    }

    // Use this for initialization
    void Start()
    {
        spheres[0].localScale = new Vector3(1.5f, 1.5f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

        #region mobileControls
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                firstTouchPosition = touch.position;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lastTouchPosition = touch.position;  //last touch position. Ommitted if you use list

                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(lastTouchPosition.x - firstTouchPosition.x) > dragDistance || Mathf.Abs(lastTouchPosition.y - firstTouchPosition.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lastTouchPosition.x - firstTouchPosition.x) > Mathf.Abs(lastTouchPosition.y - firstTouchPosition.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((lastTouchPosition.x > firstTouchPosition.x))  //If the movement was to the right)
                        {   //Right swipe
                            //Debug.Log("Right Swipe");
                            if (screenCount > 0)
                            {
                                MoveRight();
                            }
                        }
                        else
                        {   //Left swipe
                            //Debug.Log("Left Swipe");
                            if (screenCount < totalScreens)
                            {
                                MoveLeft();
                            }
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lastTouchPosition.y > firstTouchPosition.y)  //If the movement was up
                        {   //Up swipe
                            //Debug.Log("Up Swipe");
                        }
                        else
                        {   //Down swipe
                            //Debug.Log("Down Swipe");
                        }
                    }
                }
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    //Debug.Log("Tap");
                }
            }
        }
        #endregion
        //Teste pelas setas
        //if (!swiping)
        //{
        //    if (Input.GetKeyDown(KeyCode.LeftArrow) && screenCount < totalScreens)
        //    {
        //        swiping = true;
        //        MoveLeft();
        //    }
        //    if (Input.GetKeyDown(KeyCode.RightArrow) && screenCount > 0)
        //    {
        //        swiping = true;
        //        MoveRight();
        //    }
        //}
    }

    private void SpheresResetScale()
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            spheres[i].localScale = Vector3.one;
        }
    }

    private void MoveRight()
    {
        SpheresResetScale();
        lastPosition += screenSize * 2;
        screens.DOLocalMove(new Vector3(lastPosition, 0, 0), 1f, true);
        DownScale();
    }

    private void MoveLeft()
    {
        SpheresResetScale();
        lastPosition -= screenSize * 2;
        screens.DOLocalMove(new Vector3(lastPosition, 0, 0), 1f, true);
        UpScale();
    }

    private void UpScale()
    {
        screenCount += 1;
        spheres[screenCount].DOScale(1.5f, 0.5f).OnComplete(() => swiping = false);
    }

    private void DownScale()
    {
        screenCount -= 1;
        spheres[screenCount].DOScale(1.5f, 0.5f).OnComplete(() => swiping = false);
    }
}
