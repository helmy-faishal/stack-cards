using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kelas dasar dari kartu yang dapat ditumpuk
public class StackableCard : Card
{
    // Stack utama dari tumpukan kartu
    [HideInInspector]
    public StackGroup group;

    protected override void SetStart()
    {
        base.SetStart();
        group = GetComponentInParent<StackGroup>();
        OnStopDragging += CombineStackableCard;
    }

    protected virtual void CombineStackableCard()
    {
        if (selectedCard == null) return;

        StackGroup acceptedGroup = selectedCard.GetComponentInParent<StackGroup>();

        if (acceptedGroup == null) return;

        if (group.TotalStack > acceptedGroup.TotalStack)
        {
            MoveStackGroup(acceptedGroup, group);
        }
        else
        {
            MoveStackGroup(group, acceptedGroup);
        }
    }

    void MoveStackGroup(StackGroup fromGroup, StackGroup toGroup)
    {
        foreach (StackableCard card in fromGroup.cards)
        {
            card.group = toGroup;
            card.transform.position = toGroup.transform.position;
            card.transform.parent = toGroup.transform;
            toGroup.AddCard(card);
        }

        fromGroup.cards.Clear();
        Destroy(fromGroup.gameObject);
    }

    public override void DestroyCard()
    {
        OnCardDestroyed?.Invoke(this);

        if (group.cards.Count <= 1)
        {
            base.DestroyCard();
        }
        else
        {
            group.RemoveCard(this);
            Destroy(gameObject);
        }
    }
}
