using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Script yang mengatur coin yang dihasilkan oleh farm
public class GameFarm : MonoBehaviour
{
    [Tooltip("UI Farm untuk menampilkan jenis kartu preferensi farm")]
    [SerializeField] TMP_Text _preferTypeText;

    public Action OnCoinIncrease;
    public Action OnCoinDecrease;

    public int TotalCoin { get; private set; }

    private void Start()
    {
        OnCoinIncrease += () =>
        {
            TotalCoin += 1;
        };

        OnCoinDecrease += () =>
        {
            TotalCoin -= 1;
        };
    }

    private void OnDestroy()
    {
        OnCoinIncrease -= () =>
        {
            TotalCoin += 1;
        };
        OnCoinDecrease -= () =>
        {
            TotalCoin -= 1;
        };
    }

    public void SetPreferTypeCard(string text)
    {
        _preferTypeText.text = text;
    }
}
