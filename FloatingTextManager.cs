using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject textContainer;
    public GameObject textPrefab;

    // Create a list (array) with the name "floatingTexts" of all Floating Texts that could occur ingame.
    // And then create that list immediately (new List<FloatingText>();)
    private List<FloatingText> floatingTexts = new List<FloatingText>();

    private void Update()
        // Since we deleted the Update class in the FloatingText script, we are recreating it here.
    {
        // This will update every floating text in the array every frame.
        foreach (FloatingText txt in floatingTexts)
            txt.UpdateFloatingText();
    }

    public void Show(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
        // Creating a string of information that will show on the screen.
    {
        // Using the FloatingText script, get the text from the floatingText array.
        FloatingText floatingText = GetFloatingText();

        // Manually changing the contents of the text in the floating text that will show.
        // Txt is the component of the object FloatingText.
        floatingText.txt.text = msg;
        // Taking the fontSize specified in floatingText and adding it to show ingame.
        floatingText.txt.fontSize = fontSize;
        // Taking the color received from the FloatingText and adding it to show ingame.
        floatingText.txt.color = color;

        // Specifying the position of the floating text.
        // The text will not use the scene space, but the world space, and so we have to base the position
        // of the floating text off of the position of the main camera so that we are operating within
        // the same space here.
        // We are transferring the floating text world space to screen space so that we can use it in
        // UI which operates in screen space.
        floatingText.go.transform.position = Camera.main.WorldToScreenPoint(position);
        // Creating motion for the floating text that will show.
        floatingText.motion = motion;
        // Similarly, creating duration for the floating text. Info to be transferred to text object.
        floatingText.duration = duration;

        // And then run the command to show it.
        floatingText.Show();
    }

    private FloatingText GetFloatingText()
        // Create the action to get the text.
    {
        // Within the array "floatingTexts", find the non-active text (txt) for the event.
        FloatingText txt = floatingTexts.Find(t =>!t.active);

        // If a non-active txt is found for the event, then create one.
        if(txt == null)
        {
            // Creating a new Floating Text script.
            txt = new FloatingText();
            // Using the textPrefab, create a new GameObject for the floating text.
            txt.go = Instantiate(textPrefab);
            // Transforms the Text Container Game Object into the parent of the new floating text Game
            // Object.
            txt.go.transform.SetParent(textContainer.transform);
            // Gets the component "Text" (The actual text) from the Floating Text script of the game
            // object that was created.
            txt.txt = txt.go.GetComponent<TextMeshProUGUI>();
            
            // We then add the new Game Object (with script, text, and as a child of the Text Container)
            // to the array "floatingTexts".
            floatingTexts.Add(txt);
        }

        // If you find active text for the event, return.
        return txt;
    }
}
