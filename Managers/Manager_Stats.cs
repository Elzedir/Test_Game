using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Manager_Stats : MonoBehaviour
{
    // General
    private Actor_Data_SO _actorData;
    public float CurrentSpeed;

    // Damager

    public CombatStats CurrentCombatStats;
    
    public int CurrentInventorySize;

    private void Start()
    {
        _actorData = GetComponent<Actor_Base>().ActorData;
        UpdateStats();
    }

    public void UpdateStats()
    {
        CurrentInventorySize = _actorData.ActorInventory.BaseInventorySize; // change this later
        List<EquipmentItem> equipmentData = _actorData.Actor.GetEquipmentData().EquipmentItems;

        for (int i = 0; i < equipmentData.Count; i++)
        {
            List_Item item = List_Item.GetItemData(equipmentData[i].ItemStats.CommonStats.ItemID);

            float tempHealth = CurrentCombatStats.Health;
            float tempMana = CurrentCombatStats.Mana;
            float tempStamina = CurrentCombatStats.Stamina;

            CurrentCombatStats = _actorData.ActorStats.CombatStats + item.ItemStats.CombatStats;

            CurrentCombatStats.Health = tempHealth;
            CurrentCombatStats.Mana = tempMana;
            CurrentCombatStats.Stamina = tempStamina;
        }
    }
    public Damage DealDamage(float chargeTime)
    {
        Damage damage = new Damage
        {
            origin = _actorData.Actor.transform.position,
            damageAmount = CurrentCombatStats.AttackDamage * (1 + (chargeTime * 0.25f)),
            pushForce = CurrentCombatStats.AttackPushForce
        };

        return damage;
    }
    public void ReceiveDamage(Damage damage)
    {
        // Later, be able to tell between physical and magical damage types.

        float damageReduction = CurrentCombatStats.PhysicalDefence + CurrentCombatStats.MagicalDefence;

        if (_actorData.Actor.ActorStates.Dodging)
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
            _actorData.Actor.Death();
            return;
        }
    }

    public void RestoreHealth(float amount)
    {
        CurrentCombatStats.Health += amount;

        if (CurrentCombatStats.Health > _actorData.ActorStats.CombatStats.Health)
        {
            CurrentCombatStats.Health = _actorData.ActorStats.CombatStats.Health;
            return;
        }

        GameManager.Instance.ShowFloatingText("+" + amount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.Instance.HUDBarChange();
    }

    public void UpdateStatsOnLevelUp()
    {
        UpdateStats();
        CurrentCombatStats.Health = _actorData.ActorStats.CombatStats.Health;
        CurrentCombatStats.Mana = _actorData.ActorStats.CombatStats.Mana;
        CurrentCombatStats.Stamina = _actorData.ActorStats.CombatStats.Stamina;
    }
}
