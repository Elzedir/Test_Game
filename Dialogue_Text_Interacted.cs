using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue_Text_Interacted : MonoBehaviour
{
    public TextMeshProUGUI interactedTextBox;
    public float delayBetweenCharacters = 0.1f;
    private bool finishedTyping = false;

    public IEnumerator UpdateDialogue(string text)
    {
        interactedTextBox.text = "";
        yield return StartCoroutine(WriteTextCoroutine(text));
    }

    private IEnumerator WriteTextCoroutine(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            interactedTextBox.text += text[i];

            if (Input.GetKey(KeyCode.Return))
            {
                if (i < text.Length - 1)
                {
                    interactedTextBox.text = text;
                    i = text.Length;
                }
            }

            float delay = Input.GetKey(KeyCode.Space) ? delayBetweenCharacters / 10 : delayBetweenCharacters;
            yield return new WaitForSeconds(delay);
        }
        finishedTyping = true;
    }

    public bool IsFinishedTyping()
    {
        return finishedTyping;
    }

    public void ResetFinishedTyping()
    {
        finishedTyping = false;
    }
}
