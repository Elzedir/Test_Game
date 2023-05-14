using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class Manager_Stats : MonoBehaviour
{
    // General
    private Equipment_Manager equipmentManager;
    private Actor actor;

    // Damager
    public float damageAmount;
    public float pureDamage;
    public float pushForce;

    //Defence
    public float currentHealth;
    public float maxHealth;
    public float physicalDefence;
    public float magicalDefence;
    public float pureDefence;

    private void Start()
    {
        equipmentManager = GetComponent<Equipment_Manager>();
        //UpdateStats();
    }

    void UpdateStatsOnEquipmentChanged(List_Item newEquipment, List_Item previousEquipment)
    {
        // Need to change
        UpdateStats();
    }

    public void UpdateStats()
    {
        maxHealth = 0; // Change this by level
        physicalDefence = 0; // Change this by level
        magicalDefence = 0; // Change this by level

        int currentSlot = 0;

        foreach (KeyValuePair<int, (int, int, bool)> equipment in equipmentManager.currentEquipment)
        {
            if (equipment.Value.Item1 != -1)
            {
                if (currentSlot >= equipmentManager.currentEquipment.Count)
                {
                    break;
                }

                List_Item item;

                switch (equipment.Value.Item1)
                {
                    case 1:
                        item = List_Item.GetItemData(equipment.Value.Item1, List_Weapon.allWeaponData);

                        break;
                    case 2:
                        item = List_Item.GetItemData(equipment.Value.Item1, List_Armour.allArmourData);

                        break;
                    case 3:
                        item = List_Item.GetItemData(equipment.Value.Item1, List_Consumable.allConsumableData);

                        break;
                    default:
                        item = null;
                        break;
                }

                if (item is List_Weapon weapon)
                {
                    damageAmount += weapon.itemDamage;
                    pushForce += weapon.itemForce;
                }
                else if (item is List_Armour armour)
                {
                    maxHealth += armour.healthBonus;
                    physicalDefence += armour.physicalDefence;
                    magicalDefence += armour.magicalDefence;
                }

                maxHealth += actor.baseHitpoints;
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                currentSlot++;
            }
            else
            {
                currentSlot++;
            }
        }
    }

    public void ReciveDamage(int damage)
    {
        // Apply damage and calculate damage reduction
        float totalDefence = physicalDefence + magicalDefence;
        float damageReduction = totalDefence / (totalDefence + 100);
        float finalDamage = damage * (1 - damageReduction);

        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            // Death();
        }
    }

    public void RestoreHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
