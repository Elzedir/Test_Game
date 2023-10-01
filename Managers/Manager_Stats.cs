using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Manager_Stats : MonoBehaviour
{
    public static IEnumerator InitialiseStats(Actor_Base actor)
    {
        yield return new WaitForSeconds(0.1f);
        UpdateStats(actor);
        actor.ActorData.ActorInventory.InitialiseInventoryItems(actor.ActorData.ActorInventory.CurrentInventorySize, actor.ActorData.ActorInventory);
    }

    public static void UpdateStats(Actor_Base actor)
    {
        actor.ActorData.ActorInventory.CurrentInventorySize = actor.ActorData.ActorInventory.BaseInventorySize; // change this later
        List<EquipmentItem> equipmentData = actor.GetEquipmentData().EquipmentItems;
        ActorStats actorStats = actor.ActorData.ActorStats;

        float currentHealth = actor.CurrentCombatStats.CurrentHealth != 0 ? actor.CurrentCombatStats.CurrentHealth : actorStats.CombatStats.MaxHealth;
        float currentMana = actor.CurrentCombatStats.CurrentMana != 0 ? actor.CurrentCombatStats.CurrentMana : actorStats.CombatStats.MaxMana;
        float currentStamina = actor.CurrentCombatStats.CurrentStamina != 0 ? actor.CurrentCombatStats.CurrentStamina : actorStats.CombatStats.MaxStamina;

        actor.CurrentCombatStats = new CombatStats(actor.ActorData.ActorStats.CombatStats);

        for (int i = 0; i < equipmentData.Count; i++)
        {
            List_Item item = List_Item.GetItemData(equipmentData[i].ItemID);

            if (item != null && item.ItemStats.CommonStats.ItemID != -1)
            {
                actor.CurrentCombatStats = CombatStats.GetCombatStatsData(itemStats: item.ItemStats, combatStats: actor.CurrentCombatStats);
            }
        }

        actor.CurrentCombatStats.CurrentHealth = currentHealth;
        actor.CurrentCombatStats.CurrentMana = currentMana;
        actor.CurrentCombatStats.CurrentStamina = currentStamina;

        actor.CurrentCombatStats.AttackRange /= 5;
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
    public static void ReceiveDamage(Actor_Base damageDestinationActor, Damage damage)
    {
        // Later, be able to tell between physical and magical damage types.

        float damageReduction = damageDestinationActor.CurrentCombatStats.PhysicalDefence + damageDestinationActor.CurrentCombatStats.MagicalDefence;

        if (damageDestinationActor.ActorStates.Dodging)
        {
            damageReduction *= 2; // Change this to be a dodgemitigation calculation
        }
        
        float finalDamage = damage.damageAmount - damageReduction;

        if (finalDamage < 1)
        {
            finalDamage = 1;
        }

        if (GameManager.Instance.Player.gameObject != damageDestinationActor.gameObject)
        {
            GameManager.Instance.ShowFloatingText(damage.damageAmount.ToString(), 25, Color.red, damageDestinationActor.transform.position, Vector3.up * 30, 0.5f);
            GameManager.Instance.HUDBarChange();
        }
        else
        {
            GameManager.Instance.ShowFloatingText(damage.damageAmount.ToString(), 25, Color.green, damageDestinationActor.transform.position, Vector3.up * 30, 0.5f);
        }

        damageDestinationActor.CurrentCombatStats.CurrentHealth -= finalDamage;

        if (damageDestinationActor.CurrentCombatStats.CurrentHealth <= 0)
        {
            damageDestinationActor.Death();
            return;
        }

        damageDestinationActor.PushDirection = (damageDestinationActor.transform.position - damage.origin).normalized * damage.pushForce;
    }

    public static void RestoreHealth(Actor_Base actor, float amount)
    {
        if (actor.CurrentCombatStats.CurrentHealth == actor.CurrentCombatStats.MaxHealth)
        {
            return;
        }

        actor.CurrentCombatStats.CurrentHealth += amount;

        if (actor.CurrentCombatStats.CurrentHealth > actor.CurrentCombatStats.MaxHealth)
        {
            actor.CurrentCombatStats.CurrentHealth = actor.CurrentCombatStats.MaxHealth;
            return;
        }

        GameManager.Instance.ShowFloatingText("+" + amount.ToString() + "hp", 25, Color.green, actor.transform.position, Vector3.up * 30, 1.0f);
        GameManager.Instance.HUDBarChange();
    }

    public static void UpdateStatsOnLevelUp(Actor_Base actor)
    {
        UpdateStats(actor);
        actor.CurrentCombatStats.MaxHealth = actor.ActorData.ActorStats.CombatStats.MaxHealth;
        actor.CurrentCombatStats.MaxMana = actor.ActorData.ActorStats.CombatStats.MaxMana;
        actor.CurrentCombatStats.MaxStamina = actor.ActorData.ActorStats.CombatStats.MaxStamina;

        actor.CurrentCombatStats.CurrentHealth = actor.CurrentCombatStats.MaxHealth;
        actor.CurrentCombatStats.CurrentMana = actor.CurrentCombatStats.MaxMana;
        actor.CurrentCombatStats.CurrentStamina = actor.CurrentCombatStats.MaxStamina;
    }
}
