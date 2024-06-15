using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    [Header("General")]
    public float MaxSpawnRadius = 2f;
    [Header("Coin Farm")]
    public float ProduceCoinTime = 1f;
    public float PreferedCardTime = 120f;
    [Header("Card Destroyer")]
    public float DestroyCardDelay = 2f;

    public static GameConfig instance;

    private void Awake()
    {
        instance = this;
    }
}
