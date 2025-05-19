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
    TextMeshPro AxeReq1, AxeReq2;

    bool isOpen;
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
        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<TextMeshPro>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<TextMeshPro>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(); });
    }

    private void CraftAnyItem()
    {
        throw new NotImplementedException();
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
}
