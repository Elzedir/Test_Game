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
    private Actor_Data_SO _actorData;
    public float CurrentSpeed;

    // Damager

    public CombatStats CurrentCombatStats;
    
    public int CurrentInventorySize;

    private void Start()
    {
        _actorData = GetComponent<Actor_Base>().ActorData;
        StartCoroutine(DelayedUpdateStats());
        InitialiseStats();
    }

    private IEnumerator DelayedUpdateStats()
    {
        float timeout = 0.1f;
        float timer = 0.0f;

        while (_actor.ActorScripts.EquipmentManager == null || _actor.ActorScripts.EquipmentManager.CurrentEquipment == null)
        {
            if (timer > timeout)
            {
                Debug.LogWarning("Timed out waiting for EquipmentManager to initialize.");
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        UpdateStats();
    }

    public void InitialiseStats()
    {
        if (CurrentCombatStats.Health == 0)
        {
            CurrentCombatStats.Health = _actorData.ActorStats.CombatStats.Health;
        }
        if (CurrentCombatStats.Mana == 0)
        {
            CurrentCombatStats.Mana = _actorData.ActorStats.CombatStats.Mana;
        }
        if (CurrentCombatStats.Stamina == 0) 
        {
            CurrentCombatStats.Stamina = _actorData.ActorStats.CombatStats.Stamina;
        }
    }



    public void UpdateStats()
    {
        CurrentCombatStats = _actorData.ActorStats.CombatStats;
        CurrentInventorySize = _actorData.ActorInventory.BaseInventorySize;

        int currentSlot = 0;

        Equipment_Manager equipmentManager = _actor.ActorScripts.EquipmentManager;

        if (equipmentManager == null)
        {
            GetComponent<Equipment_Manager>();
        }

        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> equipment in equipmentManager.CurrentEquipment)
        {
            if (equipment.Value.Item1 != -1)
            {
                if (currentSlot >= _actor.ActorScripts.EquipmentManager.CurrentEquipment.Count)
                {
                    break;
                }

                List_Item item = List_Item.GetItemData(equipment.Value.Item1);

                if (item is List_Item_Weapon weapon)
                {
                    CurrentDamageAmount += weapon.ItemStats.WeaponStats.ItemDamage;
                    CurrentPushForce += weapon.ItemStats.WeaponStats.ItemForce;
                }
                else if (item is List_Item_Armour armour)
                {
                    MaxHealth += armour.ItemStats.ArmourStats.ItemMaxHealthBonus;
                    CurrentPhysicalDefence += armour.ItemStats.ArmourStats.ItemPhysicalArmour;
                    CurrentMagicalDefence += armour.ItemStats.ArmourStats.ItemMagicalArmour;
                }

                MaxHealth += _actor.ActorData.ActorStats.CombatStats.Health;
                if (CurrentHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }

                currentSlot++;
            }
            else
            {
                currentSlot++;
            }
        }
    }
    public Damage DealDamage(float chargeTime)
    {
        Damage damage = new Damage
        {
            origin = _actor.transform.position,
            damageAmount = CurrentDamageAmount * (1 + (chargeTime * 0.25f)),
            pushForce = CurrentPushForce
        };

        return damage;
    }
    public void ReceiveDamage(Damage damage)
    {
        // Later, be able to tell between physical and magical damage types.

        float damageReduction = CurrentPhysicalDefence + CurrentMagicalDefence;
        if (_actor.ActorStates.Dodging)
        {
            damageReduction += CurrentDodgeMitigation;
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
        
        CurrentHealth -= finalDamage;

        if (CurrentHealth <= 0)
        {
            _actor.Death();
            return;
        }

        CurrentPushDirection = (transform.position - damage.origin).normalized * damage.pushForce;
    }

    public void RestoreHealth(float amount)
    {
        CurrentHealth += amount;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
            return;
        }

        GameManager.Instance.ShowFloatingText("+" + amount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.Instance.HUDBarChange();
    }

    public void UpdateStatsOnLevelUp()
    {
        UpdateStats();
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
        CurrentStamina = MaxStamina;
    }
}
