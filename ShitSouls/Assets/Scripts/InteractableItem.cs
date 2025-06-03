using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public ItemInfoAsset itemInfo;
    public int amount;

    public void DeleteItem()
    {
        Destroy(gameObject);
    }
}
