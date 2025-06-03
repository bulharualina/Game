using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] playerSurvivStats; // Health - 0,Calories - 1,Hydration - 2

    public float[] playerRotationAndPosition; //rot xyz pos xyz

    public string[] inventoryContent;

    public string[] guickSlotsContent;



    public PlayerData(float[] _playerSurvivStats, float[] _playerRotAndPos, string[] _inventoryContent, string[] _guickSlotsContent)
    {
        playerSurvivStats = _playerSurvivStats;
        playerRotationAndPosition = _playerRotAndPos;
        inventoryContent = _inventoryContent;
        guickSlotsContent = _guickSlotsContent;
    }

}
