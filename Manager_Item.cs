using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Manager_Item : MonoBehaviour
{
    public static Dictionary<int, object> itemDictionary;
    
    // General
    public static Manager_Item instance;
    public bool equipabble = false;
    public int itemID;
    public string itemName;

    public virtual void Awake()
    {
        instance = this;

        itemDictionary = new Dictionary<int, object>();

        List_Weapon.allWeaponData = new List<List_Weapon>();
        List_Weapon.InitializeWeaponData();
        List_Armour.allArmourData = new List<List_Armour>();
        List_Armour.InitializeArmourData();
        List_Consumables.allConsumableData = new List<List_Consumables>();
        List_Consumables.InitializeConsumableData();

        foreach (List_Weapon wepData in List_Weapon.allWeaponData)
        {
            itemDictionary.Add(wepData.itemID, wepData);
        }

        foreach (List_Armour armData in  List_Armour.allArmourData)
        {
            itemDictionary.Add(armData.itemID, armData);
        }

        foreach (List_Consumables consData in List_Consumables.allConsumableData)
        {
            itemDictionary.Add(consData.itemID, consData);
        }
    }

    public static void GetItemData(int itemID)
    {
        switch (itemID)
        {
            case 1:
                int weaponID = 1;
                string[] weaponData = (string[])itemDictionary["weapon_" + weaponID.ToString()];
                string weaponName = weaponData[0];
                int weaponDamage = int.Parse(weaponData[1]);
                float weaponSpeed = float.Parse(weaponData[2]);
                float weaponForce = float.Parse(weaponData[3]);
                float weaponRange = float.Parse(weaponData[4]);
                Sprite weaponIcon = (Sprite)weaponData[5];
                return new List_Weapon(wepID, wepName, wepDamage, wepSpeed, wepForce, wepRange, wepIcon);
            case 2:
                int armorID = 1;
                string[] armorData = (string[])itemDictionary["armor_" + armorID.ToString()];
                string armorName = armorData[0];
                Sprite armorIcon = (Sprite)armorData[1];
                return new List_Armour(armID, armName, armIcon);
            case 3:
                int consumableID = 1;
                string[] consumableData = (string[])itemDictionary["consumable_" + consumableID.ToString()];
                string consumableName = consumableData[0];
                int consumableValue = int.Parse(consumableData[1]);
                Sprite consumableIcon = (Sprite)consumableData[2];
                return new List_Consumables(consID, consName, consValue, consIcon);
            default:
                Debug.Log("Invalid item type");
                break;
        }
        
        //if (itemDictionary.TryGetValue(weaponID, out object weaponData))
        //{
        //    return (List_Weapon)weaponData;
        //}

        //else
        //{
        //    Debug.LogError("Weapon ID " + weaponID + " does not exist.");
        //    return null;
        //}
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
