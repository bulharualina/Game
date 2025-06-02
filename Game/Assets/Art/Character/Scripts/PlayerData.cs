using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] playerSurvivStats; // Health - 0,Calories - 1,Hydration - 2

    public float[] playerRotationAndPosition; //rot xyz pos xyz


    public PlayerData(float[] _playerSurvivStats, float[] _playerRotAndPos) 
    {
        playerSurvivStats = _playerSurvivStats;
        playerRotationAndPosition = _playerRotAndPos;
    }

}
