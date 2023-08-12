using UnityEngine;

public class ItemInfo : MonoBehaviour 
{
    public int slotId;
    public int itemId;

    public SpriteRenderer visualRenderer;

    // Initialize the item information for a dummy item.
    // Sahte bir öðe için öðe bilgisini baþlatýr.
    public void InitDummy(int slotId, int itemId) 
    {
        this.slotId = slotId;
        this.itemId = itemId;
        visualRenderer.sprite = Utils.GetItemVisualById(itemId);
    }
}