using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardConfig : MonoBehaviour
{
    [SerializeField] CardDataSO[] _cardsData;

    public static CardConfig instance;

    private void Awake()
    {
        instance = this;
    }

    public CardDataSO GetCardData(CardType cardType)
    {
        CardDataSO cardData = Array.Find(_cardsData, data => data.Type == cardType);

        if (cardData == null)
        {
            Debug.LogError($"Type \"{cardType}\" not found!");
        }

        return cardData;
    }
}
