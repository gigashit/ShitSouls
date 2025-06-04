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
    }

    private async UniTaskVoid SetupInputEvents()
    {
        await UniTask.Delay(50);
        InputManager.Instance.inputActions.Player.Inventory.performed += ToggleInventory;
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

    public void AddItem(ItemInfoAsset itemInfo, int amount)
    {
        if (itemInfo.itemType == ItemType.Consumable)
        {
            InventoryItem existingConsumable = consumableItems.Find(i => i.itemId == itemInfo.id);

            for (int i = 0; i < amount; i++)
            {
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
            InventoryItem existingConsumable = collectedItems.Find(i => i.itemId == itemInfo.id);

            if (existingConsumable != null)
            {
                if (existingConsumable.amountHeld > 1)
                {
                    existingConsumable.amountHeld--;
                }
                else
                {
                    collectedItems.Remove(existingConsumable);
                }
            }
            else
            {
                Debug.LogError("Trying to remove item: " + existingConsumable.ItemInfo.itemName + ", Item not found in inventory");
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
            Debug.LogError("Trying to remove item: " + existingItem.ItemInfo.itemName + ", Item not found in inventory");
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

    public void UseCurrentConsumable()
    {
        if (consumableItems.Count == 0) return;

        InventoryItem current = consumableItems[selectedConsumableIndex];

        // Call effect logic

        RemoveItem(current.ItemInfo); // This will also remove from consumableItems
        if (selectedConsumableIndex >= consumableItems.Count)
            selectedConsumableIndex = Mathf.Max(consumableItems.Count - 1, 0);

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
