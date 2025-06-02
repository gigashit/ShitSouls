using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryEntry : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text amountNumber;
    private InventoryManager inventoryManager;
    private ItemInfoAsset itemInfo;

    public void SetUp(Sprite sprite, InventoryManager im, ItemInfoAsset iio, int amount)
    {
        image.sprite = sprite;
        inventoryManager = im;
        itemInfo = iio;
        amountNumber.text = amount.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryManager.ShowItemInfo(itemInfo);
    }

    public void OnSelect(BaseEventData eventData)
    {
        inventoryManager.ShowItemInfo(itemInfo);
    }
}
