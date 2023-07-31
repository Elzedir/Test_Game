using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public enum Vocation
{
    Farmer,
    LumberJack,
    Miner
}

public class Actor_Vocation : MonoBehaviour
{
    private Actor_Stats _actorStats;
    private Actor_Base _actor;

    private void Awake()
    {
        _actorStats = GetComponent<Actor_Base>().ActorData.ActorStats;
        InitialiseVocations();
        LoadVocations();
    }

    private void InitialiseVocations()
    {
        foreach (Vocation vocation in Enum.GetValues(typeof(Vocation)))
        {
            if (!_actorStats.VocationExperience.Exists(entry => entry.vocation == vocation))
            {
                _actorStats.VocationExperience.Add(new Actor_Vocation_Entry { vocation = vocation, experience = 0 });
            }
        }
    }

    public void SaveVocations()
    {
        // Save logic here
    }

    public void LoadVocations()
    {
        // Load logic here
    }

    public void GainVocationExperience(Vocation vocation, int amount)
    {
        Actor_Vocation_Entry entry = _actorStats.VocationExperience.Find(e => e.vocation == vocation);
        if (entry != null)
        {
            entry.experience += amount;
        }
    }
}
