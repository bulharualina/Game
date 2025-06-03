using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    private Button _slotButton;
    private TextMeshProUGUI _buttonText;

    public int slotNum;

    public GameObject alertUI;
    private Button _yesButton;
    private Button _noButton;


    private void Awake()
    {
        _slotButton = GetComponent<Button>();
        _buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        _yesButton = alertUI.transform.Find("YesButton").GetComponent<Button>();
        _noButton = alertUI.transform.Find("NoButton").GetComponent<Button>();
    }

    public void Start()
    {
        _slotButton.onClick.AddListener(() => 
        {
            if (SaveManager.Instance.IsSlotEmpty(slotNum))
            {
                SaveGameConfirmed();
            }
            else 
            {
                DisplayOverrideAlert();


            }
        
        });
    }
    private void Update()
    {
        if (SaveManager.Instance.IsSlotEmpty(slotNum))
        {
            _buttonText.text = "Empty";
        }
        else 
        {
            _buttonText.text = PlayerPrefs.GetString("Slot" + slotNum + "Description");
        }
    }

    public void DisplayOverrideAlert() 
    {
        alertUI.SetActive(true);

        _yesButton.onClick.AddListener(() => 
        { 
            SaveGameConfirmed(); 
            alertUI.SetActive(false); 
        });


        _noButton.onClick.AddListener(() => 
        { 
            alertUI.SetActive(false);
        });


    
    }

    private void SaveGameConfirmed()
    {
        SaveManager.Instance.SaveGame(slotNum);

        DateTime dt = DateTime.Now;
        string time = dt.ToString("dd-MM-yyyy HH:mm");

        string description = "Saved Game" + slotNum + " | " + time;
        _buttonText.text = description;
        PlayerPrefs.SetString("Slot" + slotNum + "Description", description);
        SaveManager.Instance.DeselectButton();
    }
}
