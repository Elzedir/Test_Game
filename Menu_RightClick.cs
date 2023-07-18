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

public class Menu_RightClick : MonoBehaviour
{
    public static Menu_RightClick instance;
    public Manager_Input inputManager;
    public Menu_RightClick menuRightClickScript;

    private Inventory_Slot pressedInventorySlot;
    private Inventory_EquipmentSlot pressedInventoryEquipmentSlot;
    private Equipment_Slot pressedEquipmentSlot;
    private Actor pressedActor;

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
    }
    
    public void RightClickMenuCheck()
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        
        if (hit.collider != null)
        {
            Transform hitTransform = hit.transform;
            
            if (hit.collider.gameObject.GetComponent<Actor>() != null)
            {
                RightClickMenuActor(hitTransform);
            }
        }
    }
    public void RightClickMenuOpen()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        transform.position = position;
    }
    public void RightClickMenuClose()
    {
        gameObject.SetActive(false);
        pressedInventorySlot = null;
        pressedInventoryEquipmentSlot = null;
        pressedActor = null;
    }

    public void RightClickMenuInventory(Inventory_Slot inventorySlot, Vector3 slotPosition)
    {
        position = slotPosition;
        pressedInventorySlot = inventorySlot;
        RightClickMenuOpen();
    }

    public void RightClickMenuEquipment(Inventory_EquipmentSlot equipmentSlot, Vector3 slotPosition, Actor actor)
    {
        position = slotPosition;
        pressedInventoryEquipmentSlot = equipmentSlot;
        pressedActor = actor;
        RightClickMenuOpen();
    }
    public void RightClickMenuActor(Transform actorTransform)
    {
        position = actorTransform.position;
        pressedActor = actorTransform.GetComponent<Actor>();
        RightClickMenuOpen();
    }
    
    public void EquipButtonPressed()
    {
        int pressedSlot = pressedInventorySlot.slotIndex;

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
        Debug.Log("unequip button pressed");
        if (pressedInventoryEquipmentSlot != null)
        {
            Equipment_Slot equipSlot = null;
            Equipment_Manager equipmentManager = pressedActor.GetComponent<Equipment_Manager>();

            switch (pressedInventoryEquipmentSlot.equipmentSlotType)
            {
                case EquipmentSlotType.Head:
                    equipSlot = pressedActor.equipmentManager.Head;
                    break;
                case EquipmentSlotType.Chest:
                    equipSlot = pressedActor.equipmentManager.Chest;
                    break;
                case EquipmentSlotType.MainHand:
                    equipSlot = pressedActor.equipmentManager.MainHand;
                    break;
                case EquipmentSlotType.OffHand:
                    equipSlot = pressedActor.equipmentManager.OffHand;
                    break;
                case EquipmentSlotType.Legs:
                    equipSlot = pressedActor.equipmentManager.Legs;
                    break;
            }

            
            equipmentManager.Unequip(equipSlot);
        }
    }

    public void PickupButtonPressed()
    {
        int pressedSlot = pressedInventorySlot.slotIndex;

        if (pressedSlot != -1)
        {
            bool pickedUp = inputManager.OnItemPickup(pressedSlot);

            if (!pickedUp)
            {
                Debug.Log("Could not pickup item.");
                RightClickMenuClose();
            }
        }
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
