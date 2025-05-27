using System;
using System.Collections;
using UnityEngine;

public class StatusEffect
{
    public string name;
    public float value;          // Current buildup value (0–max)
    public float maxValue;
    public float decayRate;      // Rate to decay when not exposed
    public bool isBuilding;
    public bool isInflicted;   // Prevent buildup while active        
    public bool isActive;

    private float lastBuildupTime = -999f;
    private const float buildupTimeout = 0.5f;

    public event Action OnEffectTriggered;
    public event Action OnEffectEnded;

    public StatusEffect(string name, float maxValue, float decayRate)
    {
        this.name = name;
        this.maxValue = maxValue;
        this.decayRate = decayRate;
        value = 0f;
        isBuilding = false;
        isInflicted = false;
        isActive = false;
    }

    public void AddBuildup(float amount)
    {
        if (isInflicted) return;

        value += amount;
        value = Mathf.Min(value, maxValue);

        if (value >= maxValue)
        {
            TriggerEffect();
        }

        lastBuildupTime = Time.time;
    }

    public void UpdateEffect(float deltaTime)
    {
        isActive = value > 0f;

        if (!isActive && isInflicted)
        {
            EndEffect();
        }

        if (!isActive) return;

        isBuilding = (Time.time - lastBuildupTime) < buildupTimeout;

        if (!isBuilding || isInflicted)
        {
            float decay = decayRate * deltaTime;
            value = Mathf.Max(value - decay, 0f);
        }

        Debug.Log("value: " + value + "/" + maxValue);
    }

    private void TriggerEffect()
    {
        Debug.LogWarning("Triggering effect!");
        isInflicted = true;
        OnEffectTriggered?.Invoke();
    }

    private void EndEffect()
    {
        Debug.LogWarning("Ending effect!");
        isInflicted = false;
        OnEffectEnded?.Invoke();
    }

    public float NormalizedValue => value / maxValue;
}
