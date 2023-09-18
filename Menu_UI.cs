using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_UI : MonoBehaviour
{
    public GameObject InteractedObject;
    protected bool _isOpen;
    public bool IsOpen
    {
        get { return _isOpen; }
    }

    public virtual void OpenMenu(GameObject interactedObject = null)
    {

    }

    public virtual void CloseMenu(GameObject interactedObject = null)
    {

    }
}
