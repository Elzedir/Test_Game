using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input instance;

    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, System.Action> keyListeners = new Dictionary<string, System.Action>();

    public static Manager_Input Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    public void AddKeyListener(string keyName, System.Action action, KeyCode keyCode)
    {
        keyBindings.Add(keyName, keyCode);
        keyListeners.Add(keyName, action);
    }

    void Update()
    {
        foreach (KeyValuePair<string, KeyCode> keyBinding in keyBindings)
        {
            if (Input.GetKeyDown(keyBinding.Value))
            {
                if (keyListeners.ContainsKey(keyBinding.Key))
                {
                    keyListeners[keyBinding.Key].Invoke();
                }
            }
        }
    }
}
