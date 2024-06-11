using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockCard : StackableCard
{
    protected override void SetAwake()
    {
        base.SetAwake();
        isDestroyableBySystem = true;

        type = CardType.Rock;
    }

    protected override void SetStart()
    {
        base.SetStart();

        if (acceptedCard == null || acceptedCard.Length == 0)
        {
            CardType[] value = { CardType.Rock, CardType.CoinFarm };
            acceptedCard = value;
            acceptedCardMask = GameUtility.GetCardTypeMask(acceptedCard);
        }
    }
}
