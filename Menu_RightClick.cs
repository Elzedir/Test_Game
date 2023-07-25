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

    private Inventory_Slot pressedInventorySlot;
    private Equipment_Slot pressedEquipmentSlot;
    private Actor pressedActor;
    private Interactable_Item pickedUpItem;

    private Button_Equip buttonEquip;
    private Button_PickupItem buttonPickup;
    private Button_Talk buttonTalk;
    private Button_Open buttonOpen;
    private Button_Unequip buttonUnequip;
    private Button_Drop_One buttonDropOne;
    private Button_Drop_X buttonDropX;
    private Button_Drop_All buttonDropAll;

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
        buttonEquip = GetComponentInChildren<Button_Equip>();
        buttonEquip.gameObject.SetActive(false);
        buttonPickup = GetComponentInChildren<Button_PickupItem>();
        buttonPickup.gameObject.SetActive(false);
        buttonTalk = GetComponentInChildren<Button_Talk>();
        buttonTalk.gameObject.SetActive(false);
        buttonUnequip = GetComponentInChildren<Button_Unequip>();
        buttonUnequip.gameObject.SetActive(false);
        buttonDropOne = GetComponentInChildren<Button_Drop_One>();
        buttonDropOne.gameObject.SetActive(false);
        buttonDropX = GetComponentInChildren<Button_Drop_X>();
        buttonDropX.gameObject.SetActive(false);
        buttonDropAll = GetComponentInChildren<Button_Drop_All>();
        buttonDropAll.gameObject.SetActive(false);
        buttonOpen = GetComponentInChildren<Button_Open>();
        buttonOpen.gameObject.SetActive(false);
    }
    
    public void RightClickMenuCheck()
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        
        if (hit.collider != null)
        {
            Actor actor = hit.collider.gameObject.GetComponent<Actor>();

            if (actor != null)
            {

                RightClickMenu(interactedThing: actor.gameObject, talkable: true);
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
        _thing = null;
        buttonEquip.gameObject.SetActive(false);
        buttonPickup.gameObject.SetActive(false);
        buttonTalk.gameObject.SetActive(false);
        buttonUnequip.gameObject.SetActive(false);
        buttonDropOne.gameObject.SetActive(false);
        buttonDropX.gameObject.SetActive(false);
        buttonDropAll.gameObject.SetActive(false);
        buttonOpen.gameObject.SetActive(false);
    }

    public void ActiveButtonsCheck(GameObject interactedThing = null,
                               Actor actor = null,
                               List_Item item = null,
                               bool equippable = false,
                               bool itemEquipped = false,
                               bool droppable = false,
                               bool talkable = false,
                               bool openable = false)
    {
        position = Input.mousePosition;

        _thing = interactedThing;

        buttonPickup.gameObject.SetActive(true);  // Always active for now, then use item

        buttonEquip.gameObject.SetActive(equippable);
        buttonUnequip.gameObject.SetActive(itemEquipped);
        buttonTalk.gameObject.SetActive(talkable);
        buttonDropOne.gameObject.SetActive(droppable);
        buttonDropX.gameObject.SetActive(droppable);
        buttonDropAll.gameObject.SetActive(droppable);
        buttonOpen.gameObject.SetActive(openable);

        pressedInventorySlot = _thing.GetComponent<Inventory_Slot>();
        pressedEquipmentSlot = _thing.GetComponent<Equipment_Slot>();
        pressedActor = actor;
        pickedUpItem = GetComponent<Interactable_Item>();
    }
    public void RightClickMenu(GameObject interactedThing = null,
                           Actor actor = null,
                           List_Item item = null,
                           bool equippable = false,
                           bool itemEquipped = false,
                           bool droppable = false,
                           bool talkable = false,
                           bool openable = false)
    {
        ActiveButtonsCheck(interactedThing, actor, item, equippable, itemEquipped, droppable, talkable, openable);
        RightClickMenuOpen();
    }

    public void EquipButtonPressed()
    {
        int pressedSlot = -1;

        if (pressedInventorySlot != null)
        {
            pressedSlot = pressedInventorySlot.slotIndex;
        }

        bool equipped = InputManager.OnEquipFromInventory(pickedUpItem: pickedUpItem, inventorySlotIndex: pressedSlot);
        RightClickMenuClose();
    }
    public void UnequipButtonPressed()
    {
        if (pressedEquipmentSlot != null)
        {
            Equipment_Manager equipmentManager = pressedActor.GetComponent<Equipment_Manager>();
            equipmentManager.Unequip(pressedEquipmentSlot);
            RightClickMenuClose();
        }
    }
    public void PickupButtonPressed()
    {
        bool pickedUp = InputManager.OnItemPickup(itemID: pickedUpItem.itemID, stackSize: pickedUpItem.stackSize);
        
        if (pickedUp)
        {
            Destroy(pickedUpItem.gameObject);
        }

        RightClickMenuClose();
    }
    public void Drop(int dropAmount, Equipment_Slot equipmentSlot = null, Inventory_Slot inventorySlot = null, Actor actor = null)
    {
        if (equipmentSlot != null)
        {
            Equipment_Manager equipmentManager = actor.GetComponent<Equipment_Manager>();
            equipmentManager.DropItem(equipmentSlot, dropAmount);
            Manager_Input.Instance.RefreshUI(actor.gameObject, actor.GetComponent<Equipment_Manager>());
        }

        else if (inventorySlot != null)
        {
            Inventory_Manager inventoryManager = actor.GetComponent<Inventory_Manager>();
            inventoryManager.DropItem(inventorySlot.slotIndex, dropAmount, inventoryManager);
            Manager_Input.Instance.RefreshUI(actor.gameObject, actor.GetComponent<Equipment_Manager>());
        }
    }
    public void DropOneButtonPressed()
    {
        Drop(1, equipmentSlot: pressedEquipmentSlot, inventorySlot: pressedInventorySlot, actor: pressedActor);
    }
    public void DropXButtonPressed()
    {
        UI_Slider.instance.OpenSlider();

        if (pressedEquipmentSlot != null)
        {
            Equipment_Manager equipmentManager = pressedActor.GetComponent<Equipment_Manager>();
            UI_Slider.instance.DropItemSlider(equipmentManager.currentEquipment[pressedEquipmentSlot].Item2, dropXEquipmentSlot: pressedEquipmentSlot, dropXActor: pressedActor);
            RightClickMenuClose();

        }
        else if (pressedInventorySlot != null)
        {
            Inventory_Manager inventoryManager = pressedActor.GetComponent<Inventory_Manager>();
            UI_Slider.instance.DropItemSlider(inventoryManager.InventoryItemIDs[pressedInventorySlot.slotIndex].Item2, dropXInventorySlot: pressedInventorySlot, dropXActor: pressedActor);
            RightClickMenuClose();
        }
    }
    public void DropAllButtonPressed()
    {
        Drop(-1, equipmentSlot: pressedEquipmentSlot, inventorySlot: pressedInventorySlot, actor: pressedActor);
    }
    public void TalkButtonPressed()
    {
        if (pressedActor != null)
        {
            Dialogue_Manager.instance.OpenDialogue(pressedActor.gameObject, pressedActor.dialogue);
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
