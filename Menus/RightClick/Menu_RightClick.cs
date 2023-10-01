using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Menu_RightClick : MonoBehaviour
{
    public static Menu_RightClick Instance;

    private bool _isOpen = false;
    private RectTransform _buttonsPanel;

    private IInventory _inventoryInteracted;
    private Inventory_Slot _inventorySlot;
    private IInventory _inventoryDestination;
    private IEquipment _equipmentInteracted;
    private Equipment_Slot _equipmentSlot;
    private IEquipment _equipmentDestination;
    private Actor_Base _actorInteracted;
    private Interactable_Item _itemInteractable;
    
    private Button_Equip _buttonEquip;
    private Button_PickupItem _buttonPickup;
    private Button_Take _buttonTake;
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

        _buttonsPanel = GameObject.Find("RightClickButtonsPanel").GetComponent<RectTransform>();
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
        _buttonTake = GetComponentInChildren<Button_Take>();

        CloseRightClickButtons(gameObject, true);
    }

    public void Update()
    {
        if (_isOpen && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition) && !RectTransformUtility.RectangleContainsScreenPoint(_buttonsPanel, Input.mousePosition))
        {
            RightClickMenuClose();
            return;
        }
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
        _isOpen = true;

        Vector3 newPosition = new Vector3(position.x - 27.5f, position.y + 27.5f, position.z);
        transform.position = position;
        _buttonsPanel.position = newPosition;
    }


    public void ActiveButtonsCheck(List_Item item = null,
                               bool itemEquipped = false,
                               bool itemInteractable = false,
                               bool itemTakeable = false,
                               bool droppable = false,
                               bool talkable = false,
                               bool openable = false)
    {
        position = Input.mousePosition;

        if (item != null)
        {
            _buttonEquip.gameObject.SetActive(item.ItemStats.CommonStats.ItemEquippable);
        }
        else if (_itemInteractable != null)
        {
            _buttonEquip.gameObject.SetActive(List_Item.GetItemData(_itemInteractable.ItemID).ItemStats.CommonStats.ItemEquippable);
        }

        _buttonPickup.gameObject.SetActive(itemInteractable);
        _buttonTake.gameObject.SetActive(itemTakeable);
        _buttonUnequip.gameObject.SetActive(itemEquipped);
        _buttonTalk.gameObject.SetActive(talkable);
        _buttonDropOne.gameObject.SetActive(droppable);
        _buttonDropX.gameObject.SetActive(droppable);
        _buttonDropAll.gameObject.SetActive(droppable);
        _buttonOpen.gameObject.SetActive(openable);

        RightClickMenuOpen();
    }
    public void Actor(Actor_Base actor)
    {
        _actorInteracted = actor;
        ActiveButtonsCheck(talkable: true);
    }
    public void Chest(IInventory chest)
    {
        _inventoryInteracted = chest;
        ActiveButtonsCheck(openable: true);
    }
    public void InteractableItem(Interactable_Item interactableItem)
    {
        _itemInteractable = interactableItem;
        _inventoryDestination = GameManager.Instance.Player.PlayerActor as IInventory;
        _equipmentDestination = GameManager.Instance.Player.PlayerActor as IEquipment;
        ActiveButtonsCheck(itemInteractable: true);
    }
    public void SlotInventory(IInventory inventorySource, Inventory_Slot inventorySlot)
    {
        List_Item item = null;
        bool droppable = false;
        bool takeable = false;

        if (inventorySlot.InventoryItem == null)
        {
            return;
        }

        if (inventorySlot.InventoryItem.ItemID > 0)
        {
            item = List_Item.GetItemData(inventorySlot.InventoryItem.ItemID);
            droppable = true;
            takeable = true;
        }

        if (inventorySource.GetIInventoryGO() == GameManager.Instance.Player.PlayerActor.gameObject)
        {
            _inventoryDestination = _inventoryInteracted != null ? _inventoryInteracted : GameManager.Instance.Player.PlayerActor;
            _inventoryInteracted = inventorySource;
            _inventorySlot = inventorySlot;
        }
        else
        {
            _inventoryDestination = GameManager.Instance.Player.PlayerActor as IInventory;
            _inventorySlot = inventorySlot;
        }

        ActiveButtonsCheck(item: item, droppable: droppable, itemTakeable: takeable);
    }

    public void SlotEquipment(IEquipment equipmentSource, Equipment_Slot equipmentSlot, List_Item item, bool itemEquipped)
    {
        _equipmentSlot = equipmentSlot;

        if (equipmentSource.GetIEquipmentGO() == GameManager.Instance.Player.PlayerActor.gameObject)
        {
            _equipmentDestination = _equipmentInteracted;
            _equipmentInteracted = equipmentSource;
        }
        else
        {
            _equipmentDestination = GameManager.Instance.Player.PlayerActor as IEquipment;
        }

        ActiveButtonsCheck(item: item, itemEquipped: itemEquipped, droppable: itemEquipped, itemTakeable: itemEquipped);
    }

    public void RightClickMenuExit()
    {
        _inventoryInteracted = null;
        _equipmentInteracted = null;
        _inventorySlot = null;
        _equipmentSlot = null;
        _isOpen = false;
        gameObject.SetActive(false);
        CloseRightClickButtons(gameObject);
    }
    public void EquipButtonPressed()
    {
        bool equipped = false;

        if (_inventorySlot != null)
        {
            equipped = Inventory_Manager.OnEquipFromInventory(itemSource: _inventoryInteracted, itemDestination: _inventoryDestination, inventorySlotIndex: _inventorySlot.slotIndex);
        }
        else if (_itemInteractable != null)
        {
            equipped = Inventory_Manager.OnEquipFromGround(itemDestination: _inventoryDestination, pickedUpItem: _itemInteractable);
        }

        if (equipped)
        {
            RightClickMenuExit();
        }
    }
    public void UnequipButtonPressed()
    {
        if (_equipmentSlot != null)
        {
            Debug.Log("1");
            Equipment_Manager.UnequipEquipment(equipmentSource: _equipmentInteracted, equipSlot: _equipmentSlot);
            RightClickMenuExit();
        }
    }
    public void PickupButtonPressed()
    {
        bool pickedUp = Inventory_Manager.OnItemPickup(itemDestination: GameManager.Instance.Player.PlayerActor, pickedUpItem: _itemInteractable);
        
        if (pickedUp)
        {
            RightClickMenuExit();
        }
    }

    public void TakeButtonPressed()
    {
        bool taken = Inventory_Manager.OnItemTake(itemSource: _inventoryInteracted, itemDestination: _inventoryDestination, inventorySlotIndex: _inventorySlot.slotIndex);

        if (taken)
        {
            RightClickMenuExit();
        }
    }
    public void Drop(int dropAmount, Equipment_Slot equipmentSlot = null, Inventory_Slot inventorySlot = null)
    {
        if (equipmentSlot != null)
        {
            Equipment_Manager.DropEquipment(_equipmentInteracted, equipmentSlot, dropAmount);
            Manager_Menu.Instance.InventoryMenu.RefreshInventoryUI(_equipmentInteracted.GetIEquipmentGO());
        }

        else if (inventorySlot != null)
        {
            Inventory_Manager.DropItem(_inventoryInteracted, inventorySlot.slotIndex, dropAmount);
            Manager_Menu.Instance.InventoryMenu.RefreshInventoryUI(_inventoryInteracted.GetIInventoryGO());
        }
    }
    public void DropOneButtonPressed()
    {
        Drop(1, equipmentSlot: _equipmentSlot, inventorySlot: _inventorySlot);
    }
    public void DropXButtonPressed()
    {
        UI_Slider.Instance.OpenMenu();

        if (_equipmentSlot != null)
        {
            UI_Slider.Instance.DropItemSlider(_equipmentInteracted.GetEquipmentData().EquipmentItems[_equipmentSlot.SlotIndex].StackSize, dropXEquipmentSlot: _equipmentSlot);
            RightClickMenuExit();

        }
        else if (_inventorySlot != null)
        {
            UI_Slider.Instance.DropItemSlider(_inventoryInteracted.GetInventoryData().InventoryItems[_inventorySlot.slotIndex].StackSize, dropXInventorySlot: _inventorySlot);
            RightClickMenuExit();
        }
    }
    public void DropAllButtonPressed()
    {
        Drop(-1, equipmentSlot: _equipmentSlot, inventorySlot: _inventorySlot);
    }
    public void TalkButtonPressed()
    {
        if (_actorInteracted != null)
        {
            Dialogue_Manager.instance.OpenDialogue(_actorInteracted.gameObject, _actorInteracted.DialogueData);
            RightClickMenuExit();
        }

        else { Debug.Log("Interacted character does not exist"); }
    }

    public void RightClickMenuClose()
    {
        _isOpen = false;
        gameObject.SetActive(false);
        CloseRightClickButtons(gameObject);
    }
    public void OpenButtonPressed()
    {
        Inventory_Window.Instance.OpenMenu(_inventoryInteracted.GetIInventoryGO());
        RightClickMenuClose();
    }
}
