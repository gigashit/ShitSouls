using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class PlayerInteractionHandler : MonoBehaviour
{
    public InteractionType currentInteractionType;

    [Header("UI Elements")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private Image interactButtonImage;

    [Header("Script References")]
    [SerializeField] private InputKeyIconRandomizer iconRandomizer;

    public bool canInteract = false;
    public bool isInteracting = false;

    private void Awake()
    {
        InitializeElements();
    }

    private void OnEnable()
    {
        InputManager.Instance.inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        InputManager.Instance.inputActions.Player.Interact.performed -= OnInteract;
    }

    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Interact();
    }

    private void InitializeElements()
    {
        interactionPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            InteractableEntity interactableEntity = other.GetComponent<InteractableEntity>();

            ShowInteractionPrompt(interactableEntity.promptText);
            currentInteractionType = interactableEntity.interactionType;

            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            interactionPrompt.SetActive(false);
            currentInteractionType = InteractionType.None;
            canInteract = false;
        }
    }

    private void ShowInteractionPrompt(string text)
    {
        interactionText.text = text;
        interactButtonImage.sprite = iconRandomizer.GetRandomIcon(iconRandomizer.interactIcons);
        interactionPrompt.SetActive(true);
    }

    private void Interact()
    {
        if (canInteract)
        {
            Debug.Log("Interacted with " + currentInteractionType + "!");
            isInteracting = true;
        }
    }
}

public enum InteractionType
{
    None,
    Item,
    Door,
    Ladder,
    Chest,
    MimicChest,
    NPCMerchant,
    NPCBlacksmith,
    NPCQuestgiver,
    NPCMaiden,
}
