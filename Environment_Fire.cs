using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public enum FireType
{
    Temporary,
    Perpetual
}

public class Environment_Fire : MonoBehaviour
{
    public FireType fireType;
    public Collider2D fireCollider;
    
    private Coroutine fireCoroutine;
    private Coroutine burningCoroutine;

    public float temporaryFireLifetime;
    private float fireBuildupTime = 1f;
    private float tickTime = 0.5f;
    private float burnTime = 5f;

    private void Start()
    {
        if (fireType == FireType.Temporary)
        {
            Invoke("DouseFire", temporaryFireLifetime);
        }
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.TryGetComponent<Actor>(out Actor actor))
        {
            EnterFire(actor);
        }
    }

    private void OnTriggerExit2D(Collider2D target)
    {
        if (target.TryGetComponent<Actor>(out Actor actor))
        {
            ExitFire(actor);
        }
    }

    public void DouseFire()
    {
        Destroy(gameObject);
    }
    
    public void EnterFire(Actor target)
    {
        target.inFire = true;

        if (target.isFlammable && !target.onFire)
        {
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
            }

            fireCoroutine = StartCoroutine(FireBuildup(target));
        }
    }

    private IEnumerator FireBuildup(Actor target)
    {
        yield return new WaitForSeconds(fireBuildupTime);

        target.onFire = true;

        if (burningCoroutine != null)
        {
            StopCoroutine(burningCoroutine);
        }

        burningCoroutine = StartCoroutine(Burning(target));
    }

    private IEnumerator Burning(Actor target)
    {
        int tickCount = Mathf.RoundToInt(burnTime / tickTime);

        target.AddOnFireVFX();

        for (int i = 0; i < tickCount; i++)
        {
            Damage damage = new Damage { origin = transform.position, damageAmount = target.baseHealth / 100, pushForce = 0 };
            target.ReceiveDamage(damage);
            yield return new WaitForSeconds(tickTime);
        }

        bool stillBurning = StillBurningCheck(target);

        if (stillBurning)
        {
            burningCoroutine = StartCoroutine(Burning(target));
        }
        else
        {
            target.onFire = false;
            target.RemoveOnFireVFX();
        }
    }

    public bool StillBurningCheck(Actor target)
    {
        bool stillInFire = false;

        if (target.inFire && target.onFire)
        {
            stillInFire = true;
        }

        return stillInFire;
    }

    public void ExitFire(Actor target)
    {
        target.inFire = false;

        if (target.onFire)
        {
            if (burningCoroutine != null)
            {
                StopCoroutine(burningCoroutine);
            }

            burningCoroutine = StartCoroutine(Burning(target));
        }
        else
        {
            StopCoroutine(fireCoroutine);
        }
    }
}
