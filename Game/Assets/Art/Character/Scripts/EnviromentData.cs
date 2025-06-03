using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnviromentData 
{
    public List<string> pickedupItems;
    public EnviromentData(List<string> _pickedupItemsems) 
    {
        pickedupItems = _pickedupItemsems;
    }
}
