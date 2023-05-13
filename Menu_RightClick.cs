using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Menu_RightClick : MonoBehaviour
{
    public Manager_Input inputManager;
    public Button pickupButton;
    public bool isOpen = false;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void RightClickMenuOpen()
    {
        isOpen = true;

        transform.localScale = Vector3.one;
        Vector3 mousePosition = Input.mousePosition;
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //transform.position = worldPosition;

        float windowWidth = Screen.width;
        float windowHeight = Screen.height;

        float menuWidth = GetComponent<RectTransform>().rect.width;
        float menuHeight = GetComponent<RectTransform>().rect.height;
        float xPosition = mousePosition.x + menuWidth > windowWidth ? mousePosition.x - menuWidth : mousePosition.x;
        float yPosition = mousePosition.y + menuHeight > windowHeight ? mousePosition.y - menuHeight : mousePosition.y;

        transform.position = new Vector3(xPosition, yPosition, transform.position.z);
        
    }
    public void RightClickMenuClose()
    {
        isOpen = false;
        transform.localScale = Vector3.zero;
        transform.position = new Vector3(0, 0, 0);
    }
}
