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

    [SerializeField] private Transform _actorinventoryPanel;
    [SerializeField] private Transform _chestInventoryPanel;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        gameObject.SetActive(false);
        _actorinventoryPanel.gameObject.SetActive(false);
        _chestInventoryPanel.gameObject.SetActive(false);
    }

    public void SetInventoryWindowName(string name)
    {
        nameText.text = name;
    }

    public void OpenActorInventory()
    {
        _actorinventoryPanel.gameObject.SetActive(true);
        _chestInventoryPanel.gameObject.SetActive(false);
    }

    public void CloseActorInventory()
    {
        _actorinventoryPanel.gameObject.SetActive(false);
    }

    public void OpenChestInventory()
    {
        _chestInventoryPanel.gameObject.SetActive(true);
        _actorinventoryPanel.gameObject.SetActive(false);
    }

    public void CloseChestInventory()
    {
        _chestInventoryPanel.gameObject.SetActive(false);
    }
    public override void OpenMenu<T>(GameObject inventorySourceGO)
    {
        if (_isOpen)
        {
            Manager_Menu.Instance.HandleEscapePressed(this);
        }

        IInventory<T> inventorySource = inventorySourceGO.GetComponent<IInventory<T>>();

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

        SetInventoryWindowName(inventorySource.GetIInventoryBaseClass().name);
        Manager_Menu.Instance.SetWindowToFront(gameObject);

        if (inventoryCreator != null)
        {
            inventoryCreator.CreateSlots(inventorySource.GetInventorySize());
            inventoryCreator.UpdateInventoryUI(inventorySource);
        }
        else { Debug.Log("Slot creator doesn't exist"); }

        if (inventorySource.GetIInventoryBaseClass().TryGetComponent<IEquipment<T>>(out IEquipment<T> equipmentSource))
        {
            EquipmentPanel.UpdateEquipmentUI(equipmentSource);
        }
    }
    
    public override void CloseMenu<T>(GameObject inventorySourceGO)
    {
        IInventory<T> inventorySource = inventorySourceGO.GetComponent<IInventory<T>>();

        Inventory_Creator inventoryCreator = GetComponentInChildren<Inventory_Creator_Actor>().IsOpen
        ? GetComponentInChildren<Inventory_Creator_Actor>()
        : GetComponentInChildren<Inventory_Creator_Chest>().IsOpen
            ? GetComponentInChildren<Inventory_Creator_Chest>()
            : null;

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

    public void RefreshUI<T>(GameObject interactedObject = null) where T : MonoBehaviour
    {
        if (interactedObject == null)
        {
            interactedObject = GameManager.Instance.Player.gameObject;
        }

        Manager_Stats statManager = interactedObject.GetComponent<Actor_Base>().ActorScripts.StatManager;
        statManager.UpdateStats();

        Manager_Menu.Instance.HandleEscapePressed(this);
        OpenMenu<Inventory_Window>(interactedObject);

        if (interactedObject.TryGetComponent<IEquipment<T>>(out IEquipment<T> equipmentSource))
        {
            Equipment_Manager.UpdateSprites(equipmentSource);
        }
    }
}
