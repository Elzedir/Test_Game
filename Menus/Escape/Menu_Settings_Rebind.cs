using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Menu_Settings_Rebind : MonoBehaviour
{
    public static Menu_Settings_Rebind Instance;
    public TextMeshProUGUI RebindQuestion;
    public TextMeshProUGUI PressedKey;

    public bool IsOpen = false;
    private bool _isWaitingForConfirmation = false;
    private KeyCode _newKeyCode;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        gameObject.SetActive(false);
        RebindQuestion = GameManager.Instance.FindTransformRecursively(transform, "RebindQuestion").GetComponent<TextMeshProUGUI>();
        PressedKey = GameManager.Instance.FindTransformRecursively(transform, "PressedKey").GetComponent <TextMeshProUGUI>();
    }

    public void OpenMenu(KeyValuePair<ActionKey, KeyCode> keyAction)
    {
        gameObject.SetActive(true);
        RebindQuestion.text = $"To rebind key '{keyAction.Key.ToString()}', press another key. To go back, press '{Manager_Input.Instance.KeyBindings.Keys[ActionKey.Escape].ToString()}'.";
        PressedKey.text = $"";
        StartCoroutine(WaitForNextKeyPress(keyAction.Key));
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void OnRebindKeyPresssed(ActionKey actionKey, KeyCode newKey)
    {
        RebindQuestion.text = $"To confirm {actionKey.ToString()}'s rebind to {newKey.ToString()}, press {newKey.ToString()} again. To go back, press {Manager_Input.Instance.KeyBindings.Keys[ActionKey.Escape].ToString()}.";
        PressedKey.text = $"{newKey.ToString()}";
        _isWaitingForConfirmation = true;
    }

    public IEnumerator WaitForNextKeyPress(ActionKey actionKey)
    {
        while (true)
        {
            if (Input.GetKeyDown(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Escape]))
            {
                _isWaitingForConfirmation = false;
                CloseMenu();
                break;
            }

            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (_isWaitingForConfirmation)
                    {
                        if (_newKeyCode == keyCode)
                        {
                            Manager_Input.Instance.KeyBindings.RebindKey(actionKey, _newKeyCode);
                            _isWaitingForConfirmation = false;
                            Menu_Settings.Instance.OpenMenu();
                            Menu_Settings.Instance.ControlsCategorySelected();
                            break;
                        }
                    }
                    else
                    {
                        _newKeyCode = keyCode;
                        OnRebindKeyPresssed(actionKey, _newKeyCode);
                    }
                }
            }

            yield return null;
        }
    }
}
