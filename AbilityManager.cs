using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    protected float lastCharge = 0;

    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();

    public virtual void Start()
    {
        instance = this;
        cooldowns.Add("Charge", chargeCooldown);
    }

    public virtual void FixedUpdate()
    {
        foreach (KeyValuePair<string, float> cooldown in cooldowns)
        {
            if (cooldown.Value < 0)
            {
                cooldowns[cooldown.Key] = 0;
            }
            else
            {
                cooldowns[cooldown.Key] -= Time.fixedDeltaTime;
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
            Debug.Log(chargeAvailable);

            if (withinChargeDistance && chargeAvailable)
            {
                Vector2 chargeDirection = (target.transform.position - transform.position);
                float chargeForce = Mathf.Clamp(distanceToTarget, 0f, maxCharge);
                Vector2 charge = chargeDirection * chargeForce;
                self.AddForce(charge, ForceMode2D.Impulse);
                lastCharge = 0;
                Debug.Log(this.name + "Charged");
            }
        }
    }

}
