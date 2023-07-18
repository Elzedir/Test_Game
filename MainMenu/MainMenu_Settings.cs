using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Settings : MonoBehaviour
{
    public MainMenu_Manager mainMenuManager;

    public void Start()
    {
        gameObject.SetActive(false);
    }
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        mainMenuManager.menuOpen = true;
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        mainMenuManager.menuOpen = false;
    }
}
