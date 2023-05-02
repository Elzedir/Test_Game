using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        maxHealth = 0;
        physicalDefence = 0;
        magicalDefence = 0;

        for (int i = 0; i < equipmentManager.currentEquipment.Length; i++)
        {
            if (equipmentManager.currentEquipment[i] is List_Weapon weapon)
            {
                damageAmount += weapon.weaponDamage;
                pushForce += weapon.weaponForce;
            }
            else if (equipmentManager.currentEquipment[i] is List_Armour armour)
            {
                maxHealth += armour.healthBonus;
                physicalDefence += armour.physicalDefence;
                magicalDefence += armour.magicalDefence;
            }
        }

        maxHealth += actor.baseHitpoints;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
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
