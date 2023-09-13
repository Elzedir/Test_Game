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

    public void OpenMenu<T>(IInventory<T> inventorySource = null) where T : MonoBehaviour
    {
        if (_isOpen)
        {
            Manager_Menu.Instance.HandleEscapePressed(this);
        }

        if (!inventorySource.InventoryisOpen)
        {
            gameObject.SetActive(true);
            Inventory_Creator inventoryCreator = null;

            if (inventorySource.InventoryType == InventoryType.Actor)
            {
                OpenInventoryEquippable();
                inventoryCreator = InventoryEquippableCreator.GetComponent<Inventory_Creator>();
            }
            else if (inventorySource.InventoryType == InventoryType.Chest)
            {
                OpenInventoryNotEquippable();
                inventoryCreator = InventoryNotEquippableCreator.GetComponent<Inventory_Creator>();
            }

            _isOpen = true;
            inventorySource.InventoryisOpen = true;
            inventoryCreator.IsOpen = true;

            SetInventoryWindowName(inventorySource.GetIInventoryBaseClass().name);
            Manager_Menu.Instance.SetWindowToFront(gameObject);

            if (inventoryCreator != null)
            {
                inventoryCreator.CreateSlots(inventorySource.GetInventorySize());
                inventoryCreator.UpdateInventoryUI(inventorySource);

            }
            else { Debug.Log("Slot creator doesn't exist"); }

            if (inventorySource.GetIInventoryBaseClass().TryGetComponent(out Equipmnt_Manager equipmentManager))
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

        if (inventoryCreator != null)
        {
            Inventory_Slot[] inventorySlots = inventoryCreator.GetComponentsInChildren<Inventory_Slot>();

            foreach (Inventory_Slot slot in inventorySlots)
            {
                Destroy(slot.gameObject);
            }

            inventoryCreator.IsOpen = false;
            inventoryCreator.TempInventoryManager.IsOpen = false;
            inventoryCreator.TempInventoryManager = null;
        }

        _isOpen = false;
        gameObject.SetActive(false);
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
