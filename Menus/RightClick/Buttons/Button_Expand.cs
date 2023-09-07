using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Button_Expand : MonoBehaviour
{
    public Transform ExpandPanel;

    public void Awake()
    {
        ExpandPanel = transform.Find("ExpandPanel");
    }

    public void OnButtonPress()
    {
        ExpandPanel.gameObject.SetActive(true);
    }
}
