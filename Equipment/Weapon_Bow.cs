using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class Weapon_Bow : Weapon
{
    public override void WeaponAttack(Ability ability = Ability.None)
    {
        Shoot(ability);
    }

    private void Shoot(Ability ability = Ability.None)
    {
        Actor_Base actor = GetComponentInParent<Actor_Base>();
        Equipment_Slot equipmentSlot = GetComponent<Equipment_Slot>();

        Vector2 attackDirection;

        if (actor.gameObject == GameManager.Instance.Player.gameObject)
        {
            attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - actor.transform.position;
        }
        else
        {
            attackDirection = actor.closestEnemy.transform.position - actor.transform.position;
        }

        float attackAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion arrowRotation = Quaternion.Euler(new Vector3(0, 0, attackAngle));

        var arrowGO = Instantiate(List_InGamePrefabs.GetPrefab(Prefab.Arrow), transform.position, arrowRotation);
        arrowGO.SetActive(true);

        Projectile_Arrow arrow = arrowGO.GetComponent<Projectile_Arrow>();

        arrow.InitialiseArrow(
            attackDirection.normalized, 
            actor.transform.position, 
            actor.CurrentCombatStats, 
            equipmentSlot.ChargeTime,
            equipmentSlot.EquipmentItem.ItemStats.WeaponStats.MaxChargeTime,
            actor.ActorData.FactionData
            );

        if (ability == Ability.ChargedShot)
        {
            VFX_Manager.CreateVFX("Arrow", arrowGO.transform, "Resources_VFXGraphs/ArrowTest");
            arrow.CombatStats = List_Ability.GetAbility(ability).AbilityData.CombatStats;
        }
        else
        {
            VisualEffect arrowVFX = VFX_Manager.CreateVFX("Arrow", arrowGO.transform, "Resources_VFXGraphs/ArrowTrail");
            arrowVFX.SetFloat("Lifetime", equipmentSlot.ChargeTime * 0.25f);
        }
    }
}
