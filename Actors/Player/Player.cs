using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : Actor
{
    // References
    

    protected BoxCollider2D playerColl;
    protected Animator anim;
    protected Rigidbody2D playerBody;

    // Attack
    protected int playerBaseDamage;
    protected float playerBaseSpeed;
    protected float playerBaseForce;
    protected LayerMask playerCanAttack;

    public GameObject playerWeapon;
    public Transform sheathedPosition;
    protected override BoxCollider2D Coll => playerColl;
    protected override BoxCollider2D ActorColl => playerColl;
    protected override Rigidbody2D Rigidbody2D => playerBody;

    //States

    

    // Upgrade structure


    protected SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerColl = GetComponent<BoxCollider2D>();
        playerBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (dead)
        {
            Death();
        }
    }
    public void SwapSprite(int skinID)
    {
        spriteRenderer.sprite = GameManager.Instance.playerSprites[skinID];
    }
    protected override void OnCollide(Collider2D coll)
    {
        
    }
    public void LevelUp()
    {
        baseHealth++;
        baseMana++;
        baseStamina++;
        playerBaseDamage++;
        playerBaseForce++;
        playerBaseSpeed++;
        statManager.UpdateStatsOnLevelUp();
    }
    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
        LevelUp();
    }
    
    public void Respawn()
    {
        statManager.RestoreHealth(statManager.maxHealth);
        dead = false;
        pushDirection = Vector3.zero;
    }
    protected override void Death()
    {
        foreach (Transform child in transform)
        {
            Player playerChild = child.GetComponent<Player>();

            if (playerChild != null)
            {
                playerChild.dead = true;
            }
        }

        GameManager.Instance.deathMenuAnimator.SetTrigger("Show");
    }
    public GameObject GetClosestNPC()
    {
        return closestNPC;
    }
}
