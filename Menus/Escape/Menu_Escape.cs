using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Menu_Escape : Menu_UI
{
    public static Menu_Escape Instance;

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
        GameManager.Instance.ChangeState(GameState.Paused);
    }
    public override void CloseMenu(GameObject interactedObject = null)
    {
        gameObject.SetActive(false);
        _isOpen = false;
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void CharacterSelectButtonPressed()
    {
        if (GameManager.Instance.CurrentState != GameState.InCombat)
        {
            CloseMenu();
            Menu_CharacterSelect.Instance.OpenMenu();
        }
        else
        {
            Debug.Log("Player is in combat");
        }
    }

    public void SettingsButtonPressed()
    {
        if (GameManager.Instance.CurrentState != GameState.InCombat)
        {
            CloseMenu();
            Menu_Settings.Instance.OpenMenu();
        }
        else
        {
            Debug.Log("Player is in combat");
        }
    }
}
