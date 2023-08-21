using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public abstract class Hitbox : MonoBehaviour
{
#region fields

    // Collision
    public ContactFilter2D Filter;
    protected abstract BoxCollider2D Coll { get; }
    protected Collider2D[] hits = new Collider2D[42];

    // Other
    protected Vector3 originalSize;

    #endregion
    protected virtual void Start()
    {
        originalSize = transform.localScale;
    }

    protected virtual void FixedUpdate()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollide(collision.collider);
    }
    protected virtual void OnCollide(Collider2D coll)
    {

    }
}
