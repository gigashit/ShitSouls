using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class StatusEffectManager : MonoBehaviour
{
    private Dictionary<string, StatusEffect> effects = new();
    private Dictionary<string, Coroutine> activeDamageCoroutines = new();
    
    private HealthManager healthManager;

    [Header("Status Effect Bars")]
    public StatusEffectBarUI poisonBar;

    private void Start()
    {
        InitializeScripts();

        // Initialize all status effects the player can have
        var poison = new StatusEffect("Poison", 100f, 5f, 3f);
        poison.OnEffectTriggered += () =>
        {
            Debug.LogWarning("Player is poisoned!");
            StartEffectDamageOverTime(poison.name, 5f, 0.5f); // 0.5 damage every 0.5s
        };

        poison.OnEffectEnded += () =>
        {
            Debug.LogWarning("Poison ended!");
            StopEffectDamageOverTime(poison.name);
        };

        effects.Add("Poison", poison);

        poisonBar.Initialize(poison);
        poison.OnActiveStateChanged += isNowActive =>
        {
            poisonBar.TriggerStatusEffectBarUI(isNowActive);
        };

        // TODO: add more status effects (bleed, frostbite, etc.)
    }

    public void EndEffectsOnDeath()
    {
        foreach (var effect in effects.Values)
        {
            effect.EndEffect();
            effect.value = 0f;
        }
    }

    private void InitializeScripts()
    {
        healthManager = GetComponent<HealthManager>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var effect in effects.Values)
        {
            effect.UpdateEffect(dt);
        }
    }

    /// <summary>
    /// Adds buildup to a named status effect (e.g., from poison swamp)
    /// </summary>
    public void AddBuildup(string effectName, float amount)
    {
        if (!healthManager.isDead)
        {
            if (effects.TryGetValue(effectName, out var effect))
            {
                effect.AddBuildup(amount);
            }
            else
            {
                Debug.LogWarning($"Tried to add buildup to unknown effect: {effectName}");
            }
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

    // === DAMAGE OVER TIME LOGIC ===

    private void StartEffectDamageOverTime(string effectName, float damagePerTick, float tickRate)
    {
        if (activeDamageCoroutines.ContainsKey(effectName)) return;

        Coroutine routine = StartCoroutine(DamageOverTime(damagePerTick, tickRate));
        activeDamageCoroutines[effectName] = routine;
    }

    private void StopEffectDamageOverTime(string effectName)
    {
        if (activeDamageCoroutines.TryGetValue(effectName, out var routine))
        {
            StopCoroutine(routine);
            activeDamageCoroutines.Remove(effectName);
        }
    }

    private IEnumerator DamageOverTime(float damagePerTick, float tickRate)
    {
        while (true)
        {
            healthManager.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickRate);
        }
    }
}
