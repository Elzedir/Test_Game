using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Settings : MonoBehaviour
{
    public MainMenu_Manager mainMenuManager;

    public void Start()
    {
        transform.localScale = Vector3.zero;
    }
    public void OpenMenu()
    {
        transform.localScale = Vector3.one;
        mainMenuManager.menuOpen = true;
    }

    public void CloseMenu()
    {
        transform.localScale = Vector3.zero;
        mainMenuManager.menuOpen = false;
    }
}
