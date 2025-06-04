using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class PlayerInteractionHandler : MonoBehaviour
{
    public InteractionType currentInteractionType;
    private GameObject currentInteractable;

    [Header("UI Elements")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private Image interactButtonImage;
    [SerializeField] private GameObject itemPickupPopup;
    [SerializeField] private RectTransform pickUpPopupContainer;

    [Header("Script References")]
    [SerializeField] private InputKeyIconRandomizer iconRandomizer;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private InventoryManager inventoryManager;

    public bool canInteract = false;
    public bool isInteracting = false;
    public bool isInInteractRange = false;

    private void Awake()
    {
        InitializeElements();
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
            currentInteractable = other.gameObject;

            ShowInteractionPrompt(interactableEntity.promptText);
            currentInteractionType = interactableEntity.interactionType;

            canInteract = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            isInInteractRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            interactionPrompt.SetActive(false);
            currentInteractionType = InteractionType.None;
            canInteract = false;
            isInInteractRange = false;
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
        if (canInteract && !movementController.isLocked)
        {
            InitiateCorrectInteraction();
            movementController.isLocked = true;
            Debug.Log("Interacted with " + currentInteractionType + "!");
            isInteracting = true;

            interactionPrompt.SetActive(false);
            canInteract = false;
        }
    }

    private void InitiateCorrectInteraction()
    {
        switch (currentInteractionType)
        {
            case InteractionType.NPC:
                InteractableNPC npc = currentInteractable.GetComponent<InteractableNPC>();
                dialogueManager.InitiateDialogue(npc);
                break;
            case InteractionType.Item:
                InteractableItem item = currentInteractable.GetComponent<InteractableItem>();
                inventoryManager.AddItem(item.itemInfo, item.amount);
                ShowAddedItemPopUp(item);
                break;
            default:
                Debug.LogError("Unknown interaction type");
                break;
        }
    }

    public void ExitInteraction()
    {
        movementController.isLocked = false;
        isInteracting = false;

        if (isInInteractRange)
        {
            interactionPrompt.SetActive(true);
            canInteract = true;
        }
    }

    private void ShowAddedItemPopUp(InteractableItem item)
    {
        GameObject obj = Instantiate(itemPickupPopup, pickUpPopupContainer);
        ItemAddPopUp script = obj.GetComponent<ItemAddPopUp>();

        script.SetUp(item.itemInfo.icon, item.amount, item.itemInfo.itemName);
    }

}

public enum InteractionType
{
    None,
    Item,
    Door,
    Ladder,
    Chest,
    NPC,
}
