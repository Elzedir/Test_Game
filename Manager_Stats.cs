using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Stats : MonoBehaviour
{
    public Manager_Stats instance;
    private Equipment_Manager equipmentManager;
    public float currentHealth;
    public float maxHealth;
    private Equipment_Manager EquipmentManager;

    private void Awake()
    {
        Manager_Stats instance = this;
        UpdateStats();
    }

    void Start()
    {
        
    }

    void UpdateStatsOnEquipmentChanged(Equipment newEquipment, Equipment previousEquipment)
    {
        // Need to change
        UpdateStats();
    }

    void UpdateStats()
    {
        maxHealth = 100;

        for (int i = 0; i < equipmentManager.currentEquipment.Length; i++)
        {
            if (equipmentManager.currentEquipment[i] != null)
            {
                maxHealth += equipmentManager.currentEquipment[i].health;
            }
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
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

    void Die()
    {
        Debug.Log("Player died.");
    }
}
