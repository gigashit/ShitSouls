using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAnswer", menuName = "Scriptable Objects/DialogueAnswer")]
public class DialogueAnswer : ScriptableObject
{
    public string answerText;
    public Dialogue nextDialogue;
}


