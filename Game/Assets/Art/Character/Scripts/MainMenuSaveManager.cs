using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSaveManager : MonoBehaviour
{

    public static MainMenuSaveManager Instance { get; set; }
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

    }
