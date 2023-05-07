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
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = worldPosition;
    }
    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
}
