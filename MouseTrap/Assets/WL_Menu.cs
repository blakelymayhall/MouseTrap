using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        {
            canvas.transform.GetChild(2).gameObject.
                GetComponent<Text>().text = "You Win!";
            if (name == "PlayAgain")
            {
                GetComponentInChildren<Text>().text = "Next Level?";
                if (Manager.level == 5)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            canvas.transform.GetChild(2).gameObject.
                GetComponent<Text>().text = "You Lose!";
            if (name == "PlayAgain")
            {
                GetComponentInChildren<Text>().text = "Try Again?";
            }
        }
        canvas.transform.GetChild(4).gameObject.
                GetComponent<Text>().text = "Total Clicks:\n" +
                Manager.numClicks.ToString();

    }

    public void KeepPlayingButton()
    {
        if (manager.userWin)
            Manager.level++;

        manager.retry = true;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitButton()
    {
        manager.quit = true;
    }
}
