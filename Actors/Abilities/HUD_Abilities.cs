using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Abilities : MonoBehaviour
{
    public static HUD_Abilities Instance;
    private const int AbilityCount = 10;

    [SerializeField] private Transform[] _abilityIcons = new Transform[AbilityCount];
    [SerializeField] private TextMeshProUGUI[] _abilityCooldowns = new TextMeshProUGUI[AbilityCount];

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
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
        }
    }

    public void UpdateAbilityIcons()
    {
        List<Ability> activePlayerAbilities = GameManager.Instance.Player.PlayerActor.ActorData.ActorAbilities.AbilityList;

        for (int i = 0; i < activePlayerAbilities.Count; i++)
        {
            if (activePlayerAbilities[i] != Ability.None)
            {
                List_Ability ability = List_Ability.GetAbility(activePlayerAbilities[i]);
                Image abilityIcon = _abilityIcons[i].GetComponent<Image>();
                abilityIcon.sprite = ability.AbilityIcon;
            }
        }
    }

    public void OnAbilityUsed(int abilityIndex, float cooldown)
    {
        if (abilityIndex >= 0 && abilityIndex < AbilityCount)
        {
            CooldownTimer(abilityIndex, cooldown);
        }
    }

    IEnumerator CooldownTimer(int abilityIndex, float cooldown)
    {
        float remainingCooldown = cooldown;

        while (remainingCooldown > 0)
        {
            _abilityCooldowns[abilityIndex].text = remainingCooldown.ToString("F1");
            remainingCooldown -= Time.deltaTime;
            yield return null;
        }

        _abilityCooldowns[abilityIndex].text = "";
    }
}
