using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.Services.CloudSave;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> upgradePrices;
    public List<int> xpTable;

    public Player player;

    public FloatingTextManager floatingTextManager;

    public int gold;
    public int totalExperience;

    public GameObject eventSystemContainer;
    public GameObject factionManager;
    public GameObject inventoryManager;
    public GameObject uiManager;

    public RectTransform healthBar;
    public RectTransform manaBar;
    public RectTransform staminaBar;

    public Animator deathMenuAnimator;

    public int mostRecentAutoSave = 0;
    public int leastRecentAutoSave = 4;

    public GameObject itemPrefab;
    public Transform itemsArea;

    private void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    protected virtual void Start()
    {
        InitializeGameData();
    }

    void InitializeGameData()
    {
        List_Weapon.InitializeWeaponData();
        List_Armour.InitializeArmourData();
    }

    // Floating Text
    public void ShowFloatingText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)

    {
        // Doing this so that we don't have access to the Floating Text Manager from everywhere, only here.
        // Show the following parameter in the FloatingTextManager
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);

    }

    // Upgrade Weapon
    // not implemented

    //HUD Bar
    public void HUDBarChange()
    {
        Manager_Stats playerStatManager = player.GetComponent<Manager_Stats>();

        float healthRatio = Mathf.Clamp((float)playerStatManager.currentHealth / (float)playerStatManager.maxHealth, 0f, 1f);
        healthBar.localScale = new Vector3(healthRatio, 1, 1);
        float manaRatio = Mathf.Clamp((float)playerStatManager.currentMana / (float)playerStatManager.maxMana, 0f, 1f);
        manaBar.localScale = new Vector3(manaRatio, 1 , 1);
        float staminaRatio = Mathf.Clamp((float)playerStatManager.currentStamina / (float)playerStatManager.maxStamina, 0f, 1f);
        staminaBar.localScale = new Vector3(staminaRatio, 1, 1);
    }

    // Inventory

    // Experience
    public int playerLevel()
    {
        int playerLevel = 0;
        int requiredXp = 0;

        while (totalExperience >= requiredXp)
        {
            requiredXp += xpTable[playerLevel];
            playerLevel++;

            if (playerLevel == xpTable.Count)
                return playerLevel;
        }

        return playerLevel;
    }
    public int xpReqToLevelUp(int nextlevel)
    {
        
        int playerLevel = 0;
        int xpGain = 0;

        while (playerLevel < nextlevel)
        {
            xpGain += xpTable[playerLevel];
            playerLevel++;
        }

        return xpGain;
    }
    public void GrantXp(int xp)
    {
        int CurrentLevel = playerLevel();
        totalExperience += xp;
        if (CurrentLevel < playerLevel())
            LevelUp();
    }
    public void LevelUp()
    {
        Debug.Log("Level up!");
        player.LevelUp();
        HUDBarChange();
    }

    // Save and Load
    public void OnSceneLoad(Scene s, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("spawn_player").transform.position;
    }
    public void SaveState(int saveSlot)
    {

        string currentSceneName = SceneManager.GetActiveScene().name;
        Save_Data saveData = new Save_Data(gold, totalExperience, currentSceneName);
        Save_Manager.SaveGame(saveData, saveSlot);
    }
    public void LoadState(int saveSlot)
    {
        Save_Data loadedData = Save_Manager.LoadGame(saveSlot);

        if (loadedData != null)
        {
            gold = loadedData.score;
            totalExperience = loadedData.level;
            SceneManager.LoadScene(loadedData.levelName);
            player.SetLevel(playerLevel());
        }
    }

    // Death and respawn
    public void Respawn()
    {
        deathMenuAnimator.SetTrigger("Hide");
        SceneManager.LoadScene("Main");
        player.Respawn();
    }

    // Utility
    public static float Randomise(double minValue, double maxValue)
    {
        System.Random random = new System.Random();
        float randomFloat = (float)(random.NextDouble() * (maxValue - minValue) + minValue);
        return (float)Math.Round(randomFloat, 2);
    }

    // OTher

    public void CreateNewItem(int itemID, int stackSize)
    {
        GameObject droppedItem = Instantiate(itemPrefab, player.transform.position, Quaternion.identity, itemsArea);

        Interactable_Item droppedItemScript = droppedItem.GetComponent<Interactable_Item>();

        if (droppedItemScript != null)
        {
            droppedItemScript.itemID = itemID;
            droppedItemScript.stackSize = stackSize;
            droppedItemScript.UpdateItem();
        }
    }
}
