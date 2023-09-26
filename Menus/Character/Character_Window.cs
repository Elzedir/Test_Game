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

    private TextMeshProUGUI _actorName;
    private TextMeshProUGUI _actorTitle;
    private Image _actorIcon;

    private List<Button> _specialisationButtons;

    private TextMeshProUGUI _actorSpecialisation1;
    private Aspect _specialisation1;
    private TextMeshProUGUI _actorSpecialisation2;
    private Aspect _specialisation2;
    private TextMeshProUGUI _actorSpecialisation3;
    private Aspect _specialisation3;
    private Transform _abilitiesPanel;
    private Transform _abilityList;

    private TextMeshProUGUI _levelNumber;
    private TextMeshProUGUI _healthNumber;
    private TextMeshProUGUI _manaNumber;
    private TextMeshProUGUI _staminaNumber;

    private bool _specialisationButtonPressed = false;

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
        _actorName.text = actor.name;
        _actorTitle.text = List_Aspect.GetCharacterTitle(actor).ToString();
        _actorIcon.sprite = actor.GetComponent<SpriteRenderer>().sprite;

        if (actor.ActorData.ActorAspects.ActorAspectList != null)
        {
            Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);

            _actorSpecialisation1.text = actor.ActorData.ActorAspects.ActorAspectList[0].ToString();
            _specialisation1 = actor.ActorData.ActorAspects.ActorAspectList[0];
            _actorSpecialisation2.text = actor.ActorData.ActorAspects.ActorAspectList[1].ToString();
            _specialisation2 = actor.ActorData.ActorAspects.ActorAspectList[1];
            _actorSpecialisation3.text = actor.ActorData.ActorAspects.ActorAspectList[2].ToString();
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

        _levelNumber.text = actor.ActorData.ActorStats.Level.ToString();
        _healthNumber.text = $"{actor.ActorScripts.StatManager.CurrentCombatStats.MaxHealth} / {actor.ActorData.ActorStats.CombatStats.MaxHealth}";
        _manaNumber.text = $"{actor.ActorScripts.StatManager.CurrentCombatStats.MaxMana} / {actor.ActorData.ActorStats.CombatStats.MaxMana}";
        _staminaNumber.text = $"{actor.ActorScripts.StatManager.CurrentCombatStats.MaxStamina} / {actor.ActorData.ActorStats.CombatStats.MaxStamina}";
    }

    public void SetCharacterStats(GameObject interactedObject)
    {
        
    }

    public void SpecialisationButtonPressed(Aspect specialisation)
    {
        foreach (Transform child in _abilityList)
        {
            Destroy(child.gameObject);
        }

        if (!_specialisationButtonPressed)
        {
            _abilitiesPanel.gameObject.SetActive(true);
            _specialisationButtonPressed = true;
        }

        if (specialisation == Aspect.Vocation)
        {
            // Open vocations
        }
        else
        {
            foreach (List_Ability ability in List_Ability.AllAbilityData)
            {
                if (ability.AbilityData.AbilityStats.AbilitySpecialisation == specialisation)
                {
                    GameObject abilityListIcon = Instantiate(List_InGamePrefabs.GetPrefab(Prefab.AbilityListIcon), _abilityList);
                    // Then fill in the details using the details of the ability
                }
            }
        }
    }
}
