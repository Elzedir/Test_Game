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
        arrow.Actor = actor;
        arrow.ChargeTime = equipmentSlot.ChargeTime;
        arrow.Direction = attackDirection.normalized;
        arrow.ItemStats = equipmentSlot.EquipmentItem.ItemStats;

        if (ability == Ability.ChargedShot)
        {
            GameObject vfxChild = new GameObject("ArrowVFX");
            vfxChild.transform.SetParent(arrowGO.transform);
            vfxChild.transform.localPosition = Vector3.zero;
            vfxChild.transform.localRotation = Quaternion.identity;

            VisualEffect arrowVFX = vfxChild.AddComponent<VisualEffect>();
            arrowVFX.visualEffectAsset = Resources.Load<VisualEffectAsset>("Resources_VFXGraphs/ArrowTest");
            arrowVFX.AddComponent<SortingGroup>().sortingLayerName = "VFX";

            arrow.ItemStats.CombatStats = List_Ability.GetAbility(ability).AbilityData.CombatStats;
            arrow.ItemStats.WeaponStats = List_Ability.GetAbility(ability).AbilityData.WeaponStats;
        }
    }
}
