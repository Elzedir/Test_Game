using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Character_Window : Menu_UI
{
    public static Character_Window Instance;

    Actor_Base _actor;

    TextMeshProUGUI _actorName;
    TextMeshProUGUI _actorTitle;
    Image _actorIcon;

    List<Button> _specialisationButtons;

    TextMeshProUGUI _actorSpecialisation1;
    Aspect _specialisation1;
    TextMeshProUGUI _actorSpecialisation2;
    Aspect _specialisation2;
    TextMeshProUGUI _actorSpecialisation3;
    Aspect _specialisation3;
    Transform _abilitiesPanel;
    Transform _abilityList;
    Transform _aspectChoicePanel;

    TextMeshProUGUI _levelNumber;
    TextMeshProUGUI _healthNumber;
    TextMeshProUGUI _manaNumber;
    TextMeshProUGUI _staminaNumber;

    bool _specialisationButtonPressed = false;

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        InitialiseComponents();

        gameObject.SetActive(false);
        _abilitiesPanel.gameObject.SetActive(false);
    }

    public void InitialiseComponents()
    {
        Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);

        _actorName = allChildren.FirstOrDefault(t => t.name == "ActorName").GetComponent<TextMeshProUGUI>();
        _actorTitle = allChildren.FirstOrDefault(t => t.name == "ActorTitle").GetComponent<TextMeshProUGUI>();
        _actorSpecialisation1 = allChildren.FirstOrDefault(t => t.name == "ActorSpecialisation1").GetComponent<TextMeshProUGUI>();
        _actorSpecialisation2 = allChildren.FirstOrDefault(t => t.name == "ActorSpecialisation2").GetComponent<TextMeshProUGUI>();
        _actorSpecialisation3 = allChildren.FirstOrDefault(t => t.name == "ActorSpecialisation3").GetComponent<TextMeshProUGUI>();
        _actorIcon = allChildren.FirstOrDefault(t => t.name == "ActorIcon").GetComponent<Image>();
        _abilitiesPanel = allChildren.FirstOrDefault(t => t.name == "AbilitiesPanel");
        _abilityList = allChildren.FirstOrDefault(t => t.name == "AbilityList");
        _levelNumber = allChildren.FirstOrDefault(t => t.name == "LevelNumber").GetComponent<TextMeshProUGUI>();
        _healthNumber = allChildren.FirstOrDefault(t => t.name == "HealthNumber").GetComponent<TextMeshProUGUI>();
        _manaNumber = allChildren.FirstOrDefault(t => t.name == "ManaNumber").GetComponent<TextMeshProUGUI>();
        _staminaNumber = allChildren.FirstOrDefault(t => t.name == "StaminaNumber").GetComponent<TextMeshProUGUI>();
    }

    public override void OpenMenu(GameObject interactedCharacter = null)
    {
        if (_specialisationButtons == null)
        {
            _specialisationButtons = new List<Button>();
        }
        else
        {
            _specialisationButtons.Clear();
        }

        if (interactedCharacter == null)
        {
            interactedCharacter = GameManager.Instance.Player.gameObject;
        }

        Actor_Base interactedActor = interactedCharacter.GetComponent<Actor_Base>();

        gameObject.SetActive(true);
        _isOpen = true;

        SetCharacterWindowDetails(interactedActor);
        SetCharacterStats(interactedCharacter);
    }

    public override void CloseMenu(GameObject interactedObject = null)
    {
        _actor = null;
        gameObject.SetActive(false);
        _abilitiesPanel.gameObject.SetActive(false);
        _specialisationButtonPressed = false;
        _isOpen = false;

        foreach (var button in _specialisationButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    public void SetCharacterWindowDetails(Actor_Base actor)
    {
        // Eventually put in an ability to change the font of the journal depending on the actor.
        _actor = actor;
        _actorName.text = actor.name;
        _actorTitle.text = $"Level {actor.ActorData.ActorStats.ActorLevelData.Level} {List_Aspect.GetCharacterTitle(actor).ToString()}";
        _actorIcon.sprite = actor.GetComponent<SpriteRenderer>().sprite;

        if (actor.ActorData.ActorAspects.ActorAspectList != null)
        {
            Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);

            if (actor.ActorData.ActorStats.ActorLevelData.Level >= 1)
            {
                _actorSpecialisation1.text = actor.ActorData.ActorAspects.ActorAspectList[0].ToString();
            }
            else
            {
                _actorSpecialisation1.text = "";
            }

            if (actor.ActorData.ActorStats.ActorLevelData.Level >= 4 && actor.ActorData.ActorStats.ActorLevelData.CanAddSkillSet)
            {
                _actorSpecialisation2.text = actor.ActorData.ActorAspects.ActorAspectList[1].ToString();
            }
            else
            {
                _actorSpecialisation2.text = "";
            }

            if (actor.ActorData.ActorStats.ActorLevelData.Level >= 8 && actor.ActorData.ActorStats.ActorLevelData.CanAddSkillSet)
            {
                _actorSpecialisation3.text = actor.ActorData.ActorAspects.ActorAspectList[2].ToString();
            }
            else
            {
                _actorSpecialisation3.text = "";
            }

            _specialisation1 = actor.ActorData.ActorAspects.ActorAspectList[0];
            _specialisation2 = actor.ActorData.ActorAspects.ActorAspectList[1];
            _specialisation3 = actor.ActorData.ActorAspects.ActorAspectList[2];

            _specialisationButtons.Add(allChildren.FirstOrDefault(t => t.name == "ActorSpecialisationButton1").GetComponent<Button>());
            _specialisationButtons.Add(allChildren.FirstOrDefault(t => t.name == "ActorSpecialisationButton2").GetComponent<Button>());
            _specialisationButtons.Add(allChildren.FirstOrDefault(t => t.name == "ActorSpecialisationButton3").GetComponent<Button>());
            _specialisationButtons.Add(allChildren.FirstOrDefault(t => t.name == "ActorVocationsButton").GetComponent<Button>());

            _specialisationButtons[0].onClick.AddListener(() => SpecialisationButtonPressed(_specialisation1));
            _specialisationButtons[1].onClick.AddListener(() => SpecialisationButtonPressed(_specialisation2));
            _specialisationButtons[2].onClick.AddListener(() => SpecialisationButtonPressed(_specialisation3));
            _specialisationButtons[3].onClick.AddListener(() => SpecialisationButtonPressed(Aspect.Vocation));
        }

        _levelNumber.text = actor.ActorData.ActorStats.ActorLevelData.Level.ToString();
        _healthNumber.text = $"{actor.CurrentCombatStats.CurrentHealth} / {actor.CurrentCombatStats.MaxHealth}";
        _manaNumber.text = $"{actor.CurrentCombatStats.CurrentMana} / {actor.CurrentCombatStats.MaxMana}";
        _staminaNumber.text = $"{actor.CurrentCombatStats.CurrentStamina} / {actor.CurrentCombatStats.MaxStamina}";
    }

    public void SetCharacterStats(GameObject interactedObject)
    {
        
    }

    public void SpecialisationButtonPressed(Aspect aspect)
    {
        if (aspect == Aspect.None)
        {
            if (_actor != null && _actor.ActorData.ActorStats.ActorLevelData.CanAddSkillSet)
            {
                Character_Window_AspectChoice.Instance.Open(_actor);
                return;
            }
            else
            {
                return;
            }
        }

        foreach (Transform child in _abilityList)
        {
            Destroy(child.gameObject);
        }

        if (!_specialisationButtonPressed)
        {
            _abilitiesPanel.gameObject.SetActive(true);
            _specialisationButtonPressed = true;
        }

        if (aspect == Aspect.Vocation)
        {
            // Open vocations
        }
        else
        {
            foreach (List_Ability ability in List_Ability.AllAbilityData)
            {
                if (ability.AbilityData.AbilityStats.AbilitySpecialisation == aspect)
                {
                    GameObject abilityListIcon = Instantiate(List_InGamePrefabs.GetPrefab(Prefab.AbilityListIcon), _abilityList);
                    // Then fill in the details using the details of the ability
                }
            }
        }
    }
}
