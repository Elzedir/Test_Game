using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Environment_Fire_Manager : MonoBehaviour
{
    public static Environment_Fire_Manager instance;
    public Transform environmentFireArea;

    private Dictionary<Actor, Coroutine> fireCoroutines = new();

    public GameObject firePrefab;

    private float fireBuildupTime = 0.5f;
    private float tickTime = 0.5f;
    private float burnTime = 5f;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple instances of Environment_Fire_Manager!");
        }

        instance = this;
    }
    
    public void EnterFire(Actor target)
    {
        target.inFire = true;

        if (target.isFlammable && !target.onFire && !fireCoroutines.ContainsKey(target))
        {
            Coroutine fireCoroutine = StartCoroutine(FireBuildup(target));
            fireCoroutines.Add(target, fireCoroutine);
        }
    }

    private IEnumerator FireBuildup(Actor target)
    {
        yield return new WaitForSeconds(fireBuildupTime);

        target.onFire = true;
        target.AddOnFireVFX();

        fireCoroutines[target] = StartCoroutine(Burning(target));
    }

    private IEnumerator Burning(Actor target)
    {
        int tickCount = Mathf.RoundToInt(burnTime / tickTime);

        for (int i = 0; i < tickCount; i++)
        {
            Damage damage = new Damage { origin = transform.position, damageAmount = target.baseHealth / 100, pushForce = 0 };
            target.ReceiveDamage(damage);
            yield return new WaitForSeconds(tickTime);
        }

        if (target.inFire)
        {
            fireCoroutines[target] = StartCoroutine(Burning(target));
        }
        else
        {
            target.onFire = false;
            target.RemoveOnFireVFX();
            fireCoroutines.Remove(target);
        }
    }

    public void ExitFire(Actor target)
    {
        target.inFire = false;

        if (target.onFire && !fireCoroutines.ContainsKey(target))
        {
            Coroutine fireCoroutine = StartCoroutine(Burning(target));
            fireCoroutines.Add(target, fireCoroutine);
        }
        else
        {
            StartCoroutine(DelayedFireCheck(target));
        }
    }

    private IEnumerator DelayedFireCheck(Actor target)
    {
        yield return new WaitForSeconds(0.25f);

        if (target.inFire && !target.onFire)
        {
            fireCoroutines[target] = StartCoroutine(Burning(target));
        }
    }

    private void CreateFire(Vector3 position)
    {        
        GameObject fire = Instantiate(firePrefab, position, Quaternion.identity, environmentFireArea);
        Environment_Fire fireComponent = fire.GetComponent<Environment_Fire>();
        
        fireComponent.fireType = FireType.Temporary;
        fireComponent.temporaryFireLifetime = 10;
        // Be able to change the temporaryFireLifeTime through whatever you do.
    }
}
