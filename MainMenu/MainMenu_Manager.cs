using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_Manager : MonoBehaviour
{
    public Button continueButton;
    public Button newGameButton;
    public Button loadGameButton;
    public Button settingsButton;
    public Button quitButton;

    public MainMenu_NewGameMenu newGameMenu;
    public MainMenu_LoadMenu loadMenu;
    public MainMenu_Settings settingsMenu;

    public bool menuOpen = false;

    public void OnContinuePress()
    {
        loadMenu.LoadMostRecentSave();
    }

    public void OnNewGamePress()
    {
        newGameMenu.OpenMenu();
    }

    public void OnLoadGamePress()
    {
        loadMenu.OpenMenu();
    }

    public void OnSettingsPress()
    {
        settingsMenu.OpenMenu();
    }

    public void OnQuitPress()
    {
        if (!menuOpen)
        {
            // Exit application
        }
    }
}
