using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StackableCard : Card
{
    public Vector3 CardOffset { get; set; }

    public override Transform MainTransform => base.MainTransform.parent;

    public StackGroup Group;

    protected override void SetStart()
    {
        base.SetStart();
        Group = GetComponentInParent<StackGroup>();
        Group.AddCard(this);
        m_GameControl.OnMultiTapPressed += RemoveFromStack;
        OnStopDragging += CombineStackableCard;
    }

    protected virtual void RemoveFromStack(Card card)
    {
        if (card != this) return;
        if (card is not StackableCard) return;
        if (Group.TotalStack <= 1) return;

        Vector3 spawnPosition = GameUtility.GetRandomScatterPosition(Group.transform.position,GameConfig.instance.MaxSpawnRadius);
        StackGroup newGroup = GameCardSpawner.instance.GetEmptyStackGroup(spawnPosition, MainTransform.parent, CardDataSO.CardName);

        Group.RemoveCard(this);
        newGroup.AddCard(this);
    }

    public void RemoveFromStack()
    {
        RemoveFromStack(this);
    }

    protected virtual void CombineStackableCard()
    {
        if (!HasTriggeredCard) return;

        StackGroup acceptedGroup = null;
        foreach (Card card in m_TriggeredCard)
        {
            if (card is not StackableCard) continue;
            if (!card.CanCombineCard) continue;

            acceptedGroup = card.GetComponentInParent<StackGroup>();
            break;
        }

        if (acceptedGroup == null) return;
        m_TriggeredCard.Clear();

        MergeGroup(acceptedGroup);
    }

    public void MergeGroup(StackGroup otherGroup)
    {
        if (otherGroup == null) return;
        if (otherGroup == Group) return;

        if (otherGroup.TotalStack < Group.TotalStack)
        {
            MoveStackGroup(otherGroup, Group);
        }
        else
        {
            MoveStackGroup(Group, otherGroup);
        }
    }

    void MoveStackGroup(StackGroup fromGroup, StackGroup toGroup)
    {
        foreach (StackableCard card in fromGroup.Cards)
        {
            toGroup.AddCard(card);
        }

        fromGroup.Cards.Clear();
        Destroy(fromGroup.gameObject);
    }

    public override void ChangeCardPosition(Vector3 position)
    {
        MainTransform.position = Group.GetPositionInBounds(position);
        Group.OnStackableCardMove?.Invoke();
    }

    public void ChangeSortingLayerOnMove()
    {
        ChangeSortingLayer(true);
    }

    protected override void SetEndCardDragging(Card card)
    {
        base.SetEndCardDragging(card);
        ChangeSortingLayer(false);
    }

    public override void DestroyCard()
    {
        OnCardDestroyed?.Invoke(this);

        if (Group.Cards.Count <= 1)
        {
            base.DestroyCard();
        }
        else
        {
            Group.RemoveCard(this);
            Destroy(gameObject);
        }
    }
}
