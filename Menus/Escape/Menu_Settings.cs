using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Settings : Menu_UI
{
    public static Menu_Settings Instance;
    public GameObject Hotkey;
    private List<GameObject> _openSetting = new();

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public override void OpenMenu(GameObject interactedObject = null)
    {
        gameObject.SetActive(true);
        _isOpen = true;
        Menu_Settings_Rebind.Instance.CloseMenu();
        GameManager.Instance.ChangeState(GameState.Paused);
    }
    public override void CloseMenu(GameObject interactedObject = null)
    {
        foreach (GameObject setting in _openSetting)
        {
            Destroy(setting);
        }
        _openSetting.Clear();

        gameObject.SetActive(false);
        _isOpen = false;
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void ControlsCategorySelected()
    {
        Transform settings = GameManager.Instance.FindTransformRecursively(transform, "Settings");

        foreach (KeyValuePair <ActionKey, KeyCode> key in Manager_Input.Instance.KeyBindings.Keys)
        {
            GameObject hotkeyGO = Instantiate(Hotkey, settings);
            hotkeyGO.GetComponent<Menu_Controls_Hotkey>().SetDetails(key);
            _openSetting.Add(hotkeyGO);
        }
    }
}
