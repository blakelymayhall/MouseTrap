using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WL_Menu : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************

    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private Manager manager;
    private Canvas canvas;
    //*************************************************************************

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<Canvas>().
            GetComponentInParent<Manager>();
        canvas = GetComponentInParent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1;
        
        if (manager.userWin)
            canvas.transform.GetChild(1).gameObject.
                GetComponent<Text>().text = "You Win!";
        else
            canvas.transform.GetChild(1).gameObject.
                GetComponent<Text>().text = "You Lose!";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAgainButton()
    {
        Debug.Log("TITS");
    }
}
