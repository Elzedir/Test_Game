using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_LoadMenu : MonoBehaviour
{
    public MainMenu_Manager mainMenuManager;
    public GameObject deleteSaveCheckWindow;

    public void Start()
    {
        transform.localScale = Vector3.zero;
        deleteSaveCheckWindow.transform.localScale = Vector3.zero;
    }
    public void OpenMenu()
    {
        transform.localScale = Vector3.one;
        mainMenuManager.menuOpen = true;
    }

    public void LoadSaveFile(int saveSlot)
    {
        GameManager.instance.LoadState(saveSlot);
    }

    public void DeleteSaveCheckOpen()
    {
        deleteSaveCheckWindow.transform.localScale = Vector3.one;
    }

    public void DeleteSaveFile(int saveSlot)
    {
        Save_Manager.DeleteSave(saveSlot);
        DeleteSaveCheckClose();
    }

    public void DeleteSaveCheckClose()
    {
        deleteSaveCheckWindow.transform.localScale = Vector3.zero;
    }

    public void CloseMenu()
    {
        transform.localScale = Vector3.zero;
        mainMenuManager.menuOpen = false;
    }

    public void LoadMostRecentSave()
    {
        GameManager.instance.LoadState(GameManager.instance.mostRecentAutoSave);
    }
}

