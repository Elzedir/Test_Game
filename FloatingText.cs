using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText
{
    public bool active;
    public GameObject go;
    public TextMeshProUGUI txt;
    public Vector3 motion;
    public float duration;
    public float lastShown;

    public void Show()
    {
        // The function to show the floating text.
        active = true;
        lastShown = Time.time;
        go.SetActive(active);
    }

    public void Hide()
    {
        // The function to hide the floating text.
        active = false;
        go.SetActive(active);
    }

    public void UpdateFloatingText()
    {
        // If the window is not active, return, since we don't need to do anything.
        if(!active)
            return;

        // If the time right now minus the time the floating text was first shown is greater than the
        // duration the floating text is supposed to show, then hide the floating text.
        // 10 - 7 > 2
        if (Time.time - lastShown >= duration) 
            Hide();

        // Moves the floating text slowly upwards while the floating text is still active.
        go.transform.position += motion * Time.deltaTime;
    }
}
