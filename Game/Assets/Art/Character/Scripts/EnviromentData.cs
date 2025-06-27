using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnviromentData 
{
    public List<string> pickedupItems;
 
    public List<BuildingData> buildingData;
    public EnviromentData(List<string> _pickedupItemsems, List<BuildingData> _buildingData) 
    {
        pickedupItems = _pickedupItemsems;
        buildingData = _buildingData;
    }
}


[System.Serializable]
public class BuildingData
{
    public string name;
    public SerializableVector3 position;
    public SerializableQuaternion rotation; 

   
}