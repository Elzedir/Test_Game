using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : MonoBehaviour
{
    public static UI_Slider instance;
    public Slider slider;
    public TextMeshProUGUI sliderTitle;
    public TextMeshProUGUI sliderAmount;
    public Equipment_Slot equipmentSlot;
    public Inventory_Slot inventorySlot;
    public Actor actor;

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

    public void OpenSlider()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void AcceptButtonPressed()
    {
        Menu_RightClick.instance.Drop(Mathf.RoundToInt(slider.value), equipmentSlot: equipmentSlot, inventorySlot: inventorySlot, actor: actor);
        gameObject.SetActive(false);
    }

    public void ReturnButtonPressed()
    {
        gameObject.SetActive(false);
    }

    public void DropItemSlider(int currentStackSize, Equipment_Slot dropXEquipmentSlot = null, Inventory_Slot dropXInventorySlot = null, Actor dropXActor = null)
    {
        if (dropXActor != null)
        {
            actor = dropXActor;
        }
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
