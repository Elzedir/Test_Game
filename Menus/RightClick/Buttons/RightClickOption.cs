using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickOption : MonoBehaviour
{
    public virtual void OnButtonPress()
    {
        Debug.Log("Original function not overridden");
    }
}
