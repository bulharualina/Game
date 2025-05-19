using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [SerializeField] private string _itemName;
    [SerializeField] private string _uiPrefabName;


    public string ItemName
    {
        get { return _itemName; }
    }

    public string UIPrefabName
    {
        get { return _uiPrefabName; }
    }
}
