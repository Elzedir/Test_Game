using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Inventory_EquipmentSlot
{
    protected override void Start()
    {
        // AttackAnimation = AxeAttackAnimation;
    }

    private void AxeAttackAnimation(Animator animator)
    {
        animator.SetTrigger("AxeAttack");
    }
}
