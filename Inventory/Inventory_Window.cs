using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Inventory_Window : MonoBehaviour
{
    public static Inventory_Window Instance;
    public TextMeshProUGUI nameText;
    public  List<Inventory_Slot> inventorySlots = new();
    private bool _isOpen = false;

    public bool IsOpen
    {
        get { return _isOpen; }
        set { _isOpen = value; }
    }

    [SerializeField] private Transform _inventoryEquippablePanel;
    [SerializeField] private Transform _inventoryNotEquippablePanel;

    public void Start()
    {
        gameObject.SetActive(false);
        _inventoryEquippablePanel.gameObject.SetActive(false);
        _inventoryNotEquippablePanel.gameObject.SetActive(false);
    }

    public void Open(GameObject interactedObject, Inventory_Manager inventoryManager)
    {
        gameObject.SetActive(true);

        if (inventoryManager is Inventory_Equippable)
        {
            OpenInventoryEquippable();
        }
        else if (inventoryManager is Inventory_NotEquippable)
        {
            OpenInventoryNotEquippable();
        }

        _isOpen = true;
        SetInventoryWindowName(interactedObject.name);
    }

    public List<Inventory_Slot> InventorySlots
    {
        get { return inventorySlots; }
        set { inventorySlots = value; }
    }

    public void SetInventoryWindowName(string name)
    {
        nameText.text = name;
    }

    public void OpenInventoryEquippable()
    {
        _inventoryEquippablePanel.gameObject.SetActive(true);
        _inventoryNotEquippablePanel.gameObject.SetActive(false);
    }

    public void CloseInventoryEquippable()
    {
        _inventoryEquippablePanel.gameObject.SetActive(false);
    }

    public void OpenInventoryNotEquippable()
    {
        _inventoryNotEquippablePanel.gameObject.SetActive(true);
        _inventoryEquippablePanel.gameObject.SetActive(false);
    }

    public void CloseInventoryNotEquippable()
    {
        _inventoryNotEquippablePanel.gameObject.SetActive(false);
    }
}
