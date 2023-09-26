using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile_Arrow : Projectile
{
    private GameObject stuckTarget = null;

    public Projectile_Arrow(
        Vector2 direction,
        Vector3 origin,
        CombatStats combatStats,
        float chargeTime,
        LayerMask attackableLayers
        )
    {
        this.Direction = direction;
        this.Origin = origin;
        this.CombatStats = combatStats;
        this.ChargeTime = chargeTime;
        this.AttackableLayers = attackableLayers;
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
                if (distanceFromOrigin >= CombatStats.AttackRange || _hasLanded)
                {
                    DespawnArrow();
                    // Play the hit animation.
                    Invoke("DestroyProjectile", 5f);
                }
                else
                {
                    _rb.velocity = Direction * CombatStats.AttackSwingTime;
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
