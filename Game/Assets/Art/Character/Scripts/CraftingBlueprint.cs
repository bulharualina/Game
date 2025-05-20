using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingBlueprint
{
    public string itemName;

    public string Req1;
    public string Req2;

    public int Req1amount;
    public int Req2amount;

    public int numOfReq;

    public CraftingBlueprint(string name, int reqNum, string r1, int r1num, string r2, int r2num) 
    {
        itemName = name;

        numOfReq = reqNum;

        Req1amount = r1num; 
        Req2amount = r2num;

        Req1 = r1;
        Req2 = r2;
    }
}
