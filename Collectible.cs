using UnityEngine;

public class Collectible : Hitbox
{
    protected BoxCollider2D collectColl;
    protected override BoxCollider2D Coll => collectColl;
    // Logic
    protected bool collected;

    protected override void Start()
    {
        base.Start();
        collectColl = GetComponent<BoxCollider2D>();
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            OnCollect();
    }

    protected virtual void OnCollect()
    {
        Debug.Log("Collection not implemented");
    }
}
