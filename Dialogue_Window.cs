using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Window : MonoBehaviour
{
    public static Dialogue_Window instance;

    public Dialogue_Icon_Interacted interactedIcon;
    public TextMeshProUGUI interactedName;
    public TextMeshProUGUI interactedText;

    public Dialogue_Icon_Player playerIcon;
    public TextMeshProUGUI playerName;
    public Button playerDialogOptions;

    public bool isOpen = false;

    public void Start()
    {
        instance = this;

        transform.localScale = Vector3.zero;
    }

    public void OpenDialogueWindow(GameObject interactedObject)
    {
        isOpen = true;

        transform.localScale = Vector3.one;

        UpdateInteractedObject(interactedObject);
        UpdatePlayerObject();
        
    }

    public void UpdateInteractedObject(GameObject interactedObject)
    {
        UpdateInteractedObjectImage(interactedObject);
        UpdateInteractedObjectName(interactedObject);

        Dialogue_Text_Interacted interactedTextScript = interactedText.GetComponent<Dialogue_Text_Interacted>();
        interactedTextScript.UpdateDialogue();
    }

    public void UpdateInteractedObjectImage(GameObject interactedObject)
    {
        Image interactedImage = null;

        if (interactedIcon != null)
        {
            interactedImage.sprite = interactedObject.GetComponent<SpriteRenderer>().sprite;

            if (interactedImage != null)
            {
                interactedIcon.UpdateDialogueImage(interactedImage);
            }
        }
        else
        {
            Debug.Log("Interacted icon is null");
        }
    }

    public void UpdateInteractedObjectName(GameObject interactedObject)
    {
        if (interactedName != null)
        {
            interactedName.text = interactedObject.name;
        }
        else
        {
            Debug.Log("Interacted name is null");
        }
    }

    public void UpdatePlayerObject()
    {
        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            UpdatePlayerImage(player);
            UpdatePlayerName(player);
        }
        else
        {
            Debug.Log("Player not found");
        }
    }

    public void UpdatePlayerImage(Player player)
    {
        Image playerImage = null;

        if (playerIcon != null)
        {
            playerImage.sprite = player.GetComponent<SpriteRenderer>().sprite;

            if (playerImage != null)
            {
                playerIcon.UpdateDialogueImage(playerImage);
            }
        }
        else
        {
            Debug.Log("Player icon is null");
        }
    }

    public void UpdatePlayerName(Player player)
    {
        if (playerName != null)
        {
            playerName.text = player.name;
        }
        else
        {
            Debug.Log("Player name is null");
        }
    }
    
    public void CloseDialogueWindow()
    {
        isOpen = false;
        transform.localScale = Vector3.zero;
    }
}
