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
    private Actor_Base _actor;

    public float CurrentSpeed;

    // Damager
    public float CurrentDamageAmount;
    public float CurrentPushForce;
    public Vector3 CurrentPushDirection;

    //Defence
    public float CurrentHealth;
    public float MaxHealth;
    public float CurrentMana;
    public float MaxMana;
    public float CurrentStamina;
    public float MaxStamina;
    public float CurrentPhysicalDefence;
    public float CurrentMagicalDefence;
    public float CurrentDodgeMitigation;

    private void Start()
    {
        _actor = GetComponent<Actor_Base>();
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
        if (CurrentHealth == 0)
        {
            CurrentHealth = MaxHealth;
        }
        if (CurrentMana == 0)
        {
            CurrentMana = MaxMana;
        }
        if (CurrentStamina == 0) 
        { 
            CurrentStamina = MaxStamina;
        }
    }



    public void UpdateStats()
    {
        CurrentDamageAmount = _actor.ActorData.ActorStats.CombatStats.BaseDamage;
        CurrentPushForce = _actor.ActorData.ActorStats.CombatStats.BasePushForce;
        MaxHealth = _actor.ActorData.ActorStats.CombatStats.BaseHealth;
        MaxMana = _actor.ActorData.ActorStats.CombatStats.BaseMana;
        MaxStamina = _actor.ActorData.ActorStats.CombatStats.BaseStamina;
        CurrentPhysicalDefence = _actor.ActorData.ActorStats.CombatStats.BasePhysicalDefence;
        CurrentMagicalDefence = _actor.ActorData.ActorStats.CombatStats.BaseMagicalDefence;
        CurrentSpeed = _actor.ActorData.ActorStats.CombatStats.BaseSpeed; // Add modifiers later

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

                MaxHealth += _actor.ActorData.ActorStats.CombatStats.BaseHealth;
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
