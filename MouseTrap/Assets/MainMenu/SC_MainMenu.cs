using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SC_MainMenu : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public GameObject MainMenu;
    public GameObject CreditsMenu;
    public GameObject LevelSelectMenu;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private List<int> levelsCompleted = new List<int>();
    //*************************************************************************


    // HEX FOR GRAY 4D4D4D

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void PlayNowButton()
    {
        // Play Now Button has been pressed, here you can initialize your game
        // (For example Load a Scene called GameLevel etc.)

        // Load highest achieved level or rather last saved level
        UnityEngine.SceneManagement.SceneManager.LoadScene("MT_GameScene");
    }

    public void CreditsButton()
    {
        // Show Credits Menu
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(true);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        CreditsMenu.SetActive(false);
    }

    public void LevelSelectButton()
    {
        // Show Level Select
        MainMenu.SetActive(false);
        LevelSelectMenu.SetActive(true);
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }

    // Load game from file
    void Load()
    {
        SaveData sd = new SaveData();

        // Check for save file
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                File.Open(Application.persistentDataPath +
                                            "/gamesave.save", FileMode.Open);
            sd = (SaveData)bf.Deserialize(file);
            levelsCompleted = sd.levelsCompleted;
            file.Close();
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }
}