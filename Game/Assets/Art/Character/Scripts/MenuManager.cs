using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }

    public GameObject menuCanvas;
    public GameObject uiCanvas;
    public GameObject menu;
    public GameObject saveMenu;
    public GameObject infoMenu;


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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isOpen)
        {
            uiCanvas.SetActive(false);
            menuCanvas.SetActive(true);
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            saveMenu.SetActive(false);
            infoMenu.SetActive(false);
            menu.SetActive(true);

            uiCanvas.SetActive(true);
            menuCanvas.SetActive(false);
            isOpen = false;
        }
    }
  
}
