using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_UI : MonoBehaviour
{
    protected bool _isOpen;
    public bool IsOpen
    {
        get { return _isOpen; }
    }

    public virtual void OpenMenu<T>(GameObject interactedObject = null) where T : MonoBehaviour
    {

    }

    public virtual void CloseMenu<T>(GameObject interactedObject = null) where T : MonoBehaviour
    {

    }
}
