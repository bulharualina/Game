using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public GameObject ItemInfoUI;


    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;

    public List<GameObject> slotList = new List<GameObject>();

    public List<string> itemList = new List<string>();

    private GameObject _itemToAdd;
    private GameObject _slotToEquip;
    public bool isOpen;

    //pickup PopUp
    public GameObject pickupPopUp;
    public GameObject inventoryFullPopUp;
    public TextMeshProUGUI pickupName;
    public Image pickupImage;

    public List<string> pickedupItems;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    void Start()
    {
        isOpen = false;
       
        PopulateSlotList();
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I) && !isOpen && !ConstructionManager.Instance.inConstructionMode)
        {

            Debug.Log("i is pressed");
            inventoryScreenUI.SetActive(true);
            isOpen = true;


        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            isOpen = false;
        }
    }

    public void AddToInventory(string itemName)
    {
      
         _slotToEquip = FindNextEmptySlot();
   
        _itemToAdd = (GameObject)Instantiate(Resources.Load<GameObject>(itemName), _slotToEquip.transform.position, _slotToEquip.transform.rotation);
       

        _itemToAdd.transform.SetParent(_slotToEquip.transform);

         itemList.Add(itemName);

        ShowPickupPopup(itemName, _itemToAdd.GetComponent<Image>().sprite);

        ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }

    void ShowPickupPopup(string itemName, Sprite itemSprite) 
    { 
        pickupPopUp.SetActive(true);

        pickupName.text = itemName;
        pickupImage.sprite = itemSprite;

        StartCoroutine(HidePickupPopupAfterDelay(2f));

    }
    IEnumerator HidePickupPopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        pickupPopUp.SetActive(false);
    }
    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }


        }

        return new GameObject();
    }

    public bool CheckIfInventoryFull()
    {
        int counter = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 15)
        {
            inventoryFullPopUp.SetActive(true);
            return true;
            
        }
        else
        {
            inventoryFullPopUp.SetActive(false);
            return false;
        }
    }

    public void RemoveItem(string nameToRemove, int amountToRemove) 
    { 
        int counter = amountToRemove;

        for (var i = slotList.Count-1; i>= 0;  i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter != 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    counter -= 1;
                }
            }
           
        }

        StartCoroutine(DelayedRecalculate());
    }
    private IEnumerator DelayedRecalculate()
    {
        yield return new WaitForSeconds(0.1f);
        ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }

    public void ReCalculateList() 
    { 
        itemList.Clear();

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0) 
            { 
                string name = slot.transform.GetChild(0).name;
                string strToRemove = "(Clone)";
                string res = name.Replace(strToRemove, "");
                
                itemList.Add(res);
            }
        }


    }
}
