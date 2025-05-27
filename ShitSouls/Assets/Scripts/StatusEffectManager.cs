using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class StatusEffectManager : MonoBehaviour
{
    private Dictionary<string, StatusEffect> effects = new();

    // For now, we'll expose poison publicly to test easily
    public StatusEffect Poison => effects["Poison"];
    public TMP_Text isActiveText;
    public TMP_Text isBuildingText;
    public TMP_Text isInflictedText;

    private void Start()
    {
        // Initialize all status effects the player can have
        var poison = new StatusEffect("Poison", 100f, 10f);
        poison.OnEffectTriggered += () => Debug.LogWarning("Player is poisoned!");
        poison.OnEffectEnded += () => Debug.LogWarning("Poison ended!");

        effects.Add("Poison", poison);

        // TODO: add more status effects (bleed, frostbite, etc.)
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var effect in effects.Values)
        {
            effect.UpdateEffect(dt);

            if (effect.isActive) { isActiveText.color = Color.green; } else { isActiveText.color = Color.red; }
            if (effect.isBuilding) { isBuildingText.color = Color.green; } else { isBuildingText.color = Color.red; }
            if (effect.isInflicted) { isInflictedText.color = Color.green; } else { isInflictedText.color = Color.red; }
        }
    }

    /// <summary>
    /// Adds buildup to a named status effect (e.g., from poison swamp)
    /// </summary>
    public void AddBuildup(string effectName, float amount)
    {
        if (effects.TryGetValue(effectName, out var effect))
        {
            Debug.Log("Increasing poison value");
            effect.AddBuildup(amount);
        }
        else
        {
            Debug.LogWarning($"Tried to add buildup to unknown effect: {effectName}");
        }
    }

    /// <summary>
    /// For future use — query active effects
    /// </summary>
    public bool IsEffectActive(string effectName)
    {
        return effects.ContainsKey(effectName) && effects[effectName].isActive;
    }

    public float GetEffectNormalizedValue(string effectName)
    {
        return effects.ContainsKey(effectName) ? effects[effectName].NormalizedValue : 0f;
    }
}
