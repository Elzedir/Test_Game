using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Window : MonoBehaviour
{
    public static Dialogue_Window instance;

    public GameObject interactedCharacter;
    public Dialogue_Icon_Interacted interactedIcon;
    public TextMeshProUGUI interactedName;
    public TextMeshProUGUI interactedText;

    public Dialogue_Icon_Player playerIcon;
    public TextMeshProUGUI playerName;
    public GameObject playerDialogueOptionPrefab;
    public Transform choiceArea;

    public bool isOpen = false;

    public void Start()
    {
        instance = this;

        gameObject.SetActive(false);
    }

    public IEnumerator OpenDialogueWindow(GameObject interactedObject, string text)
    {
        isOpen = true;
        interactedCharacter = interactedObject;
        gameObject.SetActive(true);

        UpdatePlayerObject();
        yield return StartCoroutine(UpdateInteractedObject(text));
    }

    public IEnumerator UpdateInteractedObject(string text)
    {
        UpdateInteractedObjectImage();
        UpdateInteractedObjectName();

        Dialogue_Text_Interacted interactedTextScript = interactedText.GetComponent<Dialogue_Text_Interacted>();
        yield return StartCoroutine(interactedTextScript.UpdateDialogue(text));
    }

    public void UpdateInteractedObjectImage()
    {
        if (interactedIcon != null)
        {
            Sprite sprite = interactedCharacter.GetComponent<SpriteRenderer>().sprite;
            Image interactedImage = interactedIcon.gameObject.GetComponent<Image>();

            if (sprite != null)
            {
                interactedImage.sprite = sprite;
                interactedIcon.UpdateDialogueImage(interactedImage);
            }
        }

        else
        {
            Debug.Log("Interacted icon is null");
        }
    }

    public void UpdateInteractedObjectName()
    {
        if (interactedName != null)
        {
            interactedName.text = interactedCharacter.name;
        }
        else
        {
            Debug.Log("Interacted name is null");
        }
    }

    public void UpdatePlayerObject()
    {
        Player player = FindFirstObjectByType<Player>();

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
        if (playerIcon != null)
        {
            Sprite playerSprite = player.GetComponent<SpriteRenderer>().sprite;
            Image playerImage = playerIcon.gameObject.GetComponent<Image>();

            if (playerImage != null)
            {
                playerImage.sprite = playerSprite;
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

    public void CreateChoiceButtons(int numChoices)
    {
        for (int i = 0; i < numChoices; i++)
        {
            GameObject buttonObject = Instantiate(playerDialogueOptionPrefab, choiceArea);
            Button choiceButton = buttonObject.GetComponent<Button>();
        }
    }

    public void UpdateChoicesUI(DialogueLine currentLine)
    {
        foreach (Transform child in choiceArea)
        {
            Destroy(child.gameObject);
        }

        if (currentLine.Choices != null && currentLine.Choices.Length > 0)
        {
            CreateChoiceButtons(currentLine.Choices.Length);

            int i = 0;

            foreach (Transform child in choiceArea)
            {
                if (i < currentLine.Choices.Length)
                {
                    Button choiceButton = child.GetComponent<Button>();
                    TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();

                    if (choiceButton != null && buttonText != null)
                    {
                        DialogueChoice choice = currentLine.Choices[i];
                        buttonText.text = choice.Choice;
                        choiceButton.onClick.AddListener(() => Dialogue_Manager.instance.OptionSelected(choice, choiceArea));

                        i++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void CloseDialogueWindow()
    {
        isOpen = false;
        gameObject.SetActive(false);
        Dialogue_Manager.instance.StopDialogue();
    }
}
