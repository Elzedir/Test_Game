using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shortsword : Equipment_Slot
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
