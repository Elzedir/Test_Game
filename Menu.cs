using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;

public class Menu : MonoBehaviour
{
    // Text fields
    public TextMeshProUGUI levelText, hitpointText, goldText, upgradeCostText, xpText;

    // Logic
    private int currentCharacterSelection = 0;
    public Image characterSelectionSprite;
    public Image weaponSprite;
    public RectTransform xpBar;

    // Character Selection
    public void OnArrowClick(bool right)
    {
        // If click is right, then bool is true, if left, then bool is false.
        // If we click the right button.
        if (right)
        {
            // Then we increase the current character selection by one, moving to the next character.
            currentCharacterSelection ++;

            // If we reach the current total count of available player sprites,
            if (currentCharacterSelection == GameManager.instance.playerSprites.Count)
                // Set the character selection back to 0.
                currentCharacterSelection = 0;

            // If we are not at the total count, then change the character selection.
            OnSelectionChanged();
        }
        // If false (we click the left arrow), then
        else
        {
            // Then we decrease the current character selection by one, moving to the previous character.
            currentCharacterSelection--;

            // If we go beneath the minimum count of available player sprites,
            if (currentCharacterSelection < 0)
                // Set the character selection back to 1 below the max count.
                currentCharacterSelection = GameManager.instance.playerSprites.Count - 1;

            // If we are not at the total count, then change the character selection.
            OnSelectionChanged();
        }
    }
    private void OnSelectionChanged()
    {
        // The action of OnSelectionChanged takes the current selected sprite and changes the player
        // sprite according to the rules set in OnArrowClick.
        characterSelectionSprite.sprite = GameManager.instance.playerSprites[currentCharacterSelection];
        // The Game Manager will then tell the player to exectute command SwapSprite
        GameManager.instance.player.SwapSprite(currentCharacterSelection);
    }

    // Weapon upgrade
    // not implemented

    // Update the character information
    public void UpdateMenu()
    {
        // UI Bars
        hitpointText.text = GameManager.instance.player.hitpoint.ToString();
        goldText.text = GameManager.instance.gold.ToString();
        levelText.text = GameManager.instance.playerLevel().ToString();

        // XP Bar
        int currentLevel = GameManager.instance.playerLevel();

        // If the current level is equal to the total number of levels in the xpTable.
        if(currentLevel == GameManager.instance.xpTable.Count)
        {
            // then make the xp Text state how much experience you have accumulated in total
            xpText.text = GameManager.instance.totalExperience.ToString() + " total experience points";
            // And scale the xp bar to the scale of 1, which will fill up the whole bar.
            xpBar.localScale = Vector3.one;
        }
        // If we are not max level
        else
        {
            // Take how much total xp was required to level up last time
            int previousLevelXp = GameManager.instance.xpReqToLevelUp(currentLevel - 1);
            // And take how much total xp will be required to level up this time
            int nextLevelXp = GameManager.instance.xpReqToLevelUp(currentLevel);

            // Find the difference between then to give us the current xp required to level up.
            int levelUpXp = nextLevelXp - previousLevelXp;
            // Calculate how far we are into the level by subtracting the total amount of experience
            // required for the previous level up and subtract it from our current total experience.
            int currentXp = GameManager.instance.totalExperience - previousLevelXp;

            float xpRatio = (float)currentXp / (float)levelUpXp;
            xpBar.localScale = new Vector3(xpRatio, 1, 1);
            xpText.text = currentXp.ToString() + " / " + levelUpXp;
        }
    }
}
