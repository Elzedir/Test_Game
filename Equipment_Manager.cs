using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Equipment_Manager : MonoBehaviour
{
    public int equipSlot = 6;
    public enum EquipmentSlot { 
        Head = 0, 
        Chest = 1, 
        LeftArm = 2, 
        RightArm = 3, 
        Legs = 4, 
        Feet = 5
    }
    public Equipment[] currentEquipment = new Equipment[System.Enum.GetNames(typeof(EquipmentSlot)).Length];

    public delegate void EquipmentChanged(Equipment newEquipment, Equipment oldEquipment);

    public Sprite[] equippedIcons;
    public Manager_Item[] equippedItems;
    public SpriteRenderer[] equippedSprites;

    public static UnityEngine.Events.UnityEvent equipmentChanged;

    void Start()
    {
        equippedIcons = new Sprite[equipSlot];
        equippedItems = new Manager_Item[equipSlot];
        equippedSprites = new SpriteRenderer[equipSlot];

        for (int i = 0; i < equipSlot; i++)
        {
            equippedIcons[i] = null;
            equippedItems[i] = null;
            equippedSprites[i] = null;
        }

        currentEquipment = new Equipment[equipSlot];
    }

    public void EquipItem(Manager_Item equipment, int equipSlot)
    {
        if (equipment == null)
        {
            Debug.LogWarning("Tried to equip null item.");
            return;
        }

        if (equipSlot >= currentEquipment.Length)
        {
            Debug.LogWarning("Tried to equip item to invalid slot.");
            return;
        }

        if (!(equipment is Equipment))
        {
            Debug.LogWarning($"Tried to equip {equipment.itemName}, which is not an equipment item.");
            return;
        }

        if (equipment.itemID != currentEquipment[equipSlot].allowedItemType)
        {
            Debug.LogWarning($"Tried to equip {equipment.itemName} in invalid slot.");
            return;
        }

        if (currentEquipment[equipSlot] != null)
        {
            Unequip(equipSlot);
        }

        currentEquipment[equipSlot] = (Equipment)equipment;

        // Modify character's stats based on equipped item
        
        Manager_UI.instance.UpdateInventoryUI();
    }

    public void Equip(Equipment newEquipment)
    {
        int slotIndex = (int)newEquipment.equipSlot;
        Equipment previousEquipment = Unequip(slotIndex);

        if (currentEquipment[slotIndex] != null)
        {
            previousEquipment = currentEquipment[slotIndex];
            Manager_Inventory.Add(previousEquipment);
        }

        if (newEquipment != null)
        {
            newEquipment.Equip();
            currentEquipment[slotIndex] = newEquipment;
            //equippedSprites[slotIndex].sprite = newEquipment.equipSprite;
        }

        if (equipmentChanged != null)
        {
            equipmentChanged.Invoke(newEquipment, previousEquipment);
        }
    }

    public Equipment Unequip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment previousEquipment = currentEquipment[slotIndex];
            previousEquipment.Unequip();
            currentEquipment[slotIndex] = null;

            SpriteRenderer equippedSprite = transform.GetChild(slotIndex).GetComponent<SpriteRenderer>();

            equippedSprite.sprite = null;

            equipmentChanged?.Invoke(null, previousEquipment);

            return previousEquipment;
        }

        return null;
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }

        if (equipmentChanged != null)
        {
            equipmentChanged.Invoke(null, null);
        }
    }

    public void SwitchWeapon(int weaponID)
    {
        if (!itemDictionary.ContainsKey(weaponID))
        {
            Debug.Log("Cannot find weaponID");
            return;
        }

        List_Weapon weaponData = GetItemData(weaponID);

        GameObject playerWeapon = GameObject.Find("PlayerWeapon");
        if (playerWeapon != null)
        {
            return;
        }

        Equipment equipmentScript = playerWeapon.GetComponent<Equipment>();

        if (equipmentScript == null)
        {
            Debug.Log("PlayerWeapon script not found.");
            return;
        }
        SpriteRenderer WepSkin = playerWeapon.GetComponent<SpriteRenderer>();
        BoxCollider2D WepCollider = playerWeapon.GetComponent<BoxCollider2D>();

        equipmentScript.wepID = weaponData.itemID;
        equipmentScript.wepName = weaponData.wepName;
        equipmentScript.wepDamage = weaponData.itemDamage;
        equipmentScript.wepSpeed = weaponData.itemSpeed;
        equipmentScript.wepForce = weaponData.itemForce;
        equipmentScript.wepRange = weaponData.itemRange;
        equipmentScript.equipSkin = weaponData.wepIcon;

        equipmentScript.SetWeaponData(weaponData);
    }
}
