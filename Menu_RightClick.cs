using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Menu_RightClick : MonoBehaviour
{
    public static Menu_RightClick instance;
    public Manager_Input inputManager;
    public Menu_RightClick menuRightClickScript;
    public Equipment_Window equipmentWindow;

    private Inventory_Slot pressedInventorySlot;
    private Equipment_Slot pressedEquipmentSlot;
    private Actor pressedActor;

    private Button_Equip buttonEquip;
    private Button_PickupItem buttonPickup;
    private Button_Talk buttonTalk;
    private Button_Unequip buttonUnequip;
    private Button_Drop_One buttonDropOne;
    private Button_Drop_X buttonDropX;
    private Button_Drop_All buttonDropAll;

    private Vector3 position;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
    }
    
    public void RightClickMenuCheck()
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        
        if (hit.collider != null)
        {
            Transform hitTransform = hit.transform;
            Actor actor = hit.collider.gameObject.GetComponent<Actor>();

            if (actor != null)
            {

                RightClickMenu(hitTransform.position, actor: actor, talkable: true);
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
        pressedInventorySlot = null;
        pressedEquipmentSlot = null;
        pressedActor = null;
        buttonEquip.gameObject.SetActive(false);
        buttonPickup.gameObject.SetActive(false);
        buttonTalk.gameObject.SetActive(false);
        buttonUnequip.gameObject.SetActive(false);
        buttonDropOne.gameObject.SetActive(false);
        buttonDropX.gameObject.SetActive(false);
        buttonDropAll.gameObject.SetActive(false);
    }

    public void ActiveButtonsCheck(Vector3 menuPosition,
                               Inventory_Slot inventorySlot = null,
                               Equipment_Slot equipmentSlot = null,
                               Actor actor = null,
                               List_Item item = null,
                               bool equippable = false,
                               bool itemEquipped = false,
                               bool droppable = false,
                               bool talkable = false)
    {
        position = menuPosition;

        buttonPickup.gameObject.SetActive(true);  // Always active for now, then use item

        buttonEquip.gameObject.SetActive(equippable);
        buttonUnequip.gameObject.SetActive(itemEquipped);
        buttonTalk.gameObject.SetActive(talkable);
        buttonDropOne.gameObject.SetActive(droppable);
        buttonDropX.gameObject.SetActive(droppable);
        buttonDropAll.gameObject.SetActive(droppable);

        pressedInventorySlot = inventorySlot;
        pressedEquipmentSlot = equipmentSlot;
        pressedActor = actor;
    }
    public void RightClickMenu(Vector3 position,
                           Inventory_Slot inventorySlot = null,
                           Equipment_Slot equipmentSlot = null,
                           Actor actor = null,
                           List_Item item = null,
                           bool equippable = false,
                           bool itemEquipped = false,
                           bool droppable = false,
                           bool talkable = false)
    {
        ActiveButtonsCheck(position, inventorySlot, equipmentSlot, actor, item, equippable, itemEquipped, droppable, talkable);
        RightClickMenuOpen();
    }
    
    public void EquipButtonPressed()
    {
        int pressedSlot = -1;

        if (pressedInventorySlot != null)
        {
            pressedSlot = pressedInventorySlot.slotIndex;
        }

        if (pressedSlot != -1)
        {
            bool equipped = inputManager.OnEquipFromInventory(pressedSlot);
            RightClickMenuClose();

            if (!equipped)
            {
                Debug.Log("Could not equip item.");
            }
        }
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
        int pressedSlot = -1;

        if (pressedInventorySlot != null)
        {
            pressedSlot = pressedInventorySlot.slotIndex;
        }

        if (pressedSlot != -1)
        {
            bool pickedUp = inputManager.OnItemPickup(pressedSlot);
            RightClickMenuClose();

            if (!pickedUp)
            {
                Debug.Log("Could not pickup item.");
            }
        }
    }
    public void Drop(int dropAmount, Equipment_Slot equipmentSlot = null, Inventory_Slot inventorySlot = null, Actor actor = null)
    {
        if (equipmentSlot != null)
        {
            Equipment_Manager equipmentManager = UI_Slider.instance.actor.GetComponent<Equipment_Manager>();
            equipmentManager.DropItem(equipmentSlot, dropAmount);
            Manager_Input.instance.RefreshUI(actor.gameObject, actor.GetComponent<Equipment_Manager>());
        }

        else if (inventorySlot != null)
        {
            Inventory_Manager inventoryManager = actor.GetComponent<Inventory_Manager>();
            inventoryManager.DropItem(inventorySlot.slotIndex, dropAmount, inventoryManager);
            Manager_Input.instance.RefreshUI(actor.gameObject, actor.GetComponent<Equipment_Manager>());
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
}
