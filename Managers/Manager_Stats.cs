using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        for (int i = 0; i < equipmentData.Count; i++)
        {
            List_Item item = List_Item.GetItemData(equipmentData[i].ItemID);

            float tempHealth = CurrentCombatStats.Health;
            float tempMana = CurrentCombatStats.Mana;
            float tempStamina = CurrentCombatStats.Stamina;

            if (item != null && item.ItemStats.CommonStats.ItemID != -1)
            {
                CurrentCombatStats = _actor.ActorData.ActorStats.CombatStats + item.ItemStats.CombatStats;
            }
            else
            {
                CurrentCombatStats = _actor.ActorData.ActorStats.CombatStats;
            }

            CurrentCombatStats.Health = tempHealth;
            CurrentCombatStats.Mana = tempMana;
            CurrentCombatStats.Stamina = tempStamina;
        }
    }
    public Damage DealDamage(float chargeTime)
    {
        Damage damage = new Damage
        {
            origin = _actor.transform.position,
            damageAmount = CurrentCombatStats.AttackDamage * (1 + (chargeTime * 0.25f)),
            pushForce = CurrentCombatStats.AttackPushForce
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
        
        CurrentCombatStats.Health -= finalDamage;

        if (CurrentCombatStats.Health <= 0)
        {
            _actor.Death();
            return;
        }

        _actor.PushDirection = (transform.position - damage.origin).normalized * damage.pushForce;
    }

    public void RestoreHealth(float amount)
    {
        CurrentCombatStats.Health += amount;

        if (CurrentCombatStats.Health > _actor.ActorData.ActorStats.CombatStats.Health)
        {
            CurrentCombatStats.Health = _actor.ActorData.ActorStats.CombatStats.Health;
            return;
        }

        GameManager.Instance.ShowFloatingText("+" + amount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.Instance.HUDBarChange();
    }

    public void UpdateStatsOnLevelUp()
    {
        UpdateStats();
        CurrentCombatStats.Health = _actor.ActorData.ActorStats.CombatStats.Health;
        CurrentCombatStats.Mana = _actor.ActorData.ActorStats.CombatStats.Mana;
        CurrentCombatStats.Stamina = _actor.ActorData.ActorStats.CombatStats.Stamina;
    }
}
