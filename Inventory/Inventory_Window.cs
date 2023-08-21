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
    public  List<Inventory_Slot> inventorySlots = new();
    public Equipment_Window EquipmentPanel;
    public Transform InventoryEquippableCreator;
    public Transform InventoryNotEquippableCreator;

    [SerializeField] private Transform _inventoryEquippablePanel;
    [SerializeField] private Transform _inventoryNotEquippablePanel;

    public void Awake()
    {
        Instance = this;
    }

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

    public Inventory_Creator GetInventoryCreator(Inventory_Manager inventoryManager)
    {
        Inventory_Creator inventoryCreator = null;

        if (inventoryManager is Inventory_Equippable)
        {
            inventoryCreator = InventoryEquippableCreator.GetComponent<Inventory_Creator>();
        }
        else if (inventoryManager is Inventory_NotEquippable)
        {
            inventoryCreator = InventoryNotEquippableCreator.GetComponent<Inventory_Creator>();
        }

        return inventoryCreator;
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

    public override void OpenMenu(GameObject interactedObject = null)
    {
        if (_isOpen)
        {
            Manager_Menu.Instance.HandleEscapePressed(this);
        }

        Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);

        if (!inventoryManager.IsOpen)
        {
            if (!inventoryManager.InventoryIsInitialised)
            {
                inventoryManager.InitialiseInventory();
            }

            Open(interactedObject, inventoryManager);
            Inventory_Creator inventoryCreator = GetInventoryCreator(inventoryManager);

            inventoryManager.IsOpen = true;
            inventoryCreator.IsOpen = true;

            Manager_Menu.Instance.SetWindowToFront(gameObject);

            if (inventoryCreator != null)
            {
                int inventorySize = inventoryManager.GetComponent<Inventory_Manager>().GetInventorySize();
                inventoryCreator.CreateSlots(inventorySize);
                inventoryCreator.UpdateInventoryUI(inventoryManager);

            }
            else
            {
                Debug.Log("Slot creator doesn't exist");
            }

            if (interactedObject.TryGetComponent(out Equipment_Manager equipmentManager))
            {
                EquipmentPanel.UpdateEquipmentUI(equipmentManager);
            }
        }
        else
        {
            int childCount = transform.parent.childCount;

            if (transform.GetSiblingIndex() == childCount - 1)
            {
                Manager_Menu.Instance.HandleEscapePressed(this);
            }
            else
            {
                Manager_Menu.Instance.SetWindowToFront(gameObject);
            }
        }
    }
    public override void CloseMenu()
    {
        Inventory_Creator inventoryCreator = null;

        if (InventoryEquippableCreator.GetComponent<Inventory_Creator>().IsOpen)
        {
            inventoryCreator = InventoryEquippableCreator.GetComponent<Inventory_Creator>();
        }
        else if (InventoryNotEquippableCreator.GetComponent<Inventory_Creator>().IsOpen)
        {
            inventoryCreator = InventoryNotEquippableCreator.GetComponent<Inventory_Creator>();
        }

        Inventory_Slot[] inventorySlots = inventoryCreator.GetComponentsInChildren<Inventory_Slot>();

        foreach (Inventory_Slot slot in inventorySlots)
        {
            Destroy(slot.gameObject);
        }
        _isOpen = false;
        inventoryCreator.IsOpen = false;
        gameObject.SetActive(false);
        inventoryCreator.TempInventoryManager.IsOpen = false;
        inventoryCreator.TempInventoryManager = null;
    }

    public void RefreshPlayerUI(GameObject interactedObject, Equipment_Manager actorEquipmentManager = null)
    {
        Manager_Stats statManager = GameManager.Instance.Player.GetComponent<Manager_Stats>();
        statManager.UpdateStats();

        Manager_Menu.Instance.HandleEscapePressed(this);
        OpenMenu(interactedObject);

        if (actorEquipmentManager != null)
        {
            actorEquipmentManager.UpdateSprites();
        }
    }
}
