using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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
    private SaveData sd = new SaveData();
    //*************************************************************************

    // Start is called before the first frame update
    void Start()
    {
        Load();
        MainMenuButton();
    }

    // Play the latest level in the saved game, or level 1
    public void PlayNowButton()
    {
        // Play Now Button has been pressed, here you can initialize your game
        // (For example Load a Scene called GameLevel etc.)
        Manager.level = sd.levelsCompleted.Max();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MT_GameScene");
    }

    // Open credits menu
    public void CreditsButton()
    {
        // Show Credits Menu
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(true);
    }

    // Open main menu
    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        CreditsMenu.SetActive(false);
        LevelSelectMenu.SetActive(false);
    }

    // Open level select menu
    public void LevelSelectButton()
    {
        // Show Level Select
        MainMenu.SetActive(false);
        LevelSelectMenu.SetActive(true);

        var buttons = LevelSelectMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name != "Button5" && button.name != "Button")
                button.interactable = false;
        }

        if (sd.levelsCompleted.Max() > 1)
        {
            List<int> allowedLevels = sd.levelsCompleted.Distinct().ToList<int>();
            foreach (Button button in buttons)
            {
                if (allowedLevels.ConvertAll<string>(x => x.ToString()).Any(x =>
                    button.GetComponentInChildren<Text>().text.EndsWith(x)))
                {
                    button.interactable = true;
                }
            }
        }
    }

    // Load a particular level
    public void LoadSpecificLevel()
    {
        Manager.level = int.Parse(EventSystem.current.currentSelectedGameObject.
            GetComponentInChildren<Text>().text.Last().ToString());
        UnityEngine.SceneManagement.SceneManager.LoadScene("MT_GameScene");
    }

    // Quit game
    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }

    // Load game from file
    void Load()
    {
        // Check for save file
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                File.Open(Application.persistentDataPath +
                                            "/gamesave.save", FileMode.Open);
            sd = (SaveData)bf.Deserialize(file);

            file.Close(); 
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }
}