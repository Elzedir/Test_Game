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

    private GameObject _sourceGO;
    private GameObject _destinationGO;
    private Interactable_Item _interactedItem;

    private Inventory_Slot _inventorySlot;
    private Equipment_Slot _equipmentSlot;
    
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
        _destinationGO = null;
        _inventorySlot = null;
        _equipmentSlot = null;
        _sourceGO = null;
        _interactedItem = null;
        CloseRightClickButtons(gameObject);
    }

    public void ActiveButtonsCheck(GameObject objectSource = null,
                               GameObject objectDestination = null,
                               object slot = null,
                               List_Item item = null,
                               bool itemEquipped = false,
                               bool droppable = false,
                               bool talkable = false,
                               bool openable = false)
    {
        position = Input.mousePosition;

        _sourceGO = objectSource != null ? objectSource : GameManager.Instance.Player.gameObject;
        _destinationGO = objectDestination != null ? objectDestination : GameManager.Instance.Player.gameObject;

        if (_sourceGO != null && _sourceGO.TryGetComponent<Interactable_Item>(out Interactable_Item interactableItem))
        {
            _interactedItem = interactableItem;
        }

        if (item != null)
        {
            _buttonPickup.gameObject.SetActive(true);
            _buttonEquip.gameObject.SetActive(item.ItemStats.CommonStats.Equippable);
        }
        else if (_interactedItem != null)
        {
            _buttonPickup.gameObject.SetActive(true);
            _buttonEquip.gameObject.SetActive(List_Item.GetItemData(_interactedItem.ItemID).ItemStats.CommonStats.Equippable);
        }
        
        _buttonUnequip.gameObject.SetActive(itemEquipped);
        _buttonTalk.gameObject.SetActive(talkable);
        _buttonDropOne.gameObject.SetActive(droppable);
        _buttonDropX.gameObject.SetActive(droppable);
        _buttonDropAll.gameObject.SetActive(droppable);
        _buttonOpen.gameObject.SetActive(openable);

        if (slot != null)
        {
            if (slot is Inventory_Slot inventorySlot)
            {
                _inventorySlot = inventorySlot;
            }
            else if (slot is Equipment_Slot equipmentSlot)
            {
                _equipmentSlot = equipmentSlot;
            }
        }
    }
    public void RightClickMenu(GameObject objectSource = null,
                           GameObject objectDestination = null,
                           object slot = null,
                           List_Item item = null,
                           bool itemEquipped = false,
                           bool droppable = false,
                           bool talkable = false,
                           bool openable = false)
    {
        ActiveButtonsCheck(objectSource, objectDestination, slot, item, itemEquipped, droppable, talkable, openable);
        RightClickMenuOpen();
    }

    public void EquipButtonPressed<T>() where T : MonoBehaviour
    {
        int inventorySlotIndex = -1;

        if (_inventorySlot != null)
        {
            inventorySlotIndex = _inventorySlot.slotIndex;
        }

        Debug.Log(_sourceGO);
        Debug.Log(_destinationGO);

        IInventory<T> inventorySource = _sourceGO.GetComponent<IInventory<T>>();
        IInventory<T> inventoryDestination = _destinationGO.GetComponent<IInventory<T>>();
        // It works in Inventory Window but not here. Try debug the whole class.

        Debug.Log(_sourceGO);
        Debug.Log(_destinationGO);
        Debug.Log(inventorySource);
        Debug.Log(inventoryDestination);

        bool equipped = Inventory_Manager.OnEquip(itemSource: inventorySource, itemDestination: inventoryDestination, pickedUpItem: _interactedItem, inventorySlotIndex: inventorySlotIndex);

        if (equipped)
        {
            RightClickMenuClose();
        }
    }
    public void UnequipButtonPressed<T>() where T : MonoBehaviour
    {
        if (_equipmentSlot != null)
        {
            IEquipment<T> equipmentSource = _sourceGO.GetComponent<IEquipment<T>>();

            Equipment_Manager.UnequipEquipment(equipmentSource: equipmentSource, equipSlot: _equipmentSlot);
            RightClickMenuClose();
        }
    }
    public void PickupButtonPressed<T>() where T : MonoBehaviour
    {
        int inventorySlotIndex = -1;

        if (_inventorySlot != null)
        {
            inventorySlotIndex = _inventorySlot.slotIndex;
        }

        IInventory<T> inventorySource = _sourceGO.GetComponent<IInventory<T>>();
        IInventory<T> inventoryDestination = _destinationGO.GetComponent<IInventory<T>>();

        bool pickedUp = Inventory_Manager.OnItemPickup(itemSource: inventorySource, itemDestination: inventoryDestination, pickedUpItem: _interactedItem, inventorySlotIndex: inventorySlotIndex);
        
        if (pickedUp && _interactedItem != null)
        {
            RightClickMenuClose();
        }
    }
    public void Drop<T>(int dropAmount, Equipment_Slot equipmentSlot = null, Inventory_Slot inventorySlot = null) where T : MonoBehaviour
    {
        if (equipmentSlot != null)
        {
            IEquipment<T> equipmentSource = _sourceGO.GetComponent<IEquipment<T>>();
            Equipment_Manager.DropEquipment(equipmentSource, equipmentSlot, dropAmount);
            Manager_Menu.Instance.InventoryMenu.RefreshUI<Menu_RightClick>(_sourceGO);
        }

        else if (inventorySlot != null)
        {
            IInventory<T> inventorySource = _sourceGO.GetComponent<IInventory<T>>();
            Inventory_Manager.DropItem(inventorySource, inventorySlot.slotIndex, dropAmount);
            Manager_Menu.Instance.InventoryMenu.RefreshUI<Menu_RightClick>(_sourceGO);
        }
    }
    public void DropOneButtonPressed()
    {
        Drop<Menu_RightClick>(1, equipmentSlot: _equipmentSlot, inventorySlot: _inventorySlot);
    }
    public void DropXButtonPressed<T>() where T : MonoBehaviour
    {
        UI_Slider.instance.OpenMenu<Menu_RightClick>();

        if (_equipmentSlot != null)
        {
            UI_Slider.instance.DropItemSlider(_sourceGO.GetComponent<IEquipment<T>>().GetEquipmentData().EquipmentItems[_equipmentSlot.SlotIndex].StackSize, dropXEquipmentSlot: _equipmentSlot);
            RightClickMenuClose();

        }
        else if (_inventorySlot != null)
        {
            UI_Slider.instance.DropItemSlider(_sourceGO.GetComponent<IInventory<T>>().GetInventoryData().InventoryItems[_inventorySlot.slotIndex].StackSize, dropXInventorySlot: _inventorySlot);
            RightClickMenuClose();
        }
    }
    public void DropAllButtonPressed()
    {
        Drop<Menu_RightClick>(-1, equipmentSlot: _equipmentSlot, inventorySlot: _inventorySlot);
    }
    public void TalkButtonPressed()
    {
        if (_destinationGO != null)
        {
            Dialogue_Manager.instance.OpenDialogue(_destinationGO, _destinationGO.GetComponent<Actor_Base>().DialogueData);
            RightClickMenuClose();
        }

        else { Debug.Log("Interacted character does not exist"); }
    }
    public void OpenButtonPressed()
    {
        if (_destinationGO.TryGetComponent<Chest>(out Chest chest))
        {
            chest.OpenChestInventory();
            RightClickMenuClose();
        }
    }
}
