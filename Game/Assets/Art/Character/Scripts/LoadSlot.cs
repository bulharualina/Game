using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : MonoBehaviour
{
    private Button _slotButton;
    private TextMeshProUGUI _buttonText;

    public int slotNum;

    private void Awake()
    {
        _slotButton = GetComponent<Button>();
        _buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    public void Start()
    {
        _slotButton.onClick.AddListener(() =>
        {
            if (!SaveManager.Instance.IsSlotEmpty(slotNum))
            {
                SaveManager.Instance.StartLoadedGame(slotNum);
                SaveManager.Instance.DeselectButton();
            }
            else
            {
                //displayWarning

            }

        });
    }
    private void Update()
    {
        if (SaveManager.Instance.IsSlotEmpty(slotNum))
        {
            _buttonText.text = "";
        }
        else
        {
            _buttonText.text = PlayerPrefs.GetString("Slot" + slotNum + "Description");
        }
    }

}

  
