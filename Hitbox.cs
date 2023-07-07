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
        CollideCheck();
    }
    protected virtual void CollideCheck()
    {
        int numHits = Coll.Overlap(Filter, hits);

        if (numHits == 0)
        {
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
            {
                continue;
            }

            OnCollide(hits[i]);

            hits[i] = null;
        }
    }
    protected virtual void OnCollide(Collider2D coll)
    {
        Debug.Log("Collision was not implemented for " + this.name);
    }

}
