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

        SetFactions();
        AttackableFactionList();
    }

    public LayerMask[] AttackableFactions()
    {
        return new LayerMask[] {passive, playerCanAttack, humanCanAttack, enemyHumanCanAttack, enemyMonsterCanAttack, destructable};
    }

    public void SetFactions()
    {
        passive = 0;
        playerCanAttack = LayerMask.GetMask("Enemy Human", "Enemy Monster");
        humanCanAttack = LayerMask.GetMask("Enemy Human", "Enemy Monster");
        enemyHumanCanAttack = LayerMask.GetMask("Player", "Human", "Enemy Monster");
        enemyMonsterCanAttack = LayerMask.GetMask("Player", "Human", "Enemy Human");
        destructable = 0;
    }

    public void AttackableFactionList()
    {
        LayerMask[] attackableFactions = AttackableFactions();
        {
            LayerMask passiveCanAttack = attackableFactions[0];
            LayerMask playerCanAttack = attackableFactions[1];
            LayerMask humanCanAttack = attackableFactions[2];
            LayerMask enemyHumanCanAttack = attackableFactions[3];
            LayerMask enemyMonsterCanAttack = attackableFactions[4];
            LayerMask destructable = attackableFactions[5];
        }
    }
}
