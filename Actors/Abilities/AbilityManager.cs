using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Equipment_Manager;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager instance;

    // Charge

    [SerializeField] protected bool canCharge = false;
    [SerializeField] protected float maxCharge;
    [SerializeField] protected float minCharge;
    [SerializeField] protected float chargeCooldown = 3.0f;
    protected bool withinChargeRange;
    protected bool charging;
    protected float immuneTime;
    protected float lastImmune;

    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();

    public virtual void Start()
    {
        instance = this;
        cooldowns.Add("Charge", chargeCooldown);
    }

    public virtual void FixedUpdate()
    {
        List<string> keys = new List<string>(cooldowns.Keys);

        foreach (string key in keys)
        {
            if (cooldowns[key] < 0)
            {
                cooldowns[key] = 0;
            }
            else
            {
                cooldowns[key] -= Time.fixedDeltaTime;
            }
        }
    }

    public virtual void Charge(GameObject target, Rigidbody2D self)
    {
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            bool withinChargeDistance = distanceToTarget < maxCharge && distanceToTarget > minCharge;
            bool chargeAvailable = cooldowns["Charge"] <= 0;

            if (withinChargeDistance && chargeAvailable)
            {
                cooldowns["Charge"] = chargeCooldown;
                Vector2 chargeDirection = target.transform.position - transform.position;
                float chargeForce = distanceToTarget;
                Vector2 charge = chargeDirection * chargeForce; // * chargeSpeedMultiplier
                self.AddForce(charge, ForceMode2D.Impulse);
                Debug.Log(this.name + "Charged");
            }

            if (distanceToTarget > maxCharge)
            {
                self.velocity = Vector2.zero;
            }

            else if (distanceToTarget < minCharge)
            {
                self.velocity = Vector2.zero;
            }
        }
    }

    //public virtual void Invulnerability()
    //{
    //    if (!dead)
    //    {
    //        if (Time.time - lastImmune > immuneTime)
    //        {
    //            lastImmune = Time.time;
    //            hitpoint -= dmg.damageAmount;
    //            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
    //            GameManager.instance.ShowFloatingText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.up * 30, 0.5f);

    //            if (hitpoint <= 0)
    //            {
    //                hitpoint = 0;
    //                Death();
    //            }
    //        }
    //    }
    //}
    
}
