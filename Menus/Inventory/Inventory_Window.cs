using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Inventory_Window : Menu_UI
{
    public static Inventory_Window Instance;

    public TextMeshProUGUI ActorNameText;
    public TextMeshProUGUI ChestNameText;

    public List<Inventory_Slot> InventorySlots = new();
    public Equipment_Window EquipmentPanel;

    [SerializeField] private Transform _actorInventoryPanel;
    [SerializeField] private Transform _chestInventoryPanel;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        gameObject.SetActive(false);
        _actorInventoryPanel.gameObject.SetActive(false);
        _chestInventoryPanel.gameObject.SetActive(false);
    }

    public void SetInventoryWindowName(string name, Inventory_Creator inventoryCreator)
    {
        if (inventoryCreator is Inventory_Creator_Actor)
        {
            ActorNameText.text = name;
        }
        else if (inventoryCreator is Inventory_Creator_Chest)
        {
            ChestNameText.text = name;
        }
        
    }

    public void OpenActorInventory()
    {
        _actorInventoryPanel.gameObject.SetActive(true);
        _chestInventoryPanel.gameObject.SetActive(false);
    }

    public void CloseActorInventory()
    {
        _actorInventoryPanel.gameObject.SetActive(false);
    }

    public void OpenChestInventory()
    {
        _chestInventoryPanel.gameObject.SetActive(true);
        _actorInventoryPanel.gameObject.SetActive(false);
    }

    public void CloseChestInventory()
    {
        _chestInventoryPanel.gameObject.SetActive(false);
    }
    public override void OpenMenu(GameObject inventorySourceGO)
    {
        if (_isOpen)
        {
            Manager_Menu.Instance.HandleEscapePressed(this, inventorySourceGO);
        }

        InteractedObject = inventorySourceGO;
        IInventory inventorySource = InteractedObject.GetComponent<IInventory>();

        if (inventorySource == null)
        {
            return;
        }
        
        Inventory_Creator inventoryCreator = null;

        if (inventorySource.InventoryType == InventoryType.Actor)
        {
            OpenActorInventory();
            inventoryCreator = GetComponentInChildren<Inventory_Creator_Actor>();
        }
        else if (inventorySource.InventoryType == InventoryType.Chest)
        {
            OpenChestInventory();
            inventoryCreator = GetComponentInChildren<Inventory_Creator_Chest>();
        }

        gameObject.SetActive(true);
        _isOpen = true;
        inventoryCreator.IsOpen = true;

        SetInventoryWindowName(inventorySource.GetIInventoryGO().name, inventoryCreator);
        Manager_Menu.Instance.SetWindowToFront(gameObject);

        if (inventoryCreator != null)
        {
            inventoryCreator.CreateSlots(inventorySource);
        }
        else { Debug.Log("Slot creator doesn't exist"); }

        if (inventorySourceGO.TryGetComponent<IEquipment>(out IEquipment equipmentSource))
        {
            EquipmentPanel.UpdateEquipmentUI(equipmentSource);
        }
    }
    
    public override void CloseMenu(GameObject inventorySourceGO)
    {
        IInventory inventorySource = inventorySourceGO.GetComponent<IInventory>();

        Inventory_Creator inventoryCreator = GetComponentInChildren<Inventory_Creator_Actor>() != null
            ? GetComponentInChildren<Inventory_Creator_Actor>()
            : GetComponentInChildren<Inventory_Creator_Chest>();

        if (inventoryCreator != null)
        {
            Inventory_Slot[] inventorySlots = inventoryCreator.GetComponentsInChildren<Inventory_Slot>();

            foreach (Inventory_Slot slot in inventorySlots)
            {
                Destroy(slot.gameObject);
            }

            inventoryCreator.IsOpen = false;
        }

        _isOpen = false;
        gameObject.SetActive(false);
    }

    public void RefreshInventoryUI(GameObject interactedObject = null)
    {
        if (interactedObject != null && InteractedObject != interactedObject)
        {
            InteractedObject = interactedObject;
        }
        else if (interactedObject == null)
        {
            InteractedObject = GameManager.Instance.Player.gameObject;
        }

        if (InteractedObject.TryGetComponent<Actor_Base>(out Actor_Base actor))
        {
            actor.ActorScripts.StatManager.UpdateStats();
        }

        Manager_Menu.Instance.HandleEscapePressed(this, InteractedObject);
        OpenMenu(InteractedObject);
    }
}
