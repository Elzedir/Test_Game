using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Stats : MonoBehaviour
{
    public Manager_Stats instance;
    private Equipment_Manager equipmentManager;
    public float currentHealth;
    public float maxHealth;

    void Start()
    {
        Manager_Stats instance = this;
        currentHealth = maxHealth = 100;
        equipmentManager = GetComponent<Equipment_Manager>();
        equipmentManager.equipmentChanged += UpdateStatsOnEquipmentChanged;
        UpdateStats();
    }

    void UpdateStatsOnEquipmentChanged(Equipment newEquipment, Equipment defaultEquipment)
    {
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
