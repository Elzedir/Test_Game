using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireType
{
    Temporary,
    Perpetual
}

public class Environment_Fire : MonoBehaviour
{
    public FireType fireType;
    public float temporaryFireLifetime = 10;
    public Collider2D fireCollider;

    private void Start()
    {
        if (fireType == FireType.Temporary)
        {
            Invoke("DouseFire", temporaryFireLifetime);
        }
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.TryGetComponent<Actor_Base>(out Actor_Base actor))
        {
            Environment_Fire_Manager.instance.EnterFire(actor);
        }
    }

    private void OnTriggerExit2D(Collider2D target)
    {
        if (target.TryGetComponent<Actor_Base>(out Actor_Base actor))
        {
            Environment_Fire_Manager.instance.ExitFire(actor);
        }
    }

    public void DouseFire()
    {
        Destroy(gameObject);
    }
}
