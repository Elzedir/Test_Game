using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Menu_CharacterSelect : Menu_UI
{
    public static Menu_CharacterSelect Instance;

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

    public override void OpenMenu<T>(GameObject interactedObject = null)
    {
        gameObject.SetActive(true);
        _isOpen = true;
        GameManager.Instance.ChangeState(GameState.Paused);
    }
    public override void CloseMenu<T>(GameObject interactedObject = null)
    {
        gameObject.SetActive(false);
        _isOpen = false;
        GameManager.Instance.ChangeState(GameState.Playing);
    }
}
