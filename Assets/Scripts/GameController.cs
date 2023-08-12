using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// GameController class manages the game logic and interactions.
// GameController s�n�f� oyun mant���n� ve etkile�imlerini y�netir.
public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Slot[] slots;
    // Array of Slot objects representing game slots
    // Oyun slotlar�n� temsil eden Slot nesneleri dizisi

    private Vector3 _target;
    // Target position for item movement
    // ��e hareketi i�in hedef konum

    private ItemInfo carryingItem;
    // The item currently being carried by the player
    // �u anda oyuncu taraf�ndan ta��nan ��e

    private Dictionary<int, Slot> slotDictionary;
    // Dictionary to store Slot objects by their IDs
    // Yuva nesnelerini kimliklerine g�re depolamak i�in s�zl�k


    public int bayt; // Player's bayt
    public int Kilobayt;// Player's kilobayt
    public int Megabayt;// Player's gigaobayt

    public TextMeshProUGUI[] curretbytstext;

    public int money;
    //Player's money
    //Oyuncunun paras�

    public TextMeshProUGUI moneytext,addpricetext;
    //money and addmoney texts
    //para ve addmoney textleri

    int addprice;
    //addbutton's price will be given at the start with a certain value and then it will increase
    //addbutton un fiyat� starta belli degerde verilecek ve sonra artacak
    private void Awake()
    {
        instance = this;
        Utils.InitResources();
        // Initialize game resources using Utils class
        // Utils s�n�f�n� kullanarak oyun kaynaklar�n� ba�lat
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
        //ba�lang�cta param�z 50 $
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
        // E�er bayt say�s� 1024'e ula��rsa
        if (bayt == 1024)
        {
            Kilobayt++;
            bayt = 0;
        }

        // Check if Kilobyte count has reached 1024, then increment Megabyte count.
        // Kilobayt say�s� 1024'e ula�t���nda, Megabayt say�s�n� art�r.
        if (Kilobayt == 1024)
        {
            Megabayt++;
            Kilobayt = 0;
        }

        // Update the text for current Byte, Kilobyte, and Megabyte values in the UI.
        // Aray�zdeki mevcut Bayt, Kilobayt ve Megabayt de�erlerini g�ncelle.
        curretbytstext[0].text = bayt + " Byte";
        curretbytstext[1].text = Kilobayt + " Kilobyte";
        curretbytstext[2].text = Megabayt + " Megabyte";

        moneytext.text = money + " $";
        //we print money on money text
        //para textine paray� yazd�rd�k

        addpricetext.text = addprice + " $";
        //ne kadar tutacag�n� yazd�rd�k
        // we print how much it will cost
    }

    // Cast a ray from the mouse position and handle interactions
    // Fare konumundan bir ���n yay�nlay�n ve etkile�imleri y�netin
    void SendRayCast()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            // If the ray hits a slot, handle interactions
            // I��n bir yuvaya �arparsa, etkile�imleri ele al�n
            var slot = hit.transform.GetComponent<Slot>();
            if (slot.state == SlotState.Full && carryingItem == null)
            {
                // Grab an item from a full slot
                // Dolu bir yuvadan bir ��e al
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
                // Bir ��eyi bo� bir yuvaya b�rak�n
                slot.CreateItem(carryingItem.itemId);
                Destroy(carryingItem.gameObject);
            }
            else if (slot.state == SlotState.Full && carryingItem != null)
            {
                // Attempt to merge an item with a full slot
                // Bir ��eyi tam yuvayla birle�tirmeye �al��
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
    // Ta��nan ��enin kol hareketi
    void OnItemSelected()
    {
        _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _target.z = 0;
        var delta = 10 * Time.deltaTime;

        delta *= Vector3.Distance(transform.position, _target);
        carryingItem.transform.position = Vector3.MoveTowards(carryingItem.transform.position, _target, delta);
    }

    // Handle merging an item with a target slot
    // Bir ��eyi hedef yuvayla birle�tirmeyi y�netin
    void OnItemMergedWithTarget(int targetSlotId)
    {
        var slot = GetSlotById(targetSlotId);
        Destroy(slot.currentItem.gameObject);

        slot.CreateItem(carryingItem.itemId + 1);

        Destroy(carryingItem.gameObject);
    }

    // Handle failure to carry an item
    // Bir ��eyi ta��ma hatas�
    void OnItemCarryFail()
    {
        var slot = GetSlotById(carryingItem.slotId);
        slot.CreateItem(carryingItem.itemId);
        Destroy(carryingItem.gameObject);
    }

    // Place a random item into a random empty slot
    // Rastgele bir ��eyi rastgele bo� bir yuvaya yerle�tirin
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
    // T�m yuvalar�n dolu olup olmad���n� kontrol edin
    bool AllSlotsOccupied()
    {
        foreach (var slot in slots)
        {
            if (slot.state == SlotState.Empty)
            {
                // Empty slot found
                // Bo� alan bulundu
                return false;
            }
        }
        // No slot is empty
        // Bo� alan bulunamad�
        return true;
    }

    // Get a Slot object by its ID
    // Kimli�ine g�re bir Slot nesnesi al�n
    Slot GetSlotById(int id)
    {
        return slotDictionary[id];
    }

    // Update earnings for all slots
    // T�m slotlar i�in kazan�lar� g�ncelle
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
        //burada t�m veri deposunu bayta d�n��t�rd�k ve bayta 1 $1 d�n��t�rd�k

        Megabayt = 0;
        Kilobayt = 0;
        bayt = 0;

        //reset because they were all sold out
        //hepsi sat�ld�g� i�in s�f�rlad�lar


    }
    public void addbutton()
    {
        if (money >= addprice)
        {
            PlaceRandomItem();
            money -= addprice;
            //money is running low
            //para azal�yor

            addprice += 25;
            //that we can call a new byte as soon as I can afford it
            //param�n yetti�i anda yeni bir bayt cagirabilecegimiz
        }
    }
}