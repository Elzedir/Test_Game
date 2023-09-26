using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Manager_Stats : MonoBehaviour
{
    // General
    private Actor_Base _actor;

    // Damager

    public CombatStats CurrentCombatStats;
    
    public int CurrentInventorySize;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        _actor = GetComponent<Actor_Base>();
        UpdateStats();
        _actor.ActorData.ActorInventory.InitialiseInventoryItems(CurrentInventorySize, _actor.ActorData.ActorInventory);
    }

    public void UpdateStats()
    {
        CurrentInventorySize = _actor.ActorData.ActorInventory.BaseInventorySize; // change this later
        List<EquipmentItem> equipmentData = _actor.GetEquipmentData().EquipmentItems;

        float currentHealth = CurrentCombatStats.CurrentHealth != 0 ? CurrentCombatStats.CurrentHealth : CurrentCombatStats.MaxHealth;
        float currentMana = CurrentCombatStats.CurrentMana != 0 ? CurrentCombatStats.CurrentMana : CurrentCombatStats.MaxMana;
        float currentStamina = CurrentCombatStats.CurrentStamina != 0 ? CurrentCombatStats.CurrentStamina : CurrentCombatStats.MaxStamina;

        CurrentCombatStats = _actor.ActorData.ActorStats.CombatStats;

        for (int i = 0; i < equipmentData.Count; i++)
        {
            List_Item item = List_Item.GetItemData(equipmentData[i].ItemID);

            if (item != null && item.ItemStats.CommonStats.ItemID != -1)
            {
                CurrentCombatStats = CombatStats.GetCombatStatsData(item.ItemStats);
            }
        }

        CurrentCombatStats.CurrentHealth = currentHealth;
        CurrentCombatStats.CurrentMana = currentMana;
        CurrentCombatStats.CurrentStamina = currentStamina;

        CurrentCombatStats.AttackRange /= 5;
    }
    public static Damage DealDamage(Vector3 damageOrigin, CombatStats combatStats, float chargeTime)
    {
        Damage damage = new Damage
        {
            origin = damageOrigin,
            damageAmount = combatStats.AttackDamage * (1 + (chargeTime * 0.25f)),
            pushForce = combatStats.AttackPushForce
        };

        return damage;
    }
    public void ReceiveDamage(Damage damage)
    {
        // Later, be able to tell between physical and magical damage types.

        float damageReduction = CurrentCombatStats.PhysicalDefence + CurrentCombatStats.MagicalDefence;

        if (_actor.ActorStates.Dodging)
        {
            damageReduction *= 2; // Change this to be a dodgemitigation calculation
        }
        
        float finalDamage = damage.damageAmount - damageReduction;

        if (finalDamage < 1)
        {
            finalDamage = 1;
        }

        if (GameManager.Instance.Player.gameObject != this.gameObject)
        {
            GameManager.Instance.ShowFloatingText(damage.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.up * 30, 0.5f);
            GameManager.Instance.HUDBarChange();
        }
        else
        {
            GameManager.Instance.ShowFloatingText(damage.damageAmount.ToString(), 25, Color.green, transform.position, Vector3.up * 30, 0.5f);
        }
        
        CurrentCombatStats.MaxHealth -= finalDamage;

        if (CurrentCombatStats.MaxHealth <= 0)
        {
            _actor.Death();
            return;
        }

        _actor.PushDirection = (transform.position - damage.origin).normalized * damage.pushForce;
    }

    public void RestoreHealth(float amount)
    {
        CurrentCombatStats.MaxHealth += amount;

        if (CurrentCombatStats.MaxHealth > _actor.ActorData.ActorStats.CombatStats.MaxHealth)
        {
            CurrentCombatStats.MaxHealth = _actor.ActorData.ActorStats.CombatStats.MaxHealth;
            return;
        }

        GameManager.Instance.ShowFloatingText("+" + amount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.Instance.HUDBarChange();
    }

    public void UpdateStatsOnLevelUp()
    {
        UpdateStats();
        CurrentCombatStats.MaxHealth = _actor.ActorData.ActorStats.CombatStats.MaxHealth;
        CurrentCombatStats.MaxMana = _actor.ActorData.ActorStats.CombatStats.MaxMana;
        CurrentCombatStats.MaxStamina = _actor.ActorData.ActorStats.CombatStats.MaxStamina;
    }
}
