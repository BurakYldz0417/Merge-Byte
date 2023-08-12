using UnityEngine;

public class ItemInfo : MonoBehaviour 
{
    public int slotId;
    public int itemId;

    public SpriteRenderer visualRenderer;

    // Initialize the item information for a dummy item.
    // Sahte bir ��e i�in ��e bilgisini ba�lat�r.
    public void InitDummy(int slotId, int itemId) 
    {
        this.slotId = slotId;
        this.itemId = itemId;
        visualRenderer.sprite = Utils.GetItemVisualById(itemId);
    }
}