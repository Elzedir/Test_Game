using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Vocation
{
    Farmer,
    LumberJack,
    Miner
}

public class Actor_Vocation : MonoBehaviour
{
    private Dictionary<Vocation, (bool, float)> _vocationList = new();
    private Vocation _vocation;
    private Actor _actor;
    public Vocation Vocation
    {
        get { return _vocation; }
    }

    private void Awake()
    {
        InitialiseVocations();
        LoadVocations()
    }

    private void InitialiseVocations()
    {
        _vocationList[Vocation.Farmer] = (false, 0);
        _vocationList[Vocation.LumberJack] = (false, 0);
        _vocationList[Vocation.Miner] = (false, 0);
    }

    public void SaveVocations()
    {
        // Save logic here
    }

    public void LoadVocations()
    {
        if (TryGetComponent<Actor>(out _actor))
        {
            foreach (KeyValuePair<Vocation, float> vocation in _actor.Vocations)
            {
                if (_vocationList.ContainsKey(vocation.Key))
                {
                    bool discovered = vocation.Value > 0;
                    _vocationList[vocation.Key] = (discovered, vocation.Value);
                    if (discovered)
                    {
                        _vocation = vocation.Key;
                    }
                }
            }
        }
    }

    public int GainVocationExperience(GameObject interactedObject, Actor actor)
    {
        int experience = 0;

        return experience;
    }
}
