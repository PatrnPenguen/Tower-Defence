using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [SerializeField] private int startCurrency;
    [NonSerialized] public int currency;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        currency = startCurrency;
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }

        Debug.Log("Not enough currency");
        return false;
    }
}