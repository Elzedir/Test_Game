using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Menu_RightClick : MonoBehaviour
{
    public static Menu_RightClick Instance;
    public Manager_Input InputManager;
    public Equipment_Window EquipmentWindow;

    private GameObject _thing;

    private Inventory_Slot _inventorySlot;
    private Equipment_Slot _equipmentSlot;
    private Actor_Base _actor;
    private Inventory_Manager _inventoryManager;
    private Interactable_Item _item;

    private Button_Equip _buttonEquip;
    private Button_PickupItem _buttonPickup;
    private Button_Talk _buttonTalk;
    private Button_Open _buttonOpen;
    private Button_Unequip _buttonUnequip;
    private Button_Drop_One _buttonDropOne;
    private Button_Drop_X _buttonDropX;
    private Button_Drop_All _buttonDropAll;

    private Vector3 position;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        gameObject.SetActive(false);
        _buttonEquip = GetComponentInChildren<Button_Equip>();
        _buttonEquip.gameObject.SetActive(false);
        _buttonPickup = GetComponentInChildren<Button_PickupItem>();
        _buttonPickup.gameObject.SetActive(false);
        _buttonTalk = GetComponentInChildren<Button_Talk>();
        _buttonTalk.gameObject.SetActive(false);
        _buttonUnequip = GetComponentInChildren<Button_Unequip>();
        _buttonUnequip.gameObject.SetActive(false);
        _buttonDropOne = GetComponentInChildren<Button_Drop_One>();
        _buttonDropOne.gameObject.SetActive(false);
        _buttonDropX = GetComponentInChildren<Button_Drop_X>();
        _buttonDropX.gameObject.SetActive(false);
        _buttonDropAll = GetComponentInChildren<Button_Drop_All>();
        _buttonDropAll.gameObject.SetActive(false);
        _buttonOpen = GetComponentInChildren<Button_Open>();
        _buttonOpen.gameObject.SetActive(false);
    }

    public void RightClickMenuOpen()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Vector3 newPosition = new Vector3(position.x - 27.5f, position.y + 27.5f, position.z);
        transform.position = newPosition;
    }
    public void RightClickMenuClose()
    {
        gameObject.SetActive(false);
        _thing = null;
        _inventorySlot = null;
        _equipmentSlot = null;
        _actor = null;
        _inventoryManager = null;
        _item = null;
        _buttonEquip.gameObject.SetActive(false);
        _buttonPickup.gameObject.SetActive(false);
        _buttonTalk.gameObject.SetActive(false);
        _buttonUnequip.gameObject.SetActive(false);
        _buttonDropOne.gameObject.SetActive(false);
        _buttonDropX.gameObject.SetActive(false);
        _buttonDropAll.gameObject.SetActive(false);
        _buttonOpen.gameObject.SetActive(false);
    }

    public void ActiveButtonsCheck(GameObject interactedThing = null,
                               Actor_Base actor = null,
                               Inventory_Manager inventoryManager = null,
                               List_Item item = null,
                               bool equippable = false,
                               bool itemEquipped = false,
                               bool droppable = false,
                               bool talkable = false,
                               bool openable = false)
    {
        position = Input.mousePosition;

        _thing = interactedThing;
        _inventoryManager = inventoryManager;

        if (_thing.TryGetComponent<Interactable_Item>(out Interactable_Item interactableItem))
        {
            _item = interactableItem;
        }

        if (item != null || _item != null)
        {
            _buttonPickup.gameObject.SetActive(true);
        }
        
        _buttonEquip.gameObject.SetActive(equippable);
        _buttonUnequip.gameObject.SetActive(itemEquipped);
        _buttonTalk.gameObject.SetActive(talkable);
        _buttonDropOne.gameObject.SetActive(droppable);
        _buttonDropX.gameObject.SetActive(droppable);
        _buttonDropAll.gameObject.SetActive(droppable);
        _buttonOpen.gameObject.SetActive(openable);

        _inventorySlot = _thing.GetComponent<Inventory_Slot>();
        _equipmentSlot = _thing.GetComponent<Equipment_Slot>();
        _actor = actor;
    }
    public void RightClickMenu(GameObject interactedThing = null,
                           Actor_Base actor = null,
                           Inventory_Manager inventoryManager = null,
                           List_Item item = null,
                           bool equippable = false,
                           bool itemEquipped = false,
                           bool droppable = false,
                           bool talkable = false,
                           bool openable = false)
    {
        ActiveButtonsCheck(interactedThing, actor, inventoryManager, item, equippable, itemEquipped, droppable, talkable, openable);
        RightClickMenuOpen();
    }

    public void EquipButtonPressed()
    {
        int inventorySlotIndex = -1;

        if (_inventorySlot != null)
        {
            inventorySlotIndex = _inventorySlot.slotIndex;
        }

        bool equipped = _inventoryManager.OnEquip(pickedUpItem: _item, inventorySlotIndex: inventorySlotIndex, interactedObject: _thing);
        if (equipped)
        {
            RightClickMenuClose();
        }
    }
    public void UnequipButtonPressed()
    {
        if (_equipmentSlot != null)
        {
            Equipment_Manager equipmentManager = _actor.GetComponent<Equipment_Manager>();
            equipmentManager.Unequip(_equipmentSlot);
            RightClickMenuClose();
        }
    }
    public void PickupButtonPressed()
    {
        int inventorySlotIndex = -1;

        if (_inventorySlot != null)
        {
            inventorySlotIndex = _inventorySlot.slotIndex;
        }
        Debug.Log(_item);
        Debug.Log(inventorySlotIndex);
        Debug.Log(_thing);
        Debug.Log(_inventoryManager);

        bool pickedUp = _inventoryManager.OnItemPickup(pickedUpItem: _item, inventorySlotIndex: inventorySlotIndex, interactedObject: _thing);
        
        if (pickedUp && _item != null)
        {
            RightClickMenuClose();
        }
    }
    public void Drop(int dropAmount, Equipment_Slot equipmentSlot = null, Inventory_Slot inventorySlot = null, Actor_Base actor = null)
    {
        if (equipmentSlot != null)
        {
            Equipment_Manager equipmentManager = actor.GetComponent<Equipment_Manager>();
            equipmentManager.DropItem(equipmentSlot, dropAmount);
            Manager_Menu.Instance.InventoryMenu.RefreshPlayerUI(actor.gameObject, actor.GetComponent<Equipment_Manager>());
        }

        else if (inventorySlot != null)
        {
            Inventory_Manager inventoryManager = actor.GetComponent<Inventory_Manager>();
            inventoryManager.DropItem(inventorySlot.slotIndex, dropAmount, inventoryManager);
            Manager_Menu.Instance.InventoryMenu.RefreshPlayerUI(actor.gameObject, actor.GetComponent<Equipment_Manager>());
        }
    }
    public void DropOneButtonPressed()
    {
        Drop(1, equipmentSlot: _equipmentSlot, inventorySlot: _inventorySlot, actor: _actor);
    }
    public void DropXButtonPressed()
    {
        UI_Slider.instance.OpenMenu();

        if (_equipmentSlot != null)
        {
            Equipment_Manager equipmentManager = _actor.GetComponent<Equipment_Manager>();
            UI_Slider.instance.DropItemSlider(equipmentManager.currentEquipment[_equipmentSlot].Item2, dropXEquipmentSlot: _equipmentSlot, dropXActor: _actor);
            RightClickMenuClose();

        }
        else if (_inventorySlot != null)
        {
            Inventory_Manager inventoryManager = _actor.GetComponent<Inventory_Manager>();
            UI_Slider.instance.DropItemSlider(inventoryManager.InventoryItemIDs[_inventorySlot.slotIndex].Item2, dropXInventorySlot: _inventorySlot, dropXActor: _actor);
            RightClickMenuClose();
        }
    }
    public void DropAllButtonPressed()
    {
        Drop(-1, equipmentSlot: _equipmentSlot, inventorySlot: _inventorySlot, actor: _actor);
    }
    public void TalkButtonPressed()
    {
        if (_actor != null)
        {
            Dialogue_Manager.instance.OpenDialogue(_actor.gameObject, _actor.DialogueData);
            RightClickMenuClose();
        }
        else
        {
            Debug.Log("Interacted character does not exist");
        }
    }
    public void OpenButtonPressed()
    {
        if (_thing.TryGetComponent<Chest>(out Chest chest))
        {
            chest.OpenChestInventory(chest.Inventory_NotEquippable);
            RightClickMenuClose();
        }
    }
}
