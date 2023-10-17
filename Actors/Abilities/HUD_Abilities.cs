using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Abilities : MonoBehaviour
{
    public static HUD_Abilities Instance;

    private Player _player;

    public const int AbilityCount = 10;

    [SerializeField] private Transform[] _abilityIcons = new Transform[AbilityCount];
    [SerializeField] private TextMeshProUGUI[] _abilityCooldowns = new TextMeshProUGUI[AbilityCount];

    private Dictionary<List_Ability, Coroutine> activeCooldowns = new Dictionary<List_Ability, Coroutine>();

    public void Awake()
    {
        Instance = this;
    }

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        _player = GameManager.Instance.Player;

        InitialiseComponents();
    }

    public void InitialiseComponents()
    {
        Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < AbilityCount; i++)
        {
            string abilityName = "AbilityIcon" + i;
            _abilityIcons[i] = allChildren.FirstOrDefault(t => t.name == abilityName);
            _abilityCooldowns[i] = _abilityIcons[i]?.GetComponentInChildren<TextMeshProUGUI>();
            _abilityCooldowns[i].text = "";

            Transform abilityOnCooldown = GameManager.Instance.FindTransformRecursively(_abilityIcons[i], "AbilityOnCooldown");
            abilityOnCooldown.gameObject.SetActive(false);
        }

        foreach (KeyValuePair <List_Ability, float> ability in _player.PlayerActor.ActorData.ActorAbilities.AbilityCooldowns)
        {
            OnAbilityUse(ability.Key); 
        }
    }

    public void Update()
    {
        if (_player == null || _player.gameObject != GameManager.Instance.Player.gameObject)
        {
            _player = GameManager.Instance.Player;
        }
    }

    public void OnAbilityUse(List_Ability ability)
    {
        float remainingCooldown = Manager_Abilities.GetRemainingCooldownTime(ability, _player.PlayerActor);

        if (remainingCooldown > 0)
        {
            int abilityIndex = _player.PlayerActor.ActorData.ActorAbilities.AbilityList.IndexOf(ability.AbilityData.AbilityStats.AbilityName);

            if (abilityIndex != -1)
            {
                if (activeCooldowns.ContainsKey(ability))
                {
                    StopCoroutine(activeCooldowns[ability]);
                }

                activeCooldowns[ability] = StartCoroutine(CooldownTimer(abilityIndex, remainingCooldown));
            }
        }
    }

    IEnumerator CooldownTimer(int abilityIndex, float cooldown)
    {
        UpdateAbilityIcon(abilityIndex, true);

        float remainingCooldown = cooldown;

        while (remainingCooldown > 0)
        {
            _abilityCooldowns[abilityIndex].text = remainingCooldown.ToString("F1");
            remainingCooldown -= Time.deltaTime;
            yield return null;
        }

        _abilityCooldowns[abilityIndex].text = "";

        UpdateAbilityIcon(abilityIndex, false);
    }
    public void UpdateAbilityIcon(int abilityIndex, bool isOnCooldown)
    {
        List_Ability ability = List_Ability.GetAbility(_player.PlayerActor.ActorData.ActorAbilities.AbilityList[abilityIndex]);
        Image abilityIcon = _abilityIcons[abilityIndex].GetComponent<Image>();
        abilityIcon.sprite = ability.AbilityData.AbilityStats.AbilityIcon;

        Transform[] allChildren = _abilityIcons[abilityIndex].GetComponentsInChildren<Transform>(true);
        Transform abilityOnCooldown = allChildren.FirstOrDefault(t => t.name == "AbilityOnCooldown");

        if (abilityOnCooldown != null)
        {
            abilityOnCooldown.gameObject.SetActive(isOnCooldown);
        }
    }
}
