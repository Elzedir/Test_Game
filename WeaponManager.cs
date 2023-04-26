using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    public bool equipabble = false;

    public List<WeaponData> weapons = new List<WeaponData>();

    private Dictionary<int, WeaponData> weaponDataDictionary;

    public virtual void Awake()
    {
        instance = this;
    
        weaponDataDictionary = new Dictionary<int, WeaponData>();

        WeaponData.allWeaponData = new List<WeaponData>();
        WeaponData.InitializeWeaponData();

        {
            foreach (WeaponData weaponData in WeaponData.allWeaponData)
            {
                weaponDataDictionary.Add(weaponData.wepID, weaponData);
            }
        }
    }

    public WeaponData GetWeaponData(int weaponID)
    {
        if (weaponDataDictionary.TryGetValue(weaponID, out WeaponData weaponData))
        {
            return weaponData;
        }

        else
        {
            Debug.LogError("Weapon ID " + weaponID + " does not exist.");
            return null;
        }
    }

    public void SwitchWeapon(int weaponID)
    {
        if (!weaponDataDictionary.ContainsKey(weaponID))
        {
            Debug.Log("Cannot find weaponID");
            return;
        }

        WeaponData weaponData = GetWeaponData(weaponID);

        GameObject playerWeapon = GameObject.Find("PlayerWeapon");
        if (playerWeapon != null)
        {
            return;
        }

        PlayerWeapon weaponScript = playerWeapon.GetComponent<PlayerWeapon>();
        
        if (weaponScript == null)
        {
            Debug.Log("PlayerWeapon script not found.");
            return;
        }
        SpriteRenderer wepSkin = playerWeapon.GetComponent<SpriteRenderer>();
        BoxCollider2D wepCollider = playerWeapon.GetComponent<BoxCollider2D>();
        
        weaponScript.wepID = weaponData.wepID;
        weaponScript.wepName = weaponData.wepName;
        weaponScript.wepDamage = weaponData.wepDamage;
        weaponScript.wepSpeed = weaponData.wepSpeed;
        weaponScript.wepForce = weaponData.wepForce;
        weaponScript.wepRange = weaponData.wepRange;
        weaponScript.wepSkin = weaponData.wepSkin;

        weaponScript.SetWeaponData(weaponData);
    }

}
