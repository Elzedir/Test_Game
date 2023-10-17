using System;
using UnityEngine;

public enum Vocation
{
    Farmer,
    LumberJack,
    Miner
}

public class Actor_Vocation : MonoBehaviour
{
    private Actor_Base _actor;

    private void Awake()
    {
        InitialiseVocations();
        LoadVocations();
    }

    private void InitialiseVocations()
    {
        foreach (Vocation vocation in Enum.GetValues(typeof(Vocation)))
        {
            if (!_actor.ActorData.ActorStats.VocationExperience.Exists(entry => entry.vocation == vocation))
            {
                _actor.ActorData.ActorStats.VocationExperience.Add(new Actor_Vocation_Entry { vocation = vocation, experience = 0 });
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
        Actor_Vocation_Entry entry = _actor.ActorData.ActorStats.VocationExperience.Find(e => e.vocation == vocation);
        if (entry != null)
        {
            entry.experience += amount;
        }
    }
}
