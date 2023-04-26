using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager instance;

    public LayerMask passive;
    public LayerMask playerCanAttack;
    public LayerMask humanCanAttack;
    public LayerMask enemyHumanCanAttack;
    public LayerMask enemyMonsterCanAttack;
    public LayerMask destructable;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        passive = 0;
        playerCanAttack = LayerMask.GetMask("Enemy Human", "Enemy Monster", "Destructable");
        humanCanAttack = LayerMask.GetMask("Enemy Human", "Enemy Monster", "Destructable");
        enemyHumanCanAttack = LayerMask.GetMask("Player", "Human", "Enemy Monster", "Destructable");
        enemyMonsterCanAttack = LayerMask.GetMask("Player","Human", "Enemy Human", "Destructable");
        destructable = 0;
    }

    public LayerMask[] AttackableFactions()
    {
        return new LayerMask[] {passive, playerCanAttack, humanCanAttack, enemyHumanCanAttack, enemyMonsterCanAttack, destructable};
    }
}
