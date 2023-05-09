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
    public Equipment_Manager equipmentManager;
    public Inventory_Manager inventory;
    public Manager_Stats statsManager;

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
    protected override LayerMask CanAttack => playerCanAttack;
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
        playerCanAttack = FactionManager.instance.AttackableFactions()[1];
        equipmentManager = GetComponent<Equipment_Manager>();
        inventory = GetComponent<Inventory_Manager>();
        statsManager = GetComponent<Manager_Stats>();
    }
    
    public void SwapSprite(int skinID)
    {
        spriteRenderer.sprite = GameManager.instance.playerSprites[skinID];
    }
    protected override void OnCollide(Collider2D coll)
    {
        
    }
    public void LevelUp()
    {
        maxHitpoint++;
        baseHitpoints = maxHitpoint;
        maxMana++;
        mana = maxMana;
        maxStamina++;
        stamina = maxStamina;
        playerBaseDamage++;
        playerBaseForce++;
        playerBaseSpeed++;
    }
    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
        LevelUp();
    }
    public void Heal(float healthRestore)
    {
        if (baseHitpoints == maxHitpoint)
            return;

        baseHitpoints += healthRestore;
        
        if (baseHitpoints > maxHitpoint)
            baseHitpoints = maxHitpoint;

        GameManager.instance.ShowFloatingText("+" + healthRestore.ToString() + "hp" , 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.instance.HUDBarChange();
    }
    public void Respawn()
    {
        Heal(maxHitpoint);
        dead = false;
        pushDirection = Vector3.zero;
    }
    protected override void ReceiveDamage(Damage dmg)
    {
        base.ReceiveDamage(dmg);
        GameManager.instance.HUDBarChange();
    }

    protected override void Death()
    {
        dead = true;
        foreach (Transform child in transform)
        {
            Player playerChild = child.GetComponent<Player>();
            if (playerChild != null)
            {
                playerChild.SetDead();
            }
        }

        GameManager.instance.deathMenuAnimator.SetTrigger("Show");
    }

    protected virtual void SetDead()
    {
        dead = true;
    }
}
