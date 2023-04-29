using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Manager_Stats : MonoBehaviour
{
    // General
    private Equipment_Manager EquipmentManager;
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
        UpdateStats();
    }

    void UpdateStatsOnEquipmentChanged(Manager_Item newEquipment, Manager_Item previousEquipment)
    {
        // Need to change
        UpdateStats();
    }

    public void UpdateStats()
    {
        // Complete update stats for damage, resistance, health, 
        
        for (int i = 0; i < EquipmentManager.currentEquipment.Length; i++)
        {
            if (EquipmentManager.currentEquipment[i] != null)
            {
                //maxHealth += equipmentManager.currentEquipment[i].health;
            }
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void ReciveDamage(int damage)
    {
        //Actor.ReceiveDamage();
    }

    public void RestoreHealth(int amount)
    {
        // Simply do the opposite of this
        //Actor.ReceiveDamage();
    }
}
