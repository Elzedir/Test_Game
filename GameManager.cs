using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    Cinematic,
    Loading,
    PlayerDead,
    InCombat
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action PlayerChange;

    public GameState CurrentState { get; private set; }

    public bool PlayerDead = false;

    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> upgradePrices;

    public Player Player;

    public FloatingTextManager floatingTextManager;

    public int gold;

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
        CurrentState = GameState.Playing;
        SceneManager.sceneLoaded += OnSceneLoad;

        Player = FindFirstObjectByType<Player>();

        InitializeGameData();
    }

    protected virtual void Start()
    {
        
    }

    private void Update()
    {
        if (Player == null)
        {
            Player = FindFirstObjectByType<Player>();

            if (Player != null)
            {
                return;
            }

            Player_Backup PlayerBackup = FindFirstObjectByType<Player_Backup>();
            PlayerBackup.AddComponent<Player>();
            Destroy(PlayerBackup.GetComponent<Player_Backup>());
            // Put in some way to find the player again. Maybe look in the last save for which character they were playing.
        }
    }

    void InitializeGameData()
    {
        LevelUpData.InitialiseLevels();
        List_Sprite.InitialiseSprites();
        List_Item_Weapon.InitializeWeaponData();
        List_Item_Armour.InitializeArmourData();
        List_Item_Consumable.InitializeConsumableData();
        List_Aspect.InitialiseSpecialisations();
        List_Ability.InitialiseAbilities();
        List_Faction.InitialiseFactions();
    }
    public void OnPlayerChange(Player player)
    {
        GameManager.Instance.Player = player;
        PlayerChange?.Invoke();
    }

    public void ChangeState(GameState newState)
    {
        switch (CurrentState)
        {
            case GameState.Playing:
                break;
            case GameState.Paused:
                break;
            case GameState.Loading:
                // hide the loading screen
                // switch to the next level
                break;
            case GameState.Cinematic:
                // Hide the cinematic
                // Resume player controls
                break;
            case GameState.PlayerDead:
                // Reset the player
                break;
        }

        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                // enbale player controls
                // resume normal sound and music
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                // disable player controls
                // play pause music and stop game sounds
                break;
            case GameState.Loading:
                // play a loading screen and slowly load the map in
                // begin loading the next level
                break;
            case GameState.Cinematic:
                // Play the cinematic
                // Stop player controls
                break;
            case GameState.PlayerDead:
                // Stop the player controls and enemies moving
                // Play game over sound effect
                break;
        }
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
        float healthRatio = Mathf.Clamp((float)Player.PlayerActor.CurrentCombatStats.MaxHealth / (float)Player.PlayerActor.CurrentCombatStats.MaxHealth, 0f, 1f);
        healthBar.localScale = new Vector3(healthRatio, 1, 1);
        float manaRatio = Mathf.Clamp((float)Player.PlayerActor.CurrentCombatStats.MaxMana / (float)Player.PlayerActor.CurrentCombatStats.MaxMana, 0f, 1f);
        manaBar.localScale = new Vector3(manaRatio, 1 , 1);
        float staminaRatio = Mathf.Clamp((float)Player.PlayerActor.CurrentCombatStats.MaxStamina / (float)Player.PlayerActor.CurrentCombatStats.MaxStamina, 0f, 1f);
        staminaBar.localScale = new Vector3(staminaRatio, 1, 1);
    }

    // Inventory

    // Experience
    public void GrantXp(int xp, Actor_Base actor)
    {
        actor.ActorData.ActorStats.ActorLevelData.TotalExperience += xp;
        LevelUpData.LevelUpCheck(actor);
    }

    // Save and Load
    public void OnSceneLoad(Scene s, LoadSceneMode mode)
    {
        Player = FindFirstObjectByType<Player>();
        Player.transform.position = GameObject.Find("spawn_player").transform.position;
    }
    public void SaveState(int saveSlot)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Save_Data saveData = new Save_Data(gold, currentSceneName);
        Save_Manager.SaveGame(saveData, saveSlot);
    }
    public void LoadState(int saveSlot)
    {
        Save_Data loadedData = Save_Manager.LoadGame(saveSlot);

        if (loadedData != null)
        {
            gold = loadedData.score;
            SceneManager.LoadScene(loadedData.levelName);
            // Actors set level?
        }
    }

    // Death and respawn
    public void Respawn()
    {
        deathMenuAnimator.SetTrigger("Hide");
        SceneManager.LoadScene("Main");
        Player.PlayerRespawn();
    }

    // Utility
    public static float Randomise(double minValue, double maxValue)
    {
        System.Random random = new System.Random();
        float randomFloat = (float)(random.NextDouble() * (maxValue - minValue) + minValue);
        return (float)Math.Round(randomFloat, 2);
    }

    // Other

    public void CreateNewItem(int itemID, int stackSize)
    {
        GameObject droppedItem = Instantiate(itemPrefab, Player.transform.position, Quaternion.identity, itemsArea);

        if (TryGetComponent<SpriteRenderer> (out SpriteRenderer droppedItemSpriteRenderer))
        {
            droppedItemSpriteRenderer.sprite = List_Item.GetItemData(itemID).ItemStats.CommonStats.ItemIcon;
        }

        Interactable_Item droppedItemScript = droppedItem.GetComponent<Interactable_Item>();

        if (droppedItemScript != null)
        {
            droppedItemScript.ItemID = itemID;
            droppedItemScript.StackSize = stackSize;
            droppedItemScript.UpdateItem();
        }
    }

    public void CreateDeadBody(Actor_Base actor)
    {
        // Instantiate a new game object and give it a dead body script with a certain animation and sprite.
    }

    public Transform FindTransformRecursively(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform result = FindTransformRecursively(child, name);

            if (result != null) return result;
        }
        return null;
    }

    [ContextMenu("Reset Player Levels")]
    private void ResetPlayerLevels()
    {
        Player = FindFirstObjectByType<Player>();

        Player.PlayerActor.ActorData.ActorStats.ActorLevelData.Level = 0;
        Player.PlayerActor.ActorData.ActorStats.ActorLevelData.TotalExperience = 0;
    }

    [ContextMenu("Reset Player Equipment")]
    private void ResetPlayerEquipment()
    {
        Player = FindFirstObjectByType<Player>();

        for (int i = 0; i < Player.PlayerActor.ActorData.ActorEquipment.EquipmentItems.Count; i++)
        {
            EquipmentItem.None(Player.PlayerActor.ActorData.ActorEquipment.EquipmentItems[i]);
        }
    }

    public void RunCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
