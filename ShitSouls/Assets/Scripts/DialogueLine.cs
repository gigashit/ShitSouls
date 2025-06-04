using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueLine", menuName = "Scriptable Objects/DialogueLine")]
public class DialogueLine : ScriptableObject
{
    [TextArea(20, 5)]
    public string lineText;
    public AudioClip lineAudio;
    public List<DialogueAnswer> dialogueAnswers;
    public DialogueAction dialogueAction;
}

public enum DialogueAction
{
    None,
    Buy,
    Leave,
    Give,
    Antagonize,
    LevelUp,
}
