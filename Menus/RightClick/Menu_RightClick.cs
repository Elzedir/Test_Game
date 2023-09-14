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

    private GameObject _interactingThing;
    private GameObject _interactedThing;

    private Inventory_Slot _inventorySlot;
    private Equipment_Slot _equipmentSlot;
    private Actor_Base _actor;
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
        _buttonPickup = GetComponentInChildren<Button_PickupItem>();
        _buttonTalk = GetComponentInChildren<Button_Talk>();
        _buttonUnequip = GetComponentInChildren<Button_Unequip>();
        _buttonDropOne = GetComponentInChildren<Button_Drop_One>();
        _buttonDropX = GetComponentInChildren<Button_Drop_X>();
        _buttonDropAll = GetComponentInChildren<Button_Drop_All>();
        _buttonOpen = GetComponentInChildren<Button_Open>();

        CloseRightClickButtons(gameObject, true);
    }



    public void CloseRightClickButtons(GameObject obj, bool initialising = false)
    {
        if (obj == null)
        {
            return;
        }

        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
               CloseRightClickButtons(child.gameObject);
            }

            if (child.GetComponent<RightClickOption>())
            {
                child.gameObject.SetActive(false);
            }

            if (child.TryGetComponent<Button_Expand>(out Button_Expand buttonExpand))
            {
                buttonExpand.ExpandPanel.gameObject.SetActive(false);
            }
        }
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
        _interactingThing = null;
        _inventorySlot = null;
        _equipmentSlot = null;
        _actor = null;
        _item = null;
        CloseRightClickButtons(gameObject);
    }

    public void ActiveButtonsCheck(object interactingThing = null, 
                               object interactedThing = null,
                               List_Item item = null,
                               bool equippable = false,
                               bool itemEquipped = false,
                               bool droppable = false,
                               bool talkable = false,
                               bool openable = false)
    {
        position = Input.mousePosition;

        if (interactedThing is GameObject go)
        {
            _interactingThing = go;
        }
        else if (interactedThing is Actor_Base interactedActor)
        {
            _interactedThing = interactedActor.gameObject;
        }
        
        if (interactingThing is Actor_Base interactingActor)
        {
            _actor = interactingActor;
        }

        if (_interactingThing.TryGetComponent<Interactable_Item>(out Interactable_Item interactableItem))
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

        _inventorySlot = _interactingThing.GetComponent<Inventory_Slot>();
        _equipmentSlot = _interactingThing.GetComponent<Equipment_Slot>();
    }
    public void RightClickMenu(object interactingThing = null,
                           object interactedThing = null,
                           List_Item item = null,
                           bool equippable = false,
                           bool itemEquipped = false,
                           bool droppable = false,
                           bool talkable = false,
                           bool openable = false)
    {
        ActiveButtonsCheck(interactingThing, interactedThing, item, equippable, itemEquipped, droppable, talkable, openable);
        RightClickMenuOpen();
    }

    public void EquipButtonPressed()
    {
        int inventorySlotIndex = -1;

        if (_inventorySlot != null)
        {
            inventorySlotIndex = _inventorySlot.slotIndex;
        }

        bool equipped = Inventory_Manager.OnEquip(, interactedObject: _interactingThing, pickedUpItem: _item, inventorySlotIndex: inventorySlotIndex);
        if (equipped)
        {
            RightClickMenuClose();
        }
    }
    public void UnequipButtonPressed()
    {
        if (_equipmentSlot != null)
        {
            Equipment_Manager.Unequip(_equipmentSlot);
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
        Debug.Log(_interactingThing);

        bool pickedUp = Inventory_Manager.OnItemPickup(pickedUpItem: _item, inventorySlotIndex: inventorySlotIndex, interactedObject: _interactingThing);
        
        if (pickedUp && _item != null)
        {
            RightClickMenuClose();
        }
    }
    public void Drop(int dropAmount, Equipment_Slot equipmentSlot = null, Inventory_Slot inventorySlot = null, Actor_Base actor = null)
    {
        if (equipmentSlot != null)
        {
            Equipment_Manager.DropEquipment(equipmentSlot, dropAmount);
            Manager_Menu.Instance.InventoryMenu.RefreshUI(actor.gameObject, actor.GetComponent<Equipment_Manager>());
        }

        else if (inventorySlot != null)
        {
            Inventory_Manager.DropItem(inventorySlot.slotIndex, dropAmount, inventoryManager);
            Manager_Menu.Instance.InventoryMenu.RefreshUI(actor.gameObject, actor.GetComponent<Equipment_Manager>());
        }
    }
    public void DropOneButtonPressed()
    {
        Drop(1, equipmentSlot: _equipmentSlot, inventorySlot: _inventorySlot, actor: _actor);
    }
    public void DropXButtonPressed()
    {
        UI_Slider.instance.OpenMenu<Menu_RightClick>();

        if (_equipmentSlot != null)
        {
            UI_Slider.instance.DropItemSlider(Equipment_Manager.CurrentEquipment[_equipmentSlot].Item2, dropXEquipmentSlot: _equipmentSlot, dropXActor: _actor);
            RightClickMenuClose();

        }
        else if (_inventorySlot != null)
        {
            UI_Slider.instance.DropItemSlider(Inventory_Manager.InventoryItemIDs[_inventorySlot.slotIndex].Item2, dropXInventorySlot: _inventorySlot, dropXActor: _actor);
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
        if (_interactingThing.TryGetComponent<Chest>(out Chest chest))
        {
            chest.OpenChestInventory(chest.Inventory_NotEquippable);
            RightClickMenuClose();
        }
    }
}
