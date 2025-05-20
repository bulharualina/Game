using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [SerializeField] private string _itemName;
  


    public string ItemName
    {
        get { return _itemName; }
    }

  
}
