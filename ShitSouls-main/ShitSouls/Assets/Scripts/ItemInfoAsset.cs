using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfoAsset", menuName = "Scriptable Objects/ItemInfoAsset")]
public class ItemInfoAsset : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string description;
    public int id;
}

public enum ItemType
{
    KeyItem,
    Trash,
    Consumable,
    Equipment,
    Weapon
}
