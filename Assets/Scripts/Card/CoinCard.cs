using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCard : StackableCard
{
    protected override void SetStart()
    {
        base.SetStart();

        type = CardType.Coin;

        if (acceptedCard == null || acceptedCard.Length == 0)
        {
            CardType[] value = { CardType.Coin, CardType.Shop };
            acceptedCard = value;
            acceptedCardMask = GameUtility.GetCardTypeMask(acceptedCard);
        }

        gameManager.farm.OnCoinIncrease?.Invoke();
    }

    public override void DestroyCard()
    {
        base.DestroyCard();

        gameManager.farm.OnCoinDecrease?.Invoke();
    }


}
