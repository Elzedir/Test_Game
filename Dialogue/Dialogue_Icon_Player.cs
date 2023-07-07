using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Icon_Player : MonoBehaviour
{
    public Image playerIcon;

    public void UpdateDialogueImage(Image image)
    {
        playerIcon = GetComponent<Image>();

        if (playerIcon != null)
        {
            playerIcon = image;
        }
        else
        {
            Debug.Log("Dialog player icon image is null");
        }
    }
}
