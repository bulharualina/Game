using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{

    public static SaveManager Instance { get; set; }
    public Canvas loadingScreen;
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

        DontDestroyOnLoad(gameObject);
    }


    public bool isLoading;

 


    #region General


    #region Saving
    public void SaveGame(int slotNUm) 
    { 
        AllGameData allGameData = new AllGameData();

        allGameData.playerData = GetPlayerData();
        allGameData.enviromentData = GetEnviromentData();

        SaveGameDataToBinaryFile(allGameData,slotNUm);


    }

    private EnviromentData GetEnviromentData()
    {
        List<string> pickedupItems = InventorySystem.Instance.pickedupItems;

        return new EnviromentData(pickedupItems);
    }


    private PlayerData GetPlayerData()
    {
        float[] playerSurvivStats = new float[3];
        if (PlayerSurvivalStats.Instance != null)
        {
            playerSurvivStats[0] = PlayerSurvivalStats.Instance.currentCalories;
            playerSurvivStats[1] = PlayerSurvivalStats.Instance.currentHydrationPercent;
            
        }
        else
        {
            Debug.LogError("PlayerSurvivalStats.Instance is null when trying to get player survival stats for saving.");
            playerSurvivStats[0] = 0f;
            playerSurvivStats[1] = 0f;
           
        }

        float[] playerRotationAndPosition = new float[6];
        if (PlayerSurvivalStats.Instance != null && PlayerSurvivalStats.Instance.player != null)
        {
            Vector3 eulerAngles = PlayerSurvivalStats.Instance.player.transform.eulerAngles;
            playerRotationAndPosition[0] = eulerAngles.x;
            playerRotationAndPosition[1] = eulerAngles.y;
            playerRotationAndPosition[2] = eulerAngles.z;

            playerRotationAndPosition[3] = PlayerSurvivalStats.Instance.player.transform.position.x;
            playerRotationAndPosition[4] = PlayerSurvivalStats.Instance.player.transform.position.y;
            playerRotationAndPosition[5] = PlayerSurvivalStats.Instance.player.transform.position.z;
        }
        else
        {
            Debug.LogError("PlayerSurvivalStats.Instance or PlayerSurvivalStats.Instance.player is null when trying to get player rotation/position for saving.");
            
            for (int i = 0; i < 6; i++) playerRotationAndPosition[i] = 0f;
        }

        string[] inventoryContent = InventorySystem.Instance.itemList.ToArray();
        string[] guickSlotsContent = GetQuickSlotsContent();
        return new PlayerData(playerSurvivStats, playerRotationAndPosition, inventoryContent, guickSlotsContent);
    }

    private string[] GetQuickSlotsContent()
    {
        List<string> qSlots = new List<string>();

        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount != 0)
            { 
                string name = slot.transform.GetChild(0).name;
                string cl = "(Clone)";
                string cleanName = name.Replace(cl, "");
                qSlots.Add(cleanName);
            }
        }

        return qSlots.ToArray();
    }


    #endregion


    #region Loading


    public void LoadGame(int slotNum) 
    {
        AllGameData allGameData = LoadGameDataFromBinaryFile(slotNum);

        if (allGameData != null)
        {
            //Player Data
            SetPlayerData(allGameData.playerData);

            //Enviroment Data
            SetEnviromentData(allGameData.enviromentData);


            isLoading = false;
            LoadingScreenOFF();
        }
        else
        {
            Debug.Log("No save game found. Starting new game or handling accordingly.");
           
        }
    }

    private void SetEnviromentData(EnviromentData enviromentData)
    {
        foreach (Transform itemType in EnviromentManager.Instance.allItems.transform) 
        {
            foreach (Transform item in itemType.transform)
            {
                if (enviromentData.pickedupItems.Contains(item.name)) 
                {
                    Destroy(item.gameObject);
                }
            }
        
        }

        InventorySystem.Instance.pickedupItems = enviromentData.pickedupItems;
    }

    private void SetPlayerData(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("SetPlayerData received null data. Cannot apply.");
            return;
        }

        if (PlayerSurvivalStats.Instance != null)
        {
               
            PlayerSurvivalStats.Instance.setCalories(playerData.playerSurvivStats[0]);
            PlayerSurvivalStats.Instance.setHydration(playerData.playerSurvivStats[1]);
               

                
            if (PlayerSurvivalStats.Instance.player != null)
            {
                Vector3 loadedRotationEuler;
                loadedRotationEuler.x = playerData.playerRotationAndPosition[0];
                loadedRotationEuler.y = playerData.playerRotationAndPosition[1];
                loadedRotationEuler.z = playerData.playerRotationAndPosition[2];
                
                PlayerSurvivalStats.Instance.player.transform.rotation = Quaternion.Euler(loadedRotationEuler);

                Vector3 loadedPosition;
                loadedPosition.x = playerData.playerRotationAndPosition[3];
                loadedPosition.y = playerData.playerRotationAndPosition[4];
                loadedPosition.z = playerData.playerRotationAndPosition[5];
                
                PlayerSurvivalStats.Instance.player.transform.position = loadedPosition;

                //inventory content
                foreach (string item in playerData.inventoryContent) 
                {
                    InventorySystem.Instance.AddToInventory(item);
                }

                // Quick slot content
                foreach (string item in playerData.guickSlotsContent)
                {
                    GameObject availableSlot = EquipSystem.Instance.FindNextEmptySlot();
                    var itemToAdd = Instantiate(Resources.Load<GameObject>(item));
                    itemToAdd.transform.SetParent(availableSlot.transform,false);
                }


            }
            else
            {
                    Debug.LogWarning("PlayerSurvivalStats.Instance.player is null. Cannot set player position/rotation.");
            }
        }
        else
        {
            Debug.LogError("PlayerSurvivalStats.Instance is null. Cannot set player data.");
        }
      
       
    }

    public void StartLoadedGame(int slotNum) 
    {
        LoadingScreenON();
        isLoading = true;
        SceneManager.LoadScene("GameScene");

        StartCoroutine(DelayadLoading(slotNum));
    }

    private IEnumerator DelayadLoading(int slotNum)
    {
        yield return new WaitForSeconds(1f);

        LoadGame(slotNum);
    }

    #endregion


    #endregion

    #region Save To Binary
    private string GetSaveFilePath(int slotNum) 
    {
        return Path.Combine(Application.persistentDataPath + Path.AltDirectorySeparatorChar, $"SaveGame{slotNum}.bin");
    }
    public void SaveGameDataToBinaryFile(AllGameData gameData,int slotNum) 
    { 
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        string fullFilePath = GetSaveFilePath(slotNum);

        FileStream fileStream = new FileStream(fullFilePath, FileMode.Create);

        binaryFormatter.Serialize(fileStream, gameData);
        fileStream.Close();

        print("Data saved to "+ fullFilePath);

    
    }

    public AllGameData LoadGameDataFromBinaryFile(int slotNum) 
    {
        string fullFilePath = GetSaveFilePath(slotNum);

        if (File.Exists(fullFilePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            
            FileStream fileStream = new FileStream(fullFilePath, FileMode.Open);

            AllGameData data = binaryFormatter.Deserialize(fileStream) as AllGameData;
            fileStream.Close();

            print("Data loaded from " + fullFilePath);
            return data;
        }
        else { return null; }
    
    }
    #endregion

  

    #region Utility

    public bool IsFileExists(int slotNum) 
    {
        string fullFilePath = GetSaveFilePath(slotNum);
        if (System.IO.File.Exists(fullFilePath)) {
            return true;
        } else { 
            return false;
        }
    }

    public bool IsSlotEmpty(int slotNum)
    {
        if (SaveManager.Instance.IsFileExists(slotNum))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }
    #endregion

    #region Loading Screen

    public void LoadingScreenON() 
    {
        loadingScreen.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadingScreenOFF()
    {
        loadingScreen.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
    #endregion
}




