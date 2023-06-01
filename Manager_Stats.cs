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
    private Actor actor;

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
        actor = GetComponent<Actor>();
        UpdateStats();
    }

    public void UpdateStats()
    {
        currentDamageAmount = actor.baseDamage;
        currentPushForce = actor.baseForce;
        maxHealth = actor.baseHealth;
        maxMana = actor.baseMana;
        maxStamina = actor.baseStamina;
        currentPhysicalDefence = actor.basePhysicalDefence;
        currentMagicalDefence = actor.baseMagicalDefence;

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
                    currentDamageAmount += weapon.itemDamage;
                    currentPushForce += weapon.itemForce;
                }
                else if (item is List_Armour armour)
                {
                    maxHealth += armour.healthBonus;
                    currentPhysicalDefence += armour.physicalDefence;
                    currentMagicalDefence += armour.magicalDefence;
                }

                maxHealth += actor.baseHealth;
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
        Debug.Log("Receive damage called in Stat manager");
        float totalDefence = currentPhysicalDefence + currentMagicalDefence;
        float damageReduction = totalDefence / (totalDefence + 100);
        float finalDamage = damage.damageAmount * (1 - damageReduction);

        GameManager.instance.ShowFloatingText(damage.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.up * 30, 0.5f);
        GameManager.instance.HUDBarChange();

        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            actor.Death();
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

        GameManager.instance.ShowFloatingText("+" + amount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.instance.HUDBarChange();
    }

    public void UpdateStatsOnLevelUp()
    {
        UpdateStats();
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
    }
}
