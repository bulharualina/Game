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



    #region General


    #region Saving
    public void SaveGame() 
    { 
        AllGameData allGameData = new AllGameData();

        allGameData.playerData = GetPlayerData();

        SaveGameDataToBinaryFile(allGameData);


    }

    private PlayerData GetPlayerData()
    {
        float[] playerSurvivStats = new float[3];
        if (PlayerSurvivalStats.Instance != null)
        {
            playerSurvivStats[0] = PlayerSurvivalStats.Instance.currentHealth;
            playerSurvivStats[1] = PlayerSurvivalStats.Instance.currentCalories;
            playerSurvivStats[2] = PlayerSurvivalStats.Instance.currentHydrationPercent;
        }
        else
        {
            Debug.LogError("PlayerSurvivalStats.Instance is null when trying to get player survival stats for saving.");
            playerSurvivStats[0] = 0f;
            playerSurvivStats[1] = 0f;
            playerSurvivStats[2] = 0f;
        }

        float[] playerRotationAndPosition = new float[6];
        if (PlayerSurvivalStats.Instance != null && PlayerSurvivalStats.Instance.player != null)
        {
            playerRotationAndPosition[0] = PlayerSurvivalStats.Instance.player.transform.rotation.x;
            playerRotationAndPosition[1] = PlayerSurvivalStats.Instance.player.transform.rotation.y;
            playerRotationAndPosition[2] = PlayerSurvivalStats.Instance.player.transform.rotation.z;

            playerRotationAndPosition[3] = PlayerSurvivalStats.Instance.player.transform.position.x;
            playerRotationAndPosition[4] = PlayerSurvivalStats.Instance.player.transform.position.y;
            playerRotationAndPosition[5] = PlayerSurvivalStats.Instance.player.transform.position.z;
        }
        else
        {
            Debug.LogError("PlayerSurvivalStats.Instance or PlayerSurvivalStats.Instance.player is null when trying to get player rotation/position for saving.");
            
            for (int i = 0; i < 6; i++) playerRotationAndPosition[i] = 0f;
        }
        return new PlayerData(playerSurvivStats, playerRotationAndPosition);
    }

 
    #endregion


    #region Loading


    public void LoadGame() 
    {
        AllGameData allGameData = LoadGameDataFromBinaryFile();

        if (allGameData != null)
        {
            //Player Data
            SetPlayerData(allGameData.playerData);
        }
        else
        {
            Debug.Log("No save game found. Starting new game or handling accordingly.");
           
        }
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
               
            PlayerSurvivalStats.Instance.setHealth(playerData.playerSurvivStats[0]);
            PlayerSurvivalStats.Instance.setCalories(playerData.playerSurvivStats[1]);
            PlayerSurvivalStats.Instance.setHydration(playerData.playerSurvivStats[2]);
               

                
            if (PlayerSurvivalStats.Instance.player != null)
            {
                Vector3 loadedRotation;
                loadedRotation.x = playerData.playerRotationAndPosition[0];
                loadedRotation.y = playerData.playerRotationAndPosition[1];
                loadedRotation.z = playerData.playerRotationAndPosition[2];
                PlayerSurvivalStats.Instance.player.transform.rotation = Quaternion.Euler(loadedRotation);

                Vector3 loadedPosition;
                loadedPosition.x = playerData.playerRotationAndPosition[3];
                loadedPosition.y = playerData.playerRotationAndPosition[4];
                loadedPosition.z = playerData.playerRotationAndPosition[5];
                PlayerSurvivalStats.Instance.player.transform.position = loadedPosition;

               

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

    public void StartLoadedGame() 
    {
        SceneManager.LoadScene("GameScene");

        StartCoroutine(DelayadLoading());
    }

    private IEnumerator DelayadLoading()
    {
        yield return new WaitForSeconds(1f);

        LoadGame();
    }

    #endregion


    #endregion

    #region Save To Binary

    public void SaveGameDataToBinaryFile(AllGameData gameData) 
    { 
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/saveGame.bin";
        FileStream fileStream = new FileStream(path, FileMode.Create);

        binaryFormatter.Serialize(fileStream, gameData);
        fileStream.Close();

        print("Data saved to "+ Application.persistentDataPath + "/saveGame.bin");
    
    }

    public AllGameData LoadGameDataFromBinaryFile() 
    {
        string path = Application.persistentDataPath + "/saveGame.bin";

        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path,FileMode.Open);

            AllGameData data = binaryFormatter.Deserialize(fileStream) as AllGameData;
            fileStream.Close();

            print("Data loaded from " + Application.persistentDataPath + "/saveGame.bin");
            return data;
        }
        else { return null; }
    
    }
    #endregion

    #region Settings

    #region Volume
    [System.Serializable]
    public class VolumeSettings 
    {
        public float musicVolume;
        public float effectsVolume;
        public float masterVolume;
    }

    public void SaveVolumeSettings(float music, float effects, float master)
    {
        VolumeSettings volumeSettings = new VolumeSettings()
        {
            musicVolume = music,
            effectsVolume = effects,
            masterVolume = master
        };

        PlayerPrefs.SetString("Volume",JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();
    }

    public VolumeSettings LoadVolumeSettings() 
    {
        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
    }
    #endregion

    #endregion


}
