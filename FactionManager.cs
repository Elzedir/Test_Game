using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager instance;

    public enum Faction
    {
        Passive,
        Player,
        Human,
        EnemyHuman,
        EnemyMonster,
        Destructable
    }

    public Dictionary<Faction, LayerMask> factionMasks = new();

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
    }

    public void SetFactions()
    {
        factionMasks[Faction.Passive] = 0;
        factionMasks[Faction.Player] = LayerMask.GetMask("Enemy Human", "Enemy Monster");
        factionMasks[Faction.Human] = LayerMask.GetMask("Enemy Human", "Enemy Monster");
        factionMasks[Faction.EnemyHuman] = LayerMask.GetMask("Player", "Human", "Enemy Monster");
        factionMasks[Faction.EnemyMonster] = LayerMask.GetMask("Player", "Human", "Enemy Human");
        factionMasks[Faction.Destructable] = 0;
    }

    private LayerMask GetLayerMask(string[] layers)
    {
        LayerMask mask = LayerMask.GetMask(layers);

        if (mask.value == 0)
        {
            Debug.LogError($"No LayerMask found for layers {string.Join(", ", layers)}");
        }

        return mask;
    }

    public Dictionary<Faction, LayerMask> AttackableFactionList()
    {
        return new Dictionary<Faction, LayerMask>(factionMasks);
    }
}
