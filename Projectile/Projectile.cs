using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public Vector2 Direction;
    public Vector3 Origin;
    public CombatStats CombatStats;
    public float ChargeTime;
    public LayerMask AttackableLayers;
    protected Rigidbody2D _rb;
    protected Collider2D _collider;
    protected bool _hasLanded = false;
    protected HashSet<Collider2D> _hitEnemies;
    protected bool _hitEnemy = false;

    protected virtual void Start()
    {
        Origin = transform.position;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        
    }

    protected void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    protected virtual void CollideCheck()
    {
        if (_collider != null)
        {
            Collider2D[] results = new Collider2D[42];
            ContactFilter2D filter = new ContactFilter2D();

            AttackableLayers |= LayerMask.GetMask("Blocking");

            filter.layerMask = AttackableLayers;
            filter.useLayerMask = true;

            int numColliders = Physics2D.OverlapCollider(_collider, filter, results);

            for (int i = 0; i < numColliders; i++)
            {
                Collider2D hit = results[i];

                if (hit.GetComponent<Equipment_Slot>() != null)
                {
                    continue;
                }

                if (hit.gameObject.layer == gameObject.layer || !hit.enabled || _hitEnemies.Contains(hit))
                    continue;

                if ((LayerMask.GetMask("Blocking") & (1 << hit.gameObject.layer)) != 0)
                {
                    _hasLanded = true;
                    _rb.velocity = Vector2.zero;
                    Invoke("DestroyProjectile", 5f);
                }
                else
                {
                    if (!_hasLanded)
                    {
                        int targetLayerMask = 1 << hit.gameObject.layer;

                        if ((AttackableLayers & targetLayerMask) != 0)
                        {
                            Damage damage = Manager_Stats.DealDamage(damageOrigin: Origin, combatStats: CombatStats, chargeTime: ChargeTime);
                            hit.SendMessage("ReceiveDamage", damage);
                            _hitEnemy = true;
                        }
                    }
                }

                _hitEnemies.Add(hit);
            }
        }
    }
}
