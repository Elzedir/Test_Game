using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile_Arrow : Projectile
{
    private GameObject stuckTarget = null;

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
                transform.position = stuckTarget.transform.position;  // Follow the stuck target
                return;
            }

            float distanceFromOrigin = Vector3.Distance(Origin, transform.position);

            if (distanceFromOrigin >= Range || _hasLanded)
            {
                _hasLanded = true;
                _rb.velocity = Vector2.zero;
                // Play the hit animation.
                Invoke("DestroyProjectile", 5f);
            }
            else
            {
                _rb.velocity = Direction * Speed;
            }

            if (_rb.velocity.magnitude < 0.1f)
            {
                _hasLanded = true;
                Invoke("DestroyProjectile", 5f);
            }
        }
    }

    protected override void CollideCheck()
    {
        base.CollideCheck();

        if (_hitEnemy)
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
}
