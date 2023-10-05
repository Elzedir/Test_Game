using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile_Arrow : Projectile
{
    private GameObject stuckTarget = null;

    public void InitialiseArrow(
        Vector2 direction,
        Vector3 origin,
        CombatStats combatStats,
        float chargeTime,
        float maxChargeTime,
        Faction_Data_SO projectileFaction
        )
    {
        this.Direction = direction;
        this.Origin = origin;
        this.CombatStats = combatStats;
        this.ChargeTime = chargeTime;
        this.MaxChargeTime = maxChargeTime;
        this.ProjectileFaction = projectileFaction;
    }

    protected override void FixedUpdate()
    {
        if (isActiveAndEnabled)
        {
            if (_hitEnemies == null)
            {
                _hitEnemies = new HashSet<Collider2D>();
            }

            CollideCheck();

            if (stuckTarget != null)
            {
                transform.position = stuckTarget.transform.position;
                return;
            }

            float distanceFromOrigin = Vector3.Distance(Origin, transform.position);

            if (_rb != null)
            {
                if (distanceFromOrigin >= Mathf.Max(CombatStats.AttackRange * (ChargeTime / MaxChargeTime), 0.2f) || _hasLanded)
                {
                    GetComponentInChildren<VisualEffect>().SetFloat("Lifetime", 0);
                    DespawnArrow();
                    // Play the hit animation.
                    Invoke("DestroyProjectile", 5f);
                }
                else
                {
                    _rb.velocity = Direction * Mathf.Max(CombatStats.AttackSwingTime * (ChargeTime / MaxChargeTime), 0.5f);
                }

                if (_rb.velocity.magnitude < 0.1f)
                {
                    DespawnArrow();
                }
            }
        }
    }

    protected override void CollideCheck()
    {
        base.CollideCheck();

        if (_hitEnemy)
        {

            DespawnArrow();
        }
    }

    private void DespawnArrow()
    {
        _hasLanded = true;
        Destroy(_collider);
        Destroy(_rb);

        if (stuckTarget != null)
        {
            transform.parent = stuckTarget.transform;
        }
        _hitEnemy = false;
    }
}
