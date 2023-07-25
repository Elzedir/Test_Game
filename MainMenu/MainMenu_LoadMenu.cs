using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_LoadMenu : MonoBehaviour
{
    public MainMenu_Manager mainMenuManager;
    public GameObject deleteSaveCheckWindow;

    public void Start()
    {
        gameObject.SetActive(false);
        deleteSaveCheckWindow.gameObject.SetActive(false);
    }
    public void OpenMenu()
    {
        gameObject.SetActive (true);
        mainMenuManager.menuOpen = true;
    }

    public void LoadSaveFile(int saveSlot)
    {
        GameManager.Instance.LoadState(saveSlot);
    }

    public void DeleteSaveCheckOpen()
    {
        deleteSaveCheckWindow.gameObject.SetActive(true);
    }

    public void DeleteSaveFile(int saveSlot)
    {
        Save_Manager.DeleteSave(saveSlot);
        DeleteSaveCheckClose();
    }

    public void DeleteSaveCheckClose()
    {
        deleteSaveCheckWindow.gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        mainMenuManager.menuOpen = false;
    }

    public void LoadMostRecentSave()
    {
        GameManager.Instance.LoadState(GameManager.Instance.mostRecentAutoSave);
    }
}

