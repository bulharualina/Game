using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }

     // -- UI -- //
    public GameObject quickSlotsPanel;

    public List<GameObject> quickSlotsList = new List<GameObject>();


    public GameObject numbers;

    public int selectedNumber = -1;
    public GameObject selectedItem;

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


    private void Start()
    {
        PopulateSlotList();
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SelectQuickSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectQuickSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectQuickSlot(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectQuickSlot(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectQuickSlot(5);
        }
    }

    private void SelectQuickSlot(int number)
    {
        if (checkIfSlotIsFull(number))
        {
            if (selectedNumber != number)
            {
                selectedNumber = number;

                //unselect previously selected item
                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                }
                selectedItem = GetSelectedItem(number);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;


                //change color
                foreach (Transform child in numbers.transform)
                {
                    child.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;
                }
                TextMeshProUGUI toBeChanged = numbers.transform.Find("number" + number).transform.Find("Text").GetComponent<TextMeshProUGUI>();
                toBeChanged.color = Color.white;
            }
            else 
            {
                selectedNumber = -1;
                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;
                }

                foreach (Transform child in numbers.transform)
                {
                    child.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;
                }
            }
        }
      
    }

    private GameObject GetSelectedItem(int slotNumber)
    {
        return quickSlotsList[slotNumber-1].transform.GetChild(0).gameObject;
    }

    bool checkIfSlotIsFull(int slotNumber)
    {
        if (quickSlotsList[slotNumber - 1].transform.childCount > 0)
        {
            return true;
        }
        else 
        { 
            return false; 
        }
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        // Find next free slot
        GameObject availableSlot = FindNextEmptySlot();
        // Set transform of our object
        itemToEquip.transform.SetParent(availableSlot.transform, false);
        // Getting clean name
        string cleanName = itemToEquip.name.Replace("(Clone)", "");
      

        InventorySystem.Instance.ReCalculateList();

    }


    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckIfFull()
    {

        int counter = 0;

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
