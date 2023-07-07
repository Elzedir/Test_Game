using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Icon_Interacted : MonoBehaviour
{
    public Image interactedIcon;

    public void UpdateDialogueImage(Image image)
    {
        interactedIcon = GetComponent<Image>();

        if (interactedIcon != null)
        {
            interactedIcon = image;
        }
        else
        {
            Debug.Log("Dialog interacted icon image is null");
        }
    }
}
