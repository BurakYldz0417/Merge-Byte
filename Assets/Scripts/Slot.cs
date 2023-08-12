using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Slot class represents a slot that can hold an item.
// Slot sınıfı, bir öğeyi tutabilen bir yuva temsil eder.
public class Slot : MonoBehaviour
{
    public int id;
    public Item currentItem;
    public SlotState state = SlotState.Empty;

    public float baseEarningRate = 1.0f;
    public float bonusEarningRate = 2.0f;
    private float currentEarnings = 0.0f;

    // Create an item in the slot with the given item id.
    // Verilen öğe kimliğiyle bir öğe oluşturur.
    public void CreateItem(int id) 
    {
        var itemGO = (GameObject)Instantiate(Resources.Load("Prefabs/Item"));
        
        itemGO.transform.SetParent(this.transform);
        itemGO.transform.localPosition = Vector3.zero;
        itemGO.transform.localScale = Vector3.one;

        currentItem = itemGO.GetComponent<Item>();
        currentItem.Init(id, this);

        ChangeStateTo(SlotState.Full);
    }

    // Change the state of the slot to the target state.
    // Yuva durumunu hedef duruma değiştirir.
    private void ChangeStateTo(SlotState targetState)
    {
        state = targetState;
    }

    // Handle the event of an item being grabbed from the slot.
    // Bir öğenin yuvadan alındığı olayı işler.
    public void ItemGrabbed()
    {
        Destroy(currentItem.gameObject);
        ChangeStateTo(SlotState.Empty);
    }

    // Update the earnings of the slot based on the elapsed time.
    // Geçen süreye göre yuvanın kazançlarını günceller.
    public void UpdateEarnings(float deltaTime)
    {
        // Calculate earnings based on the current item and earning rates
        if (state == SlotState.Full)
        {
            currentEarnings += deltaTime * (currentItem.id == 0 ? baseEarningRate : bonusEarningRate);
            if (currentEarnings >= 1.0f)
            {
                GameController.instance.bayt += (int)currentEarnings;
                currentEarnings -= (int)currentEarnings;
            }
        }
    }
    // Receive an item with the given id into the slot.
    // Verilen kimliğe sahip bir öğeyi yuvaya alır.
    private void ReceiveItem(int id)
    {
        switch (state)
        {
            case SlotState.Empty: 

                CreateItem(id);
                ChangeStateTo(SlotState.Full);
                break;

            case SlotState.Full: 
                if (currentItem.id == id)
                {
                    //Merged
                    //Birleştirdik
                    Debug.Log("Merged");
                }
                else
                {
                    //Push item back
                    //Itemleri geri cagırdık
                    Debug.Log("Push back");
                }
                break;
        }
    }
}
// Enumeration representing the state of a slot.
// Bir yuvanın durumunu temsil eden numaralandırma.
public enum SlotState
{
    Empty,
    Full
}