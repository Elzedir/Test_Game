using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shortsword : Inventory_EquipmentSlot
{
    protected override void Start()
    {
        // AttackAnimation = ShortswordAttackAnimation;
    }

    private void ShortswordAttackAnimation(Animator animator)
    {
        animator.SetTrigger("ShortswordBasicAttack");
    }
}
