using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;

    public List<string> inventoryItemList = new List<string>();

    //category Buttons
    Button toolsBTN;

    //Craft Buttons
    Button craftAxeBTN;

    //Req text
    TextMeshProUGUI AxeReq1, AxeReq2;

    bool isOpen;


    //blueprints

    public CraftingBlueprint AxeBLP = new CraftingBlueprint("Axe",2,"Stone",3,"Stick",3);
    public static CraftingSystem Instance { get; set; }


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
        isOpen = false;

        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        //Axe
        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<TMPro.TextMeshProUGUI>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<TMPro.TextMeshProUGUI>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });
    }

    private void CraftAnyItem(CraftingBlueprint blpToCraft)
    {
        InventorySystem.Instance.AddToInventory(blpToCraft.itemName);
        if (blpToCraft.numOfReq == 1) 
        {
            InventorySystem.Instance.RemoveItem(blpToCraft.Req1, blpToCraft.Req1amount);

        }else if (blpToCraft.numOfReq == 2)
        {
            InventorySystem.Instance.RemoveItem(blpToCraft.Req1, blpToCraft.Req1amount);
            InventorySystem.Instance.RemoveItem(blpToCraft.Req2, blpToCraft.Req2amount);
        }
    }

 
   

    void OpenToolsCategory() 
    { 
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.C) && !isOpen)
        {
            craftingScreenUI.SetActive(true);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen) {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            isOpen = false;

        }
    }

    public void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count = 0;

        inventoryItemList = InventorySystem.Instance.itemList;

        foreach (string itemName in inventoryItemList)
        {
            switch (itemName) 
            {
                case "Stone":
                    stone_count++;
                    break;
                case "Stick":
                    stick_count++;
                    break;
            }
        }

        //Axe
        AxeReq1.text = "3 Stone ["+ stone_count +"]";
        AxeReq2.text = "3 Stick [" + stick_count + "]";

        if (stone_count >= 3 && stick_count >= 3)
        {
            craftAxeBTN.gameObject.SetActive(true);
        }
        else 
        {
            craftAxeBTN.gameObject.SetActive(false);
        }

    }
}
