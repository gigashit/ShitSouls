using Cysharp.Threading.Tasks;
using System.Collections;
using System.IO.Pipes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image dialogueBG;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image answersBG;
    [SerializeField] private GameObject answerButton;
    [SerializeField] private RectTransform answerButtonsContainer;

    [Header("Script References")]
    [SerializeField] private PlayerInteractionHandler playerInteractionHandler;
    [SerializeField] private ThirdPersonCameraController thirdPersonCameraController;

    private InteractableNPC currentNPC;
    private Dialogue currentDialogue;
    private Coroutine lineCoroutine;
    private int lineIndex;
    private bool isSpeaking;

    private void Start()
    {

        dialogueBG.gameObject.SetActive(false);
        answersBG.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetupInputEvents();
    }

    private void OnDisable()
    {
        InputManager.Instance.inputActions.Player.Interact.performed -= OnInteract;
    }

    private async UniTaskVoid SetupInputEvents()
    {
        await UniTask.Delay(50);
        InputManager.Instance.inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {

    }

    public void InitiateDialogue(InteractableNPC npc)
    {
        currentNPC = npc;
        Dialogue dialogue;

        if (!npc.hasTalkedTo)
        {
            dialogue = npc.dialogues.Find(d => d.isFirst);
        }
        else
        {
            dialogue = npc.dialogues.Find(d => d.isDefault);
        }

        currentDialogue = dialogue;
        lineCoroutine = StartCoroutine(ShowDialogue(dialogue.lines[0]));
        lineIndex = 0;

        npc.hasTalkedTo = true;
    }

    private IEnumerator ShowDialogue(DialogueLine line)
    {
        isSpeaking = true;
        dialogueText.text = line.lineText;
        dialogueBG.gameObject.SetActive(true);
        currentNPC.audioSource.clip = line.lineAudio;
        currentNPC.audioSource.Play();

        yield return new WaitForSeconds(line.lineAudio.length + 1f);

        if (line.dialogueAnswers.Count == 0)
        {
            switch (line.dialogueAction)
            {
                case DialogueAction.None:
                    lineIndex++;
                    lineCoroutine = StartCoroutine(ShowDialogue(currentDialogue.lines[lineIndex]));
                    break;
                case DialogueAction.Leave:
                    LeaveDialogue();
                    break;
                default:
                    Debug.LogError("DialogueAction not found");
                    break;

            }
        }
        else
        {
            isSpeaking = false;
            dialogueBG.gameObject.SetActive(false);
            PopulateAnswerButtons(line);
        }
    }

    private void PopulateAnswerButtons(DialogueLine line)
    {
        thirdPersonCameraController.cameraInputEnabled = false;
        bool isFirst = true;

        foreach (Transform child in answersBG.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueAnswer answer in line.dialogueAnswers)
        {
            GameObject buttonObj = Instantiate(answerButton, answersBG.transform);
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = answer.answerText;

            DialogueAnswer capturedAnswer = answer;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                HandleAnswerSelected(capturedAnswer.nextDialogue);
            });

            if (isFirst)
            {
                EventSystem.current.SetSelectedGameObject(buttonObj);
                isFirst = false;
            }

        }

        answersBG.gameObject.SetActive(true);
    }

    private void HandleAnswerSelected(Dialogue nextDialogue)
    {
        thirdPersonCameraController.cameraInputEnabled = true;
        currentDialogue = nextDialogue;
        lineCoroutine = StartCoroutine(ShowDialogue(nextDialogue.lines[0]));
        lineIndex = 0;

        answersBG.gameObject.SetActive(false);
    }

    private void LeaveDialogue()
    {
        playerInteractionHandler.ExitInteraction();
        isSpeaking = false;
        dialogueBG.gameObject.SetActive(false);
    }
}
