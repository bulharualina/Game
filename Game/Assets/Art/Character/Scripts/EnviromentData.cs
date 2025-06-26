using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnviromentData 
{
    public List<string> pickedupItems;
    public List<TreeData> treeData;
    public EnviromentData(List<string> _pickedupItemsems, List<TreeData> _treeData) 
    {
        pickedupItems = _pickedupItemsems;
        treeData = _treeData;
    }
}

[System.Serializable]
public class TreeData 
{
    public string name;
    public SerializableVector3 position;
    public SerializableVector3 rotation;


}