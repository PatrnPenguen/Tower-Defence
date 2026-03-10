using UnityEngine;
using System;

[Serializable]
public class Tower
{
    public string towerName;
    public int cost;
    public GameObject prefab;

    public Tower(string _name, int _cost, GameObject _prefab)
    {
        towerName = _name;
        cost = _cost;
        prefab = _prefab;
    }
}
