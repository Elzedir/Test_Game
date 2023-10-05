using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class VFX_Manager : MonoBehaviour
{
    public static VFX_Manager Instance;

    public GameObject onFirePrefab;
    public GameObject onFire;
    public bool onFireExists = false;

    public GameObject firePrefab;

    public void Start()
    {
        Instance = this;
    }

    public void AddOnFireVFX(Actor_Base actor, Transform VFXArea)
    {
        if (!onFireExists)
        {
            onFire = Instantiate(onFirePrefab, VFXArea);
            onFireExists = true;
        }
    }
    public void RemoveOnFireVFX(Actor_Base actor, Transform VFXArea)
    {
        if (onFireExists)
        {
            Destroy(onFire);
            onFireExists = false;
        }
    }
    public static VisualEffect CreateVFX(string name, Transform parent, string location, float destroyTimer = 0)
    {
        GameObject vfxChild = new GameObject($"{name}VFX");
        vfxChild.transform.SetParent(parent);
        vfxChild.transform.localPosition = Vector3.zero;
        vfxChild.transform.localRotation = Quaternion.identity;
        vfxChild.transform.localScale = parent.localScale;

        VisualEffect arrowVFX = vfxChild.AddComponent<VisualEffect>();
        arrowVFX.visualEffectAsset = Resources.Load<VisualEffectAsset>(location);
        arrowVFX.AddComponent<SortingGroup>().sortingLayerName = "VFX";

        if (destroyTimer != 0)
        {
            GameManager.Instance.RunCoroutine(VFX_Manager.Instance.RemoveVFX(vfxChild, destroyTimer));
        }

        return arrowVFX;
    }

    public IEnumerator RemoveVFX(GameObject vfx, float destroyTimer)
    {
        yield return new WaitForSeconds(destroyTimer);
        Destroy(vfx);
    }
}
