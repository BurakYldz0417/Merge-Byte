using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// GameController class manages the game logic and interactions.
// GameController sýnýfý oyun mantýðýný ve etkileþimlerini yönetir.
public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Slot[] slots;
    // Array of Slot objects representing game slots
    // Oyun slotlarýný temsil eden Slot nesneleri dizisi

    private Vector3 _target;
    // Target position for item movement
    // Öðe hareketi için hedef konum

    private ItemInfo carryingItem;
    // The item currently being carried by the player
    // Þu anda oyuncu tarafýndan taþýnan öðe

    private Dictionary<int, Slot> slotDictionary;
    // Dictionary to store Slot objects by their IDs
    // Yuva nesnelerini kimliklerine göre depolamak için sözlük


    public int bayt; // Player's bayt
    public int Kilobayt;// Player's kilobayt
    public int Megabayt;// Player's gigaobayt

    public TextMeshProUGUI[] curretbytstext;

    public int money;
    //Player's money
    //Oyuncunun parasý

    public TextMeshProUGUI moneytext,addpricetext;
    //money and addmoney texts
    //para ve addmoney textleri

    int addprice;
    //addbutton's price will be given at the start with a certain value and then it will increase
    //addbutton un fiyatý starta belli degerde verilecek ve sonra artacak
    private void Awake()
    {
        instance = this;
        Utils.InitResources();
        // Initialize game resources using Utils class
        // Utils sýnýfýný kullanarak oyun kaynaklarýný baþlat
    }

    private void Start()
    {
        slotDictionary = new Dictionary<int, Slot>();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].id = i;
            slotDictionary.Add(i, slots[i]);
        }
        money = 50;
        //baþlangýcta paramýz 50 $
        //our initial money is $50

        addprice = 25;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SendRayCast();
        }

        if (Input.GetMouseButton(0) && carryingItem)
        {
            OnItemSelected();
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Drop item
            SendRayCast();
        }

        
        UpdateSlotEarnings();
        // Eðer bayt sayýsý 1024'e ulaþýrsa
        if (bayt == 1024)
        {
            Kilobayt++;
            bayt = 0;
        }

        // Check if Kilobyte count has reached 1024, then increment Megabyte count.
        // Kilobayt sayýsý 1024'e ulaþtýðýnda, Megabayt sayýsýný artýr.
        if (Kilobayt == 1024)
        {
            Megabayt++;
            Kilobayt = 0;
        }

        // Update the text for current Byte, Kilobyte, and Megabyte values in the UI.
        // Arayüzdeki mevcut Bayt, Kilobayt ve Megabayt deðerlerini güncelle.
        curretbytstext[0].text = bayt + " Byte";
        curretbytstext[1].text = Kilobayt + " Kilobyte";
        curretbytstext[2].text = Megabayt + " Megabyte";

        moneytext.text = money + " $";
        //we print money on money text
        //para textine parayý yazdýrdýk

        addpricetext.text = addprice + " $";
        //ne kadar tutacagýný yazdýrdýk
        // we print how much it will cost
    }

    // Cast a ray from the mouse position and handle interactions
    // Fare konumundan bir ýþýn yayýnlayýn ve etkileþimleri yönetin
    void SendRayCast()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            // If the ray hits a slot, handle interactions
            // Iþýn bir yuvaya çarparsa, etkileþimleri ele alýn
            var slot = hit.transform.GetComponent<Slot>();
            if (slot.state == SlotState.Full && carryingItem == null)
            {
                // Grab an item from a full slot
                // Dolu bir yuvadan bir öðe al
                var itemGO = (GameObject)Instantiate(Resources.Load("Prefabs/ItemDummy"));
                itemGO.transform.position = slot.transform.position;
                itemGO.transform.localScale = Vector3.one * 2;

                carryingItem = itemGO.GetComponent<ItemInfo>();
                carryingItem.InitDummy(slot.id, slot.currentItem.id);

                slot.ItemGrabbed();
            }
            else if (slot.state == SlotState.Empty && carryingItem != null)
            {
                // Drop an item into an empty slot
                // Bir öðeyi boþ bir yuvaya býrakýn
                slot.CreateItem(carryingItem.itemId);
                Destroy(carryingItem.gameObject);
            }
            else if (slot.state == SlotState.Full && carryingItem != null)
            {
                // Attempt to merge an item with a full slot
                // Bir öðeyi tam yuvayla birleþtirmeye çalýþ
                if (slot.currentItem.id == carryingItem.itemId)
                {
                    print("Merged");
                    OnItemMergedWithTarget(slot.id);
                }
                else
                {
                    OnItemCarryFail();
                }
            }
        }
        else
        {
            if (!carryingItem)
            {
                return;
            }
            OnItemCarryFail();
        }
    }

    // Handle movement of the carried item
    // Taþýnan öðenin kol hareketi
    void OnItemSelected()
    {
        _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _target.z = 0;
        var delta = 10 * Time.deltaTime;

        delta *= Vector3.Distance(transform.position, _target);
        carryingItem.transform.position = Vector3.MoveTowards(carryingItem.transform.position, _target, delta);
    }

    // Handle merging an item with a target slot
    // Bir öðeyi hedef yuvayla birleþtirmeyi yönetin
    void OnItemMergedWithTarget(int targetSlotId)
    {
        var slot = GetSlotById(targetSlotId);
        Destroy(slot.currentItem.gameObject);

        slot.CreateItem(carryingItem.itemId + 1);

        Destroy(carryingItem.gameObject);
    }

    // Handle failure to carry an item
    // Bir öðeyi taþýma hatasý
    void OnItemCarryFail()
    {
        var slot = GetSlotById(carryingItem.slotId);
        slot.CreateItem(carryingItem.itemId);
        Destroy(carryingItem.gameObject);
    }

    // Place a random item into a random empty slot
    // Rastgele bir öðeyi rastgele boþ bir yuvaya yerleþtirin
    void PlaceRandomItem()
    {
        if (AllSlotsOccupied())
        {
            Debug.Log("No empty slot available!");
            return;
        }

        var rand = UnityEngine.Random.Range(0, slots.Length);
        var slot = GetSlotById(rand);

        while (slot.state == SlotState.Full)
        {
            rand = UnityEngine.Random.Range(0, slots.Length);
            slot = GetSlotById(rand);
        }

        slot.CreateItem(0);
    }

    // Check if all slots are occupied
    // Tüm yuvalarýn dolu olup olmadýðýný kontrol edin
    bool AllSlotsOccupied()
    {
        foreach (var slot in slots)
        {
            if (slot.state == SlotState.Empty)
            {
                // Empty slot found
                // Boþ alan bulundu
                return false;
            }
        }
        // No slot is empty
        // Boþ alan bulunamadý
        return true;
    }

    // Get a Slot object by its ID
    // Kimliðine göre bir Slot nesnesi alýn
    Slot GetSlotById(int id)
    {
        return slotDictionary[id];
    }

    // Update earnings for all slots
    // Tüm slotlar için kazançlarý güncelle
    void UpdateSlotEarnings()
    {
        foreach (var slot in slots)
        {
            if (slot.state == SlotState.Full)
            {
                slot.UpdateEarnings(Time.deltaTime);
            }
        }
    }

    public void sell()
    {
        money = money + (Megabayt * 1024 * 1024 + Kilobayt * 1024 + bayt);
        //here we converted all data storage to bytes and converted to a byte 1 $1
        //burada tüm veri deposunu bayta dönüþtürdük ve bayta 1 $1 dönüþtürdük

        Megabayt = 0;
        Kilobayt = 0;
        bayt = 0;

        //reset because they were all sold out
        //hepsi satýldýgý için sýfýrladýlar


    }
    public void addbutton()
    {
        if (money >= addprice)
        {
            PlaceRandomItem();
            money -= addprice;
            //money is running low
            //para azalýyor

            addprice += 25;
            //that we can call a new byte as soon as I can afford it
            //paramýn yettiði anda yeni bir bayt cagirabilecegimiz
        }
    }
}