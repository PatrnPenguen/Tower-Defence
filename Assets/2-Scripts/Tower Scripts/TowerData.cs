using UnityEngine;
using System;

[Serializable]
public class TowerData
{
    public string towerName;
    public GameObject prefab;

    public TowerData(string _name, GameObject _prefab)
    {
        towerName = _name;
        prefab = _prefab;
    }
}