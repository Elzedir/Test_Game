using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Manager : MonoBehaviour
{
    public static Dialogue_Manager instance;
    public Dialogue_Data_SO currentDialogue;
    private int currentLineIndex = 0;
    public Text dialogueText;
    public Button[] choiceButtons;

    public void Start()
    {
        instance = this;
    }

    public void StartDialogue(GameObject interactedCharacter, Dialogue_Data_SO dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;

        Dialogue_Window dialogueWindow = FindObjectOfType<Dialogue_Window>();

        if (dialogueWindow != null)
        {
            dialogueWindow.OpenDialogueWindow(interactedCharacter);
        }

        UpdateDialogueUI(interactedCharacter);
    }

    public void UpdateDialogueUI(GameObject interactedCharacter)
    {
        var nextLine = AdvanceDialogue();

        if (nextLine != null)
        {
            dialogueText.text = nextLine.line;

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < nextLine.choices.Length)
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    choiceButtons[i].GetComponentInChildren<Text>().text = nextLine.choices[i].choiceText;

                    Dialogue_Data_SO nextDialogue = nextLine.choices[i].nextDialogue;
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => StartDialogue(interactedCharacter, nextDialogue));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            EndDialogue();
            dialogueText.text = "";

            foreach (var button in choiceButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    public DialogueLine AdvanceDialogue()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            return null;
        }

        return currentDialogue.dialogueLines[currentLineIndex++];
    }

    public void EndDialogue()
    {
        currentDialogue = null;
        currentLineIndex = 0;
    }
}