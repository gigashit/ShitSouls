using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> collectedItems = new List<InventoryItem>();
    public List<InventoryItem> consumableItems = new List<InventoryItem>();

    [Header("Inventory UI Elements")]
    public GameObject inventoryScreen;
    public GameObject itemButtonPrefab;
    public Transform gridParent;
    public TMP_Text inspectName;
    public TMP_Text inspectDescription;
    public Image inspectIcon;

    [Header("Consumable UI Elements")]
    public Image prevIcon;
    public Image currentIcon;
    public Image nextIcon;
    public TMP_Text consumableAmountText;

    [Header("Script References")]
    [SerializeField] private PlayerMovementController playerMovementController;
    [SerializeField] private PlayerInteractionHandler playerInteractionHandler;
    [SerializeField] private ThirdPersonCameraController thirdPersonCameraController;

    private Coroutine populateCoroutine;

    private int selectedConsumableIndex = 0;

    public bool isInventoryOpen = false;

    private void Start()
    {
        inventoryScreen.SetActive(false);
        PopulateInventoryUI();
        UpdateConsumableUI();
    }

    private void OnEnable()
    {
        SetupInputEvents();
    }

    private void OnDisable()
    {
        InputManager.Instance.inputActions.Player.Inventory.performed -= ToggleInventory;
        InputManager.Instance.inputActions.Player.Roll.performed -= ExitInventory;
        InputManager.Instance.inputActions.Player.Previous.performed -= OnPreviousConsumable;
        InputManager.Instance.inputActions.Player.Next.performed -= OnNextConsumable;
        InputManager.Instance.inputActions.UI.ScrollWheel.performed -= OnScrollWheel;
        InputManager.Instance.inputActions.Player.UseItem.performed -= UseCurrentConsumable;
    }

    private async UniTaskVoid SetupInputEvents()
    {
        await UniTask.Delay(50);
        InputManager.Instance.inputActions.Player.Inventory.performed += ToggleInventory;
        InputManager.Instance.inputActions.Player.Roll.performed += ExitInventory;
        InputManager.Instance.inputActions.Player.Previous.performed += OnPreviousConsumable;
        InputManager.Instance.inputActions.Player.Next.performed += OnNextConsumable;
        InputManager.Instance.inputActions.UI.ScrollWheel.performed += OnScrollWheel;
        InputManager.Instance.inputActions.Player.UseItem.performed += UseCurrentConsumable;
    }

    private void ToggleInventory(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (playerInteractionHandler.isInteracting) return;

        if (!isInventoryOpen)
        {
            inventoryScreen.SetActive(true);
            isInventoryOpen = true;
            playerMovementController.isLocked = true;
            thirdPersonCameraController.cameraInputEnabled = false;
        }
        else
        {
            inventoryScreen.SetActive(false);
            isInventoryOpen = false;
            playerMovementController.isLocked = false;
            thirdPersonCameraController.cameraInputEnabled = true;
        }
    }

    private void ExitInventory(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (isInventoryOpen)
        {
            inventoryScreen.SetActive(false);
            isInventoryOpen = false;
            playerMovementController.isLocked = false;
            thirdPersonCameraController.cameraInputEnabled = true;
        }
    }

    private void OnPreviousConsumable(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!playerInteractionHandler.isInteracting && !isInventoryOpen)
            CycleConsumable(-1);
    }

    private void OnNextConsumable(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!playerInteractionHandler.isInteracting && !isInventoryOpen)
            CycleConsumable(1);
    }

    private void OnScrollWheel(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!playerInteractionHandler.isInteracting && !isInventoryOpen)
        {
            Vector2 scrollValue = ctx.ReadValue<Vector2>();
            if (scrollValue.y > 0)
                CycleConsumable(-1);
            else if (scrollValue.y < 0)
                CycleConsumable(1);
        }
    }

    public void AddItem(ItemInfoAsset itemInfo, int amount)
    {
        if (itemInfo.itemType == ItemType.Consumable)
        {
            InventoryItem existingConsumable = consumableItems.Find(i => i.itemId == itemInfo.id);

             if (existingConsumable != null)
             {
                 existingConsumable.amountHeld += amount;
             }
            else
             {
                InventoryItem newConsumable = new InventoryItem
                {
                    ItemInfo = itemInfo,
                    itemId = itemInfo.id,
                    amountHeld = amount
                 };

                consumableItems.Add(newConsumable);
            }

            UpdateConsumableUI();
        }

        InventoryItem existingItem = collectedItems.Find(i => i.itemId == itemInfo.id);

        if (existingItem != null)
        {
            existingItem.amountHeld += amount;
        }
        else
        {
            InventoryItem itemToAdd = new InventoryItem
            {
                ItemInfo = itemInfo,
                itemId = itemInfo.id,
                amountHeld = amount
            };

            collectedItems.Add(itemToAdd);
        }

        if (populateCoroutine != null) { StopCoroutine(populateCoroutine); populateCoroutine = null; }
        StartCoroutine(PopulateInventoryUI());
    }

    public void RemoveItem(ItemInfoAsset itemInfo)
    {
        if (itemInfo.itemType == ItemType.Consumable)
        {
            InventoryItem existingConsumable = consumableItems.Find(i => i.itemId == itemInfo.id);

            if (existingConsumable != null)
            {
                int consumableIndex = consumableItems.IndexOf(existingConsumable);
                
                if (existingConsumable.amountHeld > 1)
                {
                    existingConsumable.amountHeld--;
                }
                else
                {
                    consumableItems.Remove(existingConsumable);
                    // If we removed the currently selected item or an item before it,
                    // we need to adjust the selected index
                    if (consumableIndex <= selectedConsumableIndex)
                    {
                        selectedConsumableIndex = Mathf.Max(0, selectedConsumableIndex - 1);
                    }
                }

                // Ensure selectedConsumableIndex is within bounds
                if (consumableItems.Count > 0)
                {
                    selectedConsumableIndex = Mathf.Min(selectedConsumableIndex, consumableItems.Count - 1);
                }
                else
                {
                    selectedConsumableIndex = 0;
                }

                UpdateConsumableUI();
            }
            else
            {
                Debug.LogError("Trying to remove consumable item: " + itemInfo.itemName + ", Item not found in consumables");
            }
        }

        InventoryItem existingItem = collectedItems.Find(i => i.itemId == itemInfo.id);

        if (existingItem != null)
        {
            if (existingItem.amountHeld > 1)
            {
                existingItem.amountHeld--;
            }
            else
            {
                collectedItems.Remove(existingItem);
            }
        }
        else
        {
            Debug.LogError("Trying to remove item: " + itemInfo.itemName + ", Item not found in inventory");
        }

        if (populateCoroutine != null) { StopCoroutine(populateCoroutine); populateCoroutine = null; }
        StartCoroutine(PopulateInventoryUI());
    }

    private IEnumerator PopulateInventoryUI()
    {
        if (collectedItems.Count <= 0)
        {
            inspectName.text = "";
            inspectDescription.text = "";
            inspectIcon.enabled = false;
        }
        else
        {
            inspectIcon.enabled = true;
            // Shuffle here?           
        }

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        int index = 0;
        bool isFirst = true;

        foreach (var item in collectedItems)
        {
            GameObject buttonObj = Instantiate(itemButtonPrefab, gridParent);
            InventoryEntry entry = buttonObj.GetComponent<InventoryEntry>();

            entry.SetUp(item.ItemInfo.icon, this, item.ItemInfo, item.amountHeld);

            index++;

            if (index > 4)
            {
                yield return null;
                index = 0;
            }

            if (isFirst)
            {
                EventSystem.current.SetSelectedGameObject(buttonObj);
                ShowItemInfo(item.ItemInfo);
                isFirst = false;
            }
        }
    }

    public void ShowItemInfo(ItemInfoAsset itemInfo)
    {
        inspectName.text = itemInfo.itemName;
        inspectDescription.text = itemInfo.description;
        inspectIcon.sprite = itemInfo.icon;
    }

    public void CycleConsumable(int direction)
    {
        if (consumableItems.Count == 0) return;

        selectedConsumableIndex = (selectedConsumableIndex + direction + consumableItems.Count) % consumableItems.Count;
        UpdateConsumableUI();
    }

    public void UseCurrentConsumable(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (consumableItems.Count == 0) return;
        if (playerInteractionHandler.isInteracting || isInventoryOpen) return;

        InventoryItem current = consumableItems[selectedConsumableIndex];

        // Call effect logic
        Debug.Log("Using consumable: " + current.ItemInfo.itemName);

        RemoveItem(current.ItemInfo); // This will also remove from consumableItems and handle index adjustment
        UpdateConsumableUI();
    }

    private void UpdateConsumableUI()
    {
        if (consumableItems.Count == 0)
        {
            prevIcon.enabled = currentIcon.enabled = nextIcon.enabled = false;
            consumableAmountText.text = "";
            return;
        }

        currentIcon.sprite = consumableItems[selectedConsumableIndex].ItemInfo.icon;
        consumableAmountText.text = consumableItems[selectedConsumableIndex].amountHeld.ToString();
        currentIcon.enabled = true;

        if (consumableItems.Count > 1)
        {
            int count = consumableItems.Count;
            int prevIndex = (selectedConsumableIndex - 1 + count) % count;
            int nextIndex = (selectedConsumableIndex + 1) % count;

            prevIcon.sprite = consumableItems[prevIndex].ItemInfo.icon;
            nextIcon.sprite = consumableItems[nextIndex].ItemInfo.icon;
            prevIcon.enabled = nextIcon.enabled = true;
        }
        else
        {
            prevIcon.enabled = nextIcon.enabled = false;
        }
    }
}

public class InventoryItem
{
    public ItemInfoAsset ItemInfo;
    public int amountHeld;
    public int itemId;
}
