using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> upgradePrices;
    public List<int> xpTable;

    public Player player;

    public FloatingTextManager floatingTextManager;

    public int gold;
    public int totalExperience;


    public GameObject hud;
    public GameObject menu;
    public GameObject eventSystemContainer;
    public GameObject factionManager;
    public GameObject weaponManager;
    private static bool exists = false;

    public RectTransform healthBar;
    public RectTransform manaBar;
    public RectTransform staminaBar;

    public Animator deathMenuAnimator;


    // Don't destroy on load
    private void Awake()
    {
        if (!exists)
        {
            exists = true;
            instance = this;
            SceneManager.sceneLoaded += LoadState;
            SceneManager.sceneLoaded += OnSceneLoad;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(player);
            DontDestroyOnLoad(floatingTextManager);
            DontDestroyOnLoad(hud);
            DontDestroyOnLoad(menu);
            DontDestroyOnLoad(eventSystemContainer);
            DontDestroyOnLoad(factionManager);
            DontDestroyOnLoad(weaponManager);
        }
        
        else
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud.gameObject);
            Destroy(menu.gameObject);
            Destroy(eventSystemContainer.gameObject);
            Destroy(factionManager.gameObject);
            Destroy(weaponManager.gameObject);
            return;
        }   
    }

    protected virtual void Start()
    {
        SetFactions();
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
        float healthRatio = (float)player.hitpoint / (float)player.maxHitpoint;
        healthBar.localScale = new Vector3(1, healthRatio, 1);
        // float manaRatio = (float)player.mana / (float)player.maxMana
        // manahBar.localScale = new Vector3(1, manaRatio, 1)
        // float staminaRatio = (float)player.stamina / (float)player.maxStamina
        // staminaBar.localScale = new Vector3(1, staminaRatio, 1)
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
    public void SaveState()
    {
        
        string s = " ";

        s += "0" + "|";
        s += gold.ToString() + "|";
        s += totalExperience.ToString() + "|";

        PlayerPrefs.SetString("SaveState", s);
    }
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;
        
        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        gold = int.Parse(data[1]);

        totalExperience = int.Parse(data[2]);

        if(playerLevel() !=1)
        player.SetLevel(playerLevel());
    }

    // Death and respawn
    public void Respawn()
    {
        deathMenuAnimator.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        player.Respawn();
    }

    // Factions
    public void SetFactions()
    {
        LayerMask[] attackableFactions = FactionManager.instance.AttackableFactions();
        {
            LayerMask passiveCanAttack = attackableFactions[0];
            LayerMask playerCanAttack = attackableFactions[1];
            LayerMask humanCanAttack = attackableFactions[2];
            LayerMask enemyHumanCanAttack = attackableFactions[3];
            LayerMask enemyMonsterCanAttack = attackableFactions[4];
            LayerMask destructable = attackableFactions[5];
        }
    }
}
