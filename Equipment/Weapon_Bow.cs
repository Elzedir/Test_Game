using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Bow : Weapon
{
    public override void WeaponAttack()
    {
        Shoot();
    }

    private void Shoot()
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
        arrow.Range = equipmentSlot.ItemStats.WeaponStats.ItemRange;
        arrow.Speed = equipmentSlot.ItemStats.WeaponStats.ItemSpeed;
    }
}
