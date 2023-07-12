using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Manager : MonoBehaviour
{
    public static VFX_Manager instance;

    public GameObject onFirePrefab;
    public GameObject onFire;
    public bool onFireExists = false;

    public GameObject firePrefab;

    public void Start()
    {
        instance = this;
    }

    public void AddOnFireVFX(Actor actor, Transform VFXArea)
    {
        if (!onFireExists)
        {
            onFire = Instantiate(onFirePrefab, VFXArea);
            onFireExists = true;
        }
    }
    public void RemoveOnFireVFX(Actor actor, Transform VFXArea)
    {
        if (onFireExists)
        {
            Destroy(onFire);
            onFireExists = false;
        }
    }
}
