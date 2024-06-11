using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperCard : StackableCard
{
    protected override void SetAwake()
    {
        base.SetAwake();
        isDestroyableBySystem = true;

        type = CardType.Paper;
    }

    protected override void SetStart()
    {
        base.SetStart();

        if (acceptedCard == null || acceptedCard.Length == 0)
        {
            CardType[] value = { CardType.Paper, CardType.CoinFarm };
            acceptedCard = value;
            acceptedCardMask = GameUtility.GetCardTypeMask(acceptedCard);
        }
    }
}
