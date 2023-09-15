using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : Menu_UI
{
    public static UI_Slider instance;
    public Slider slider;
    public TextMeshProUGUI sliderTitle;
    public TextMeshProUGUI sliderAmount;
    public Equipment_Slot equipmentSlot;
    public Inventory_Slot inventorySlot;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        gameObject.SetActive(false);
    }

    private void OnSliderValueChanged(float value)
    {
        sliderAmount.text = value.ToString("0");
    }

    public override void OpenMenu<T>(GameObject interactedObject = null)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public override void CloseMenu<T>(GameObject interactedObject = null)
    {
        gameObject.SetActive(false);
    }

    public void AcceptButtonPressed()
    {
        Menu_RightClick.Instance.Drop<UI_Slider>(Mathf.RoundToInt(slider.value), equipmentSlot: equipmentSlot, inventorySlot: inventorySlot);
        gameObject.SetActive(false);
    }

    public void DropItemSlider(int currentStackSize, Equipment_Slot dropXEquipmentSlot = null, Inventory_Slot dropXInventorySlot = null)
    {
        if (dropXEquipmentSlot != null)
        {
            equipmentSlot = dropXEquipmentSlot;
        }
        else if (dropXInventorySlot != null)
        {
            inventorySlot = dropXInventorySlot;
        }

        sliderTitle.text = "How much would you like to drop?";

        slider.minValue = 0;
        slider.maxValue = currentStackSize;
        slider.value = 0;
    }
}
