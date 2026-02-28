using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SymbolDataSO", menuName = "Scriptable Objects/SymbolDataSO")]
public class SymbolDataSO : ScriptableObject
{
    [SerializeField] private List<SymbolData> symbolDatas;

    public SymbolData GetSymbolData(int id)
    {
        if (id >= 0 && id < symbolDatas.Count)
        {
            return symbolDatas[id];
        }
        return null;
    }

    public int GetSymbolCount()
    {
        return symbolDatas.Count;
    }
}

[Serializable]
public class SymbolData
{
    public Sprite symbolSprite;
    public int payoutValue; // Base payout for 3 of a kind
}