using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Equipment_Weapon
{
    protected override void Start()
    {
        AttackAnimation = SpearAttackAnimation;
    }

    private void SpearAttackAnimation(Animator animator)
    {
        animator.SetTrigger("ShortswordAttack");
    }
}
