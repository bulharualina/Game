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
    public GameObject constructionsScreenUI;
    public GameObject refineProcessScreenUI;


    public List<string> inventoryItemList = new List<string>();

    //category Buttons
    Button _toolsBTN;
    Button _constructionBTN;
    Button _refineProcessBTN;

    //Craft Buttons
    Button _craftAxeBTN;
    Button _craftPlankBTN;
    Button _craftFoundationBTN;
    Button _craftWallBTN;

    //Back Buttons
    Button _toolsBackBTN;
    Button _constructionBackBTN;
    Button _refineProcessBackBTN;



    //Req text
    TextMeshProUGUI _AxeReq1, _AxeReq2;
    TextMeshProUGUI _PlankReq;
    TextMeshProUGUI _FoundationReq;
    TextMeshProUGUI _WallReq;

    public bool isOpen;


    //blueprints

    public CraftingBlueprint AxeBLP = new CraftingBlueprint("Axe",2,"Stone",3,"Stick",3);
    public CraftingBlueprint PlankBLP = new CraftingBlueprint("Plank",1,"Log",1);
    public CraftingBlueprint FoundationBLP = new CraftingBlueprint("Foundation",1, "Plank", 4);
    public CraftingBlueprint WallBLP = new CraftingBlueprint("Wall", 1, "Plank", 2);
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
        //Buttons
        _toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        _toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });
       
        _constructionBTN = craftingScreenUI.transform.Find("ConstructionButton").GetComponent<Button>();
        _constructionBTN.onClick.AddListener(delegate { OpenConstructionsCategory(); });

        _refineProcessBTN = craftingScreenUI.transform.Find("RefineProcessButton").GetComponent<Button>();
        _refineProcessBTN.onClick.AddListener(delegate { OpenRefineProcessCategory(); });

        //-----TOOLS------
        //Axe
        _AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<TMPro.TextMeshProUGUI>();
        _AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<TMPro.TextMeshProUGUI>();
        _craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        _craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });

        //-----Construction-----
        //Foundation
        _FoundationReq = constructionsScreenUI.transform.Find("Foundation").transform.Find("req").GetComponent<TextMeshProUGUI>();
        _craftFoundationBTN = constructionsScreenUI.transform.Find("Foundation").transform.Find("Button").GetComponent<Button>();
        _craftFoundationBTN.onClick.AddListener(delegate { CraftAnyItem(FoundationBLP); });

        //Wall
        _WallReq = constructionsScreenUI.transform.Find("Wall").transform.Find("req").GetComponent<TextMeshProUGUI>();
        _craftWallBTN = constructionsScreenUI.transform.Find("Wall").transform.Find("Button").GetComponent<Button>();
        _craftWallBTN.onClick.AddListener(delegate { CraftAnyItem(WallBLP); });

        //-----Refine & Process-----
        //Plank
        _PlankReq = refineProcessScreenUI.transform.Find("Plank").transform.Find("req").GetComponent<TextMeshProUGUI>();
        _craftPlankBTN = refineProcessScreenUI.transform.Find("Plank").transform.Find("Button").GetComponent<Button>();
        _craftPlankBTN.onClick.AddListener(delegate { CraftAnyItem(PlankBLP); });

        //Back buttons
        _toolsBackBTN = toolsScreenUI.transform.Find("BackButton").GetComponent<Button>();
        _toolsBackBTN.onClick.AddListener(delegate { BackToCraftingCategories(); });

        _constructionBackBTN = constructionsScreenUI.transform.Find("BackButton").GetComponent<Button>();
        _constructionBackBTN.onClick.AddListener(delegate { BackToCraftingCategories(); });

        _refineProcessBackBTN = refineProcessScreenUI.transform.Find("BackButton").GetComponent<Button>();
        _refineProcessBackBTN.onClick.AddListener(delegate { BackToCraftingCategories(); });
    }

    private void BackToCraftingCategories()
    {
        toolsScreenUI.SetActive(false);
        constructionsScreenUI.SetActive(false);
        refineProcessScreenUI.SetActive(false);
        craftingScreenUI.SetActive(true);
    }

    private void OpenRefineProcessCategory()
    {
        craftingScreenUI.SetActive(false);
        refineProcessScreenUI.SetActive(true);
    }

    private void OpenConstructionsCategory()
    {
        craftingScreenUI.SetActive(false);
        constructionsScreenUI.SetActive(true);
    }
    void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }

    private void CraftAnyItem(CraftingBlueprint blpToCraft)
    {
       //Debug.Log("Attempting to craft: " + blpToCraft.itemName);
        InventorySystem.Instance.AddToInventory(blpToCraft.itemName);
        if (blpToCraft.itemName == "Plank") 
        {
            InventorySystem.Instance.AddToInventory(blpToCraft.itemName);
           // InventorySystem.Instance.AddToInventory(blpToCraft.itemName);

        }
        if (blpToCraft.numOfReq == 1) 
        {
            InventorySystem.Instance.RemoveItem(blpToCraft.Req1, blpToCraft.Req1amount);

        }else if (blpToCraft.numOfReq == 2)
        {
            InventorySystem.Instance.RemoveItem(blpToCraft.Req1, blpToCraft.Req1amount);
            InventorySystem.Instance.RemoveItem(blpToCraft.Req2, blpToCraft.Req2amount);
        }
    }

 
   

  

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.C) && !isOpen && !ConstructionManager.Instance.inConstructionMode)
        {
            craftingScreenUI.SetActive(true);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen) {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            constructionsScreenUI.SetActive(false);
            refineProcessScreenUI.SetActive(false);
            isOpen = false;

        }
    }

    public void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count = 0;
        int log_count = 0;
        int plank_count = 0;

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
                case "Log":
                    log_count++;
                    break;
                case "Plank":
                    plank_count++;
                    break;
            }
        }

        //Axe
        _AxeReq1.text = "3 Stone ["+ stone_count +"]";
        _AxeReq2.text = "3 Stick [" + stick_count + "]";

        if (stone_count >= 3 && stick_count >= 3)
        {
            _craftAxeBTN.gameObject.SetActive(true);
        }
        else 
        {
            _craftAxeBTN.gameObject.SetActive(false);
        }

        //Plank
        _PlankReq.text = "1 Log [" + log_count + "]";
        if (log_count >= 1)
        {
            _craftPlankBTN.gameObject.SetActive(true);
        }
        else 
        {
            _craftPlankBTN.gameObject.SetActive(false);
        }

        //Foundation
        _FoundationReq.text = "4 Plank [" + plank_count +"]";
        if (plank_count >= 4)
        {
            _craftFoundationBTN.gameObject.SetActive(true);
        }
        else 
        {
            _craftFoundationBTN.gameObject.SetActive(false);
        }

        //Wall
        _WallReq.text = "2 Plank [" + plank_count + "]";
        if (plank_count >= 2)
        {
            _craftWallBTN.gameObject.SetActive(true);
        }
        else
        {
            _craftWallBTN.gameObject.SetActive(false);
        }
    }
}
