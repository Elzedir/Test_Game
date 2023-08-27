using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Character_Window : Menu_UI
{
    public static Character_Window Instance;

    public TextMeshProUGUI NameText;
    public Image ActorIcon;
    private Actor_Base _actor;

    public void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameObject.SetActive(false);
    }

    public override void OpenMenu(GameObject interactedCharacter = null)
    {
        if (interactedCharacter == null)
        {
            interactedCharacter = GameManager.Instance.Player.gameObject;
        }

        gameObject.SetActive(true);
        _isOpen = true;

        SetCharacterWindowName(interactedCharacter.name);
        SetCharacterWindowIcon(interactedCharacter.GetComponent<SpriteRenderer>().sprite);
        SetCharacterStats(interactedCharacter);
    }

    public override void CloseMenu()
    {
        gameObject.SetActive(false);
        _isOpen = false;
    }

    public void SetCharacterWindowName(string name)
    {
        // Eventually put in an ability to change the font of the journal depending on the actor.
        NameText.text = name + "'s Journal";
    }

    public void SetCharacterWindowIcon(Sprite interactedIcon)
    {
        ActorIcon.sprite = interactedIcon;
    }

    public void SetCharacterStats(GameObject interactedObject)
    {
        // Go through the actor data of the object and copy it into the character window
    }
}
