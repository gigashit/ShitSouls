using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : MonoBehaviour
{
    public NPCType npcType;
    public List<Dialogue> dialogues;
    public AudioSource audioSource;
    public bool hasTalkedTo;
}
public enum NPCType
{
    MerchantNPC,
    BlacksmithNPC,
    SludgeHoeNPC,
    GuestgiverNPC,
}
