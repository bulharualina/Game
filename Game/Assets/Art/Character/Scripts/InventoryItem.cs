using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // --- Is this item trashable --- //
    public bool isTrashable;

    // --- Item Info UI --- //
    private GameObject _itemInfoUI;

    private TextMeshProUGUI _itemInfoUI_itemName;
    private TextMeshProUGUI _itemInfoUI_itemDescription;
    private TextMeshProUGUI _itemInfoUI_itemFunctionality;

    public string thisName, thisDescription, thisFunctionality;

    // --- Consumption --- //
    private GameObject _itemPendingConsumption;
    public bool isConsumable;

    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;

    // --- Equipping --- //
    public bool isEquippable;
    private GameObject _itemPendingEquipping;
    public bool isInsideQuickSlot;//is in slot

    public bool isSelected;//item  selected

    public bool isUsable;
    public GameObject itemPendingToBeUsed;

    private void Awake()
    {
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem.Instance is null when InventoryItem is trying to initialize! Make sure InventorySystem initializes first.");
            return;
        }
        _itemInfoUI = InventorySystem.Instance.ItemInfoUI;
       
        if (_itemInfoUI == null)
        {
            Debug.LogError("ItemInfoUI is not assigned in InventorySystem!", InventorySystem.Instance.gameObject);
            return;
        }

        _itemInfoUI_itemName = _itemInfoUI.transform.Find("itemName").GetComponent<TextMeshProUGUI>();
        _itemInfoUI_itemDescription = _itemInfoUI.transform.Find("itemDescription").GetComponent<TextMeshProUGUI>();
        _itemInfoUI_itemFunctionality = _itemInfoUI.transform.Find("itemFunctionality").GetComponent<TextMeshProUGUI>();

        if (_itemInfoUI_itemName == null) Debug.LogError("itemName TextMeshProUGUI not found on ItemInfoUI child!", _itemInfoUI);
        if (_itemInfoUI_itemDescription == null) Debug.LogError("itemDescription TextMeshProUGUI not found on ItemInfoUI child!", _itemInfoUI);
        if (_itemInfoUI_itemFunctionality == null) Debug.LogError("itemFunctionality TextMeshProUGUI not found on ItemInfoUI child!", _itemInfoUI);
    }

    void Update() 
    {
        if (isSelected)
        {
            gameObject.GetComponent<DragDrop>().enabled = false;
        }
        else 
        { 
            gameObject.gameObject.GetComponent<DragDrop>().enabled = true;
        }
    
    }

    // Triggered when the mouse enters into the area of the item that has this script.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemInfoUI == null || _itemInfoUI_itemName == null || _itemInfoUI_itemDescription == null || _itemInfoUI_itemFunctionality == null)
        {
            Debug.LogWarning("Item info UI components not initialized. Cannot display info.");
            return;
        }
        _itemInfoUI.SetActive(true);
        _itemInfoUI_itemName.text = thisName;
        _itemInfoUI_itemDescription.text = thisDescription;
        _itemInfoUI_itemFunctionality.text = thisFunctionality;
    }

    // Triggered when the mouse exits the area of the item that has this script.
    public void OnPointerExit(PointerEventData eventData)
    {
        _itemInfoUI.SetActive(false);
    }

    // Triggered when the mouse is clicked over the item that has this script.
    public void OnPointerDown(PointerEventData eventData)
    {
        //Right Mouse Button Click on
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable)
            {
                // Setting this specific gameobject to be the item we want to destroy later
                _itemPendingConsumption = gameObject;
                consumingFunction(healthEffect, caloriesEffect, hydrationEffect);
            }

            if (isEquippable && !isInsideQuickSlot && !EquipSystem.Instance.CheckIfFull())
            {
                EquipSystem.Instance.AddToQuickSlots(gameObject);
                isInsideQuickSlot = true;

            }

            if (isUsable) 
            {
                itemPendingToBeUsed = gameObject;

                UseItem();
            }
        }

    }

    private void UseItem()
    {
        _itemInfoUI.SetActive(false);

        InventorySystem.Instance.isOpen = false;
        InventorySystem.Instance.inventoryScreenUI.SetActive(false);

        CraftingSystem.Instance.isOpen = false;
        CraftingSystem.Instance.craftingScreenUI.SetActive(false);
        CraftingSystem.Instance.toolsScreenUI.SetActive(false);
        CraftingSystem.Instance.constructionsScreenUI.SetActive(false);

    }

    // Triggered when the mouse button is released over the item that has this script.
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable && _itemPendingConsumption == gameObject)
            {
                DestroyImmediate(gameObject);
                InventorySystem.Instance.ReCalculateList();
                CraftingSystem.Instance.RefreshNeededItems();
            }
            if (isUsable && itemPendingToBeUsed == gameObject)
            {
                DestroyImmediate(gameObject);
                InventorySystem.Instance.ReCalculateList();
                CraftingSystem.Instance.RefreshNeededItems();
            }
        }
    }

    private void consumingFunction(float healthEffect, float caloriesEffect, float hydrationEffect)
    {
        Debug.Log("Consuming Function called for item: " + thisName);
        _itemInfoUI.SetActive(false);

        // Crucial null check here
        if (PlayerSurvivalStats.Instance == null)
        {
            Debug.LogError("PlayerSurvivalStats.Instance is NULL! Cannot apply consumption effects for " + thisName + ".");
            return; // Stop execution if instance is null to prevent NRE
        }

        healthEffectCalculation(healthEffect);

        caloriesEffectCalculation(caloriesEffect);

        hydrationEffectCalculation(hydrationEffect);
        Debug.Log("Consumption effects applied (attempted) for " + thisName + ".");
    }


    private static void healthEffectCalculation(float healthEffect)
    {
        // --- Health --- //
        Debug.Log("Applying Health Effect: " + healthEffect);
        if (PlayerSurvivalStats.Instance == null)
        {
            Debug.LogError("PlayerSurvivalStats.Instance is NULL inside healthEffectCalculation!");
            return;
        }
        float healthBeforeConsumption = PlayerSurvivalStats.Instance.currentHealth;
        float maxHealth = PlayerSurvivalStats.Instance.maxHealth;

        if (healthEffect != 0)
        {
            if ((healthBeforeConsumption + healthEffect) > maxHealth)
            {
                PlayerSurvivalStats.Instance.setHealth(maxHealth);
            }
            else
            {
                PlayerSurvivalStats.Instance.setHealth(healthBeforeConsumption + healthEffect);
            }
        }
    }


    private static void caloriesEffectCalculation(float caloriesEffect)
    {
        // --- Calories --- //
        Debug.Log("Applying Calories Effect: " + caloriesEffect);
        if (PlayerSurvivalStats.Instance == null)
        {
            Debug.LogError("PlayerSurvivalStats.Instance is NULL inside caloriesEffectCalculation!");
            return;
        }
        float caloriesBeforeConsumption = PlayerSurvivalStats.Instance.currentCalories;
        float maxCalories = PlayerSurvivalStats.Instance.maxCalories;

        if (caloriesEffect != 0)
        {
            if ((caloriesBeforeConsumption + caloriesEffect) > maxCalories)
            {
                PlayerSurvivalStats.Instance.setCalories(maxCalories);
            }
            else
            {
                PlayerSurvivalStats.Instance.setCalories(caloriesBeforeConsumption + caloriesEffect);
            }
        }
    }


    private static void hydrationEffectCalculation(float hydrationEffect)
    {
        // --- Hydration --- //
        Debug.Log("Applying Hydration Effect: " + hydrationEffect);
        if (PlayerSurvivalStats.Instance == null)
        {
            Debug.LogError("PlayerSurvivalStats.Instance is NULL inside hydrationEffectCalculation!");
            return;
        }
        float hydrationBeforeConsumption = PlayerSurvivalStats.Instance.currentHydrationPercent;
        float maxHydration = PlayerSurvivalStats.Instance.maxHydrationPercent;

        if (hydrationEffect != 0)
        {
            if ((hydrationBeforeConsumption + hydrationEffect) > maxHydration)
            {
                PlayerSurvivalStats.Instance.setHydration(maxHydration);
            }
            else
            {
                PlayerSurvivalStats.Instance.setHydration(hydrationBeforeConsumption + hydrationEffect);
            }
        }
    }


}
