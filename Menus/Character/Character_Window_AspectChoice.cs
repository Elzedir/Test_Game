using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character_Window_AspectChoice : MonoBehaviour
{
    public static Character_Window_AspectChoice Instance;
    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        gameObject.SetActive(false);
    }

    public void Open(Actor_Base actor)
    {
        gameObject.SetActive(true);

        foreach (Transform child in transform)
        {
            Destroy(child);
        }

        Array aspectEnums = Enum.GetValues(typeof(Aspect));
        List<Aspect> aspects = new List<Aspect>(aspectEnums.Length);

        foreach (Aspect aspectEnum in aspectEnums)
        {
            aspects.Add(aspectEnum);
        }

        foreach (Aspect aspect in aspects)
        {
            bool aspectFound = false;

            foreach (Aspect actorAspect in actor.ActorData.ActorAspects.ActorAspectList)
            {
                if (actorAspect == aspect)
                {
                    aspectFound = true;
                    break;
                }
            }

            if (!aspectFound)
            {
                GameObject aspectGO = Instantiate(List_InGamePrefabs.GetPrefab(Prefab.AspectChoice), transform);
                aspectGO.SetActive(true);
                aspectGO.name = aspect.ToString();
                aspectGO.GetComponentInChildren<TextMeshProUGUI>().text = aspect.ToString();
            }
        }
    }
}
