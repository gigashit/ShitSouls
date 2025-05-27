using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform playerCurrentLocation;

    [Header("Ending Sequence Elements")]
    [SerializeField] private Image deathScreenBG;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private Image blackScreen;
    [SerializeField] private Color zeroColor;
    [SerializeField] private Color textOriginalColor;
    [SerializeField] private Color bgOriginalColor;

    [Header("Script References")]
    [SerializeField] private HealthManager healthManager;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        deathScreenBG.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);
    }

    public void InitiateDeathSequence()
    {
        StartCoroutine(DeathSequence());
    }

    public IEnumerator DeathSequence()
    {
        float roll = Random.Range(2f, 5f);

        yield return new WaitForSeconds(roll);

        float textShowDuration = 4f;

        deathText.transform.localScale = Vector3.one;
        deathText.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), textShowDuration).SetEase(Ease.OutCirc);
        deathText.color = zeroColor;
        deathText.DOColor(textOriginalColor, textShowDuration).SetEase(Ease.OutCirc);
        deathScreenBG.color = zeroColor;
        deathScreenBG.DOColor(bgOriginalColor, textShowDuration);
        deathScreenBG.gameObject.SetActive(true);

        yield return new WaitForSeconds(textShowDuration);

        blackScreen.color = zeroColor;
        blackScreen.DOColor(Color.black, 2f);
        blackScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        ResetGame();
        deathScreenBG.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        healthManager.RespawnPlayer();
        blackScreen.DOColor(zeroColor, 1f);

        yield return new WaitForSeconds(1f);

        blackScreen.gameObject.SetActive(false);
    }

    private void ResetGame()
    {
        playerCurrentLocation.DOMove(playerSpawnPoint.position, 0.1f);
    }
}
