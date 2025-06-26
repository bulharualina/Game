using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnviromentData 
{
    public List<string> pickedupItems;
    public List<TreeData> treeData;
    public List<string> animalsData;
    public List<BuildingData> buildingData;
    public EnviromentData(List<string> _pickedupItemsems, List<TreeData> _treeData, List<string> _animalsData, List<BuildingData> _buildingData) 
    {
        pickedupItems = _pickedupItemsems;
        treeData = _treeData;
        animalsData = _animalsData;
        buildingData = _buildingData;
    }
}

[System.Serializable]
public class TreeData 
{
    public string name;
    public SerializableVector3 position;
    public SerializableVector3 rotation;


}

[System.Serializable]
public class BuildingData
{
    public string name;
    public SerializableVector3 position;
    public SerializableQuaternion rotation; 

   
}