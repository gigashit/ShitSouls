using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image dialogueBG;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image answersBG;
    [SerializeField] private GameObject answerButton;
    [SerializeField] private RectTransform answerButtonsContainer;

    private InteractableNPC currentNPC;

    private void Start()
    {
        dialogueBG.gameObject.SetActive(false);
        // answersBG.gameObject.SetActive(false);
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

        ShowDialogue(dialogue.lines[0]);

        npc.hasTalkedTo = true;
    }

    private void ShowDialogue(DialogueLine line)
    {
        dialogueText.text = line.lineText;
        dialogueBG.gameObject.SetActive(true);
    }
}
