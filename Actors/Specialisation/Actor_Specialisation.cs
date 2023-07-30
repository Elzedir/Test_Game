using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Specialisation
{
    Archer,
    Knight,
    Wizard
}

public enum Title
{
    Hexblade,
    Witcher,
    Alchemist,
    Wanderer,
    Brightbow
}

public abstract class Actor_Specialisation : MonoBehaviour
{
    private Dictionary<HashSet<Specialisation>, Title> titleMappings = new Dictionary<HashSet<Specialisation>, Title>();
    private List<Specialisation> _specialisationList = new();
    private Specialisation _specialisation;
    private string _titleName;
    public Specialisation Specialisation
    {
        get { return _specialisation; }
    }
    public string TitleName
    {
        get { return _titleName; } 
    }

    private void Awake()
    {
        InitialiseTitles();
    }

    private void InitialiseTitles()
    {
        titleMappings[new HashSet<Specialisation> 
        { Specialisation.Knight, Specialisation.Wizard }] 
            = Title.Hexblade;

        titleMappings[new HashSet<Specialisation> 
        { Specialisation.Archer, Specialisation.Knight, Specialisation.Wizard }] 
            = Title.Witcher;

        titleMappings[new HashSet<Specialisation> 
        { Specialisation.Knight, Specialisation.Wizard }] 
            = Title.Alchemist;

        titleMappings[new HashSet<Specialisation> 
        { Specialisation.Knight, Specialisation.Archer }] 
            = Title.Wanderer;

        titleMappings[new HashSet<Specialisation> 
        { Specialisation.Archer, Specialisation.Wizard }] 
            = Title.Brightbow;
    }

    public void SetSpecialisation(Specialisation specialisation)
    {
        if (_specialisationList.Count < 3)
        {
            if (!_specialisationList.Contains(specialisation))
            {
                _specialisationList.Add(specialisation);
            }
        }
        else
        {
            Debug.Log("Can only have three specialisations");
        }

        RenameTitle();
    }

    private void RenameTitle()
    {
        HashSet<Specialisation> currentSpecialisations = new HashSet<Specialisation>(_specialisationList);

        if (titleMappings.TryGetValue(currentSpecialisations, out Title title))
        {
            _titleName = title.ToString();
        }
        else
        {
            _titleName = "Unknown";
        }
    }
}
