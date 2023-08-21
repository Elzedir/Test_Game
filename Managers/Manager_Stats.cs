using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;

[System.Serializable]
public class Manager_Stats : MonoBehaviour
{
    // General
    private Equipment_Manager equipmentManager;
    private Actor_Base actor;

    // Damager
    public float currentDamageAmount;
    public float currentPushForce;
    public Vector3 currentPushDirection;

    //Defence
    public float currentHealth;
    public float maxHealth;
    public float currentMana;
    public float maxMana;
    public float currentStamina;
    public float maxStamina;
    public float currentPhysicalDefence;
    public float currentMagicalDefence;


    private void Start()
    {
        equipmentManager = GetComponent<Equipment_Manager>();
        actor = GetComponent<Actor_Base>();
        UpdateStats();
        InitialiseStats();
    }

    public void InitialiseStats()
    {
        if (currentHealth == 0)
        {
            currentHealth = maxHealth;
        }
        if (currentMana == 0)
        {
            currentMana = maxMana;
        }
        if (currentStamina == 0) 
        { 
            currentStamina = maxStamina;
        }
    }

    public void UpdateStats()
    {
        currentDamageAmount = actor.ActorData.baseDamage;
        currentPushForce = actor.ActorData.baseForce;
        maxHealth = actor.ActorData.baseHealth;
        maxMana = actor.ActorData.baseMana;
        maxStamina = actor.ActorData.baseStamina;
        currentPhysicalDefence = actor.ActorData.basePhysicalDefence;
        currentMagicalDefence = actor.ActorData.baseMagicalDefence;

        int currentSlot = 0;

        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> equipment in equipmentManager.currentEquipment)
        {
            if (equipment.Value.Item1 != -1)
            {
                if (currentSlot >= equipmentManager.currentEquipment.Count)
                {
                    break;
                }

                List_Item item = List_Item.GetItemData(equipment.Value.Item1);

                if (item is List_Weapon weapon)
                {
                    currentDamageAmount += weapon.itemDamage;
                    currentPushForce += weapon.itemForce;
                }
                else if (item is List_Armour armour)
                {
                    maxHealth += armour.itemMaxHealthBonus;
                    currentPhysicalDefence += armour.itemPhysicalArmour;
                    currentMagicalDefence += armour.itemMagicalArmour;
                }

                maxHealth += actor.ActorData.baseHealth;
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
    public Damage DealDamage()
    {
        Damage damage = new Damage
        {
            origin = actor.transform.position,
            damageAmount = currentDamageAmount,
            pushForce = currentPushForce
        };

        return damage;
    }
    public void ReceiveDamage(Damage damage)
    {
        float totalDefence = currentPhysicalDefence + currentMagicalDefence;
        float damageReduction = totalDefence / (totalDefence + 100);
        float finalDamage = damage.damageAmount * (1 - damageReduction);

        Player player = GetComponent<Player>();
        if (player != null)
        {
            GameManager.Instance.ShowFloatingText(damage.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.up * 30, 0.5f);
            GameManager.Instance.HUDBarChange();
        }
        
        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            actor.dead = true;
            return;
        }

        currentPushDirection = (transform.position - damage.origin).normalized * damage.pushForce;
    }

    public void RestoreHealth(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            return;
        }

        GameManager.Instance.ShowFloatingText("+" + amount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.Instance.HUDBarChange();
    }

    public void UpdateStatsOnLevelUp()
    {
        UpdateStats();
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
    }
}
