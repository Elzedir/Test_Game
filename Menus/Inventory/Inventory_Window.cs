using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Inventory_Window : Menu_UI
{
    public static Inventory_Window Instance;

    public TextMeshProUGUI nameText;
    public List<Inventory_Slot> InventorySlots = new();
    public Equipment_Window EquipmentPanel;

    public GameObject InventorySource;

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

    public void SetInventoryWindowName(string name)
    {
        nameText.text = name;
    }

    public void OpenActorInventory()
    {
        _actorInventoryPanel.gameObject.SetActive(true);
        _chestInventoryPanel.gameObject.SetActive(false);
    }

    public void CloseActorInventory()
    {
        InventorySource = null;
        _actorInventoryPanel.gameObject.SetActive(false);
    }

    public void OpenChestInventory()
    {
        _chestInventoryPanel.gameObject.SetActive(true);
        _actorInventoryPanel.gameObject.SetActive(false);
    }

    public void CloseChestInventory()
    {

        InventorySource = null;
        _chestInventoryPanel.gameObject.SetActive(false);
    }
    public override void OpenMenu(GameObject inventorySourceGO)
    {
        InteractedObject = inventorySourceGO;

        if (_isOpen)
        {
            Manager_Menu.Instance.HandleEscapePressed(this);
        }
        InventorySource = inventorySourceGO;
        IInventory inventorySource = InventorySource.GetComponent<IInventory>();

        if (inventorySource == null)
        {
            return;
        }

        if (inventorySource.InventoryIsOpen)
        {
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
            {
                Manager_Menu.Instance.HandleEscapePressed(this);
            }
            else
            {
                Manager_Menu.Instance.SetWindowToFront(gameObject);
            }
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
        inventorySource.InventoryIsOpen = true;
        inventoryCreator.IsOpen = true;

        SetInventoryWindowName(inventorySource.GetIInventoryGO().name);
        Manager_Menu.Instance.SetWindowToFront(gameObject);

        if (inventoryCreator != null)
        {
            inventoryCreator.CreateSlots(inventorySource.GetInventorySize());
            inventoryCreator.UpdateInventoryUI(inventorySource);
        }
        else { Debug.Log("Slot creator doesn't exist"); }

        if (inventorySource.GetIInventoryGO().TryGetComponent<IEquipment>(out IEquipment equipmentSource))
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
            inventorySource.InventoryIsOpen = false;
        }

        _isOpen = false;
        gameObject.SetActive(false);
    }

    public void RefreshInventoryUI(GameObject interactedObject = null)
    {
        if (interactedObject != null)
        {
            InteractedObject = interactedObject;
        }
        else
        {
            InteractedObject = GameManager.Instance.Player.gameObject;
        }

        if (InteractedObject.TryGetComponent<Actor_Base>(out Actor_Base actor))
        {
            actor.ActorScripts.StatManager.UpdateStats();
        }
          
        Inventory_Creator slotCreator = Manager_Menu.Instance.InventoryMenu.GetComponentInChildren<Inventory_Creator>();
        slotCreator.UpdateInventoryUI(GameManager.Instance.Player.PlayerActor);

        Manager_Menu.Instance.HandleEscapePressed(this, InteractedObject);
        OpenMenu(InteractedObject);

        if (InteractedObject.TryGetComponent<IEquipment>(out IEquipment equipmentSource))
        {
            Equipment_Manager.UpdateSprites(equipmentSource);
        }
    }
}
