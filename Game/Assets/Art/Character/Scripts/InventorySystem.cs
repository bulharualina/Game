using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;

    public List<GameObject> slotList = new List<GameObject>();

    public List<string> itemList = new List<string>();

    private GameObject _itemToAdd;
    private GameObject _slotToEquip;
    public bool isOpen;




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

        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
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

        //_itemToAdd = (GameObject)Instantiate(Resources.Load<GameObject>(itemName), _slotToEquip.transform.position, _slotToEquip.transform.rotation);
        string uiPrefabName = itemName + "Icon";
        GameObject prefabToLoad = Resources.Load<GameObject>(uiPrefabName);
        if (prefabToLoad == null)
        {
            Debug.LogError($"UI prefab '{uiPrefabName}Icon' not found in Resources.");
            return;
        }

        _itemToAdd = Instantiate(prefabToLoad, _slotToEquip.transform);
        _itemToAdd.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        _itemToAdd.GetComponent<RectTransform>().localScale = Vector3.one;

        _itemToAdd.transform.SetParent(_slotToEquip.transform);

         itemList.Add(itemName);
        


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
            return true;
        }
        else
        {
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
