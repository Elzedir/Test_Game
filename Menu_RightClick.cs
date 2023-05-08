using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Menu_RightClick : MonoBehaviour
{
    public Manager_Input inputManager;
    public Button pickupButton;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideMenu();
        }
    }
    public void OnRightClick()
    {
        gameObject.SetActive(true);
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
    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
}
