using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusEffectBarUI : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI label;

    public Color passiveColor;
    public Color inflictedColor;

    private StatusEffect effect;
    private Coroutine textFadeCoroutine;

    public void Initialize(StatusEffect effect)
    {
        this.effect = effect;
        label.text = "";
        gameObject.SetActive(false);

        effect.OnEffectTriggered += () =>
        {
            label.text = effect.name;
            Debug.Log($"{effect.name} inflicted!");
            
            if (textFadeCoroutine != null) { StopCoroutine(textFadeCoroutine); }

            textFadeCoroutine = StartCoroutine(FadeOutText());
        };

        effect.OnEffectEnded += () =>
        {
            label.text = "";
        };
    }

    private IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(2f);

        DOTween.Kill(label);
        label.DOFade(0f, 2f);

        yield return new WaitForSeconds(2f);

        textFadeCoroutine = null;
    }

    public void TriggerStatusEffectBarUI(bool enable)
    {
        gameObject.SetActive(enable);
    }

    private void Update()
    {
        if (effect.value > 0)
        {
            fillImage.fillAmount = effect.NormalizedValue;
            fillImage.color = effect.isInflicted ? inflictedColor : passiveColor;
        }
    }
}