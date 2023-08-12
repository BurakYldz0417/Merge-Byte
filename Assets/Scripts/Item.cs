using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int id;
    public Slot parentSlot;

    public SpriteRenderer visualRenderer;

    // Initialize the item with its ID and parent slot.
    // Öğeyi kimliği ve üst öğe yuvası ile başlatır.
    public void Init(int id, Slot slot)
    {
        this.id = id;
        this.parentSlot = slot;
        visualRenderer.sprite = Utils.GetItemVisualById(id);
    }
}
