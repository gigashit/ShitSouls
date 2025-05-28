using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class HealthManager : MonoBehaviour
{
    public Image healthBarFill;
    public Image healthBarFlashFill;
    public Image maxHPIndicator;

    public float playerCurrentHealth;
    public float playerMaxHealth;

    private PlayerMovementController movementController;
    private GameManager gameManager;
    private StatusEffectManager statusEffectManager;

    public bool isDead;

    private void Start()
    {
        InitializeScripts();

        isDead = false;
    }

    private void InitializeScripts()
    {
        movementController = GetComponent<PlayerMovementController>();
        gameManager = FindFirstObjectByType<GameManager>();
        statusEffectManager = GetComponent<StatusEffectManager>();
    }

    public void ResetHP()
    {
        if (playerMaxHealth > 3)
        {
            playerMaxHealth -= 3;
            if (playerMaxHealth <= 0) { playerMaxHealth = 3; }
        }

        playerCurrentHealth = playerMaxHealth;

        healthBarFill.transform.localScale = new Vector3(playerMaxHealth * 0.01f, healthBarFill.transform.localScale.y, healthBarFill.transform.localScale.z);
        healthBarFlashFill.transform.localScale = new Vector3(playerMaxHealth * 0.01f, healthBarFlashFill.transform.localScale.y, healthBarFlashFill.transform.localScale.z);
        healthBarFill.fillAmount = 1f;
        healthBarFlashFill.fillAmount = 1f;

        MoveMaxHPIndicator(playerMaxHealth * 0.01f);
    }

    private void MoveMaxHPIndicator(float value)
    {
        float minX = -340f;
        float maxX = 360f;

        RectTransform rt = maxHPIndicator.gameObject.GetComponent<RectTransform>();
        Vector2 anchoredPos = rt.anchoredPosition;
        anchoredPos.x = Mathf.Lerp(minX, maxX, value);
        rt.anchoredPosition = anchoredPos;
    }

    public void TakeDamage(float damageAmount)
    {
        playerCurrentHealth -= damageAmount;
        UpdateHPBar(false);

        if (playerCurrentHealth <= 0)
        {
            KillPlayer();
        }
    }

    private void UpdateHPBar(bool isHeal)
    {
        if (!isHeal)
        {
            DOTween.Kill(healthBarFill);
            DOTween.Kill(healthBarFlashFill);

            healthBarFill.fillAmount = playerCurrentHealth / playerMaxHealth;
            healthBarFlashFill.DOFillAmount(playerCurrentHealth / playerMaxHealth, (playerMaxHealth - playerCurrentHealth) * 0.02f);
        }
        else
        {
            DOTween.Kill(healthBarFill);
            DOTween.Kill(healthBarFlashFill);

            healthBarFill.DOFillAmount(playerCurrentHealth / playerMaxHealth, (playerCurrentHealth - playerMaxHealth) * 0.02f);
            healthBarFlashFill.DOFillAmount(playerCurrentHealth / playerMaxHealth, (playerCurrentHealth - playerMaxHealth) * 0.02f);
        }
    }

    private void KillPlayer()
    {
        movementController.KillPlayer();
        gameManager.InitiateDeathSequence();
        statusEffectManager.EndEffectsOnDeath();

        isDead = true;
    }

    public void RespawnPlayer()
    {
        ResetHP();
        movementController.RespawnPlayer();
        isDead = false;
    }
}
