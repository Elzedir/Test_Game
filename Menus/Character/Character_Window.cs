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

    [SerializeField] private TextMeshProUGUI _actorName;
    [SerializeField] private TextMeshProUGUI _actorTitle;
    [SerializeField] private Image _actorIcon;
    [SerializeField] private TextMeshProUGUI _actorSpecialisation1;
    [SerializeField] private TextMeshProUGUI _actorSpecialisation2;
    [SerializeField] private TextMeshProUGUI _actorSpecialisation3;

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
    }

    public override void OpenMenu(GameObject interactedCharacter = null)
    {
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

    public override void CloseMenu()
    {
        gameObject.SetActive(false);
        _isOpen = false;
    }

    public void SetCharacterWindowDetails(Actor_Base actor)
    {
        // Eventually put in an ability to change the font of the journal depending on the actor.
        _actorName.text = actor.name;
        _actorTitle.text = List_Specialisation.GetCharacterTitle(actor).ToString();
        _actorIcon.sprite = actor.GetComponent<SpriteRenderer>().sprite;

        if (actor.ActorData.ActorSpecialisations.ActorSpecialisations != null)
        {
            _actorSpecialisation1.text = actor.ActorData.ActorSpecialisations.ActorSpecialisations[0].ToString();
            _actorSpecialisation2.text = actor.ActorData.ActorSpecialisations.ActorSpecialisations[1].ToString();
            _actorSpecialisation3.text = actor.ActorData.ActorSpecialisations.ActorSpecialisations[2].ToString();
        }
    }

    public void SetCharacterStats(GameObject interactedObject)
    {
        
    }
}
