using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class ItemAddPopUp : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text itemNameText;

    public void SetUp(Sprite icon, int amount, string text)
    {
        itemIcon.sprite = icon;
        amountText.text = amount.ToString();
        itemNameText.text = text;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(3f);

        itemIcon.DOFade(0f, 1f);
        amountText.DOFade(0f, 1f);
        itemNameText.DOFade(0f, 1f);

        yield return new WaitForSeconds(1.1f);

        Destroy(gameObject);
    }
}
