using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StackGroup : MonoBehaviour
{
    [SerializeField] float _spaceBetweenStack = 0.1f;

    public List<StackableCard> Cards = new List<StackableCard>();
    public int TotalStack => Cards.Count;

    public Action OnStackableCardMove;
    Action OnStackChange;

    private void Start()
    {
        OnStackChange += CheckStackEmpty;
    }

    void CheckStackEmpty()
    {
        if (Cards.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    public void AddCard(StackableCard card)
    {
        Cards.Add(card);
        card.Group = this;
        card.transform.parent = this.transform;

        OnStackableCardMove += card.ChangeSortingLayerOnMove;
        OnStackChange += StackUpdate;

        OnStackChange?.Invoke();
    }

    public Vector3 GetPositionInBounds(Vector3 position)
    {
        Vector3 newPosition = GameUtility.ClampPointInBounds(GameManager.PlayableBounds, position);

        if (Cards.Count == 1)
        {
            return newPosition;
        }

        StackableCard firstCard = Cards.FirstOrDefault<StackableCard>();
        StackableCard lastCard = Cards.LastOrDefault<StackableCard>();

        Vector3 firstPosition = newPosition + firstCard.CardOffset;
        Vector3 lastPosition = newPosition + lastCard.CardOffset;

        bool isBottomNonPlayable = firstPosition.y <= GameManager.PlayableBounds.min.y;
        bool isTopNonPlayable = lastPosition.y >= GameManager.PlayableBounds.max.y;

        if (isBottomNonPlayable || isTopNonPlayable)
        {
            newPosition.y = transform.position.y;
        }

        return newPosition;
    }

    public void RemoveCard(StackableCard card)
    {
        Cards.Remove(card);
        OnStackableCardMove -= card.ChangeSortingLayerOnMove;
        OnStackChange -= StackUpdate;

        OnStackChange?.Invoke();
    }

    void StackUpdate()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            StackableCard card = Cards[i];

            Vector3 offset = i * _spaceBetweenStack * Vector3.up;
            card.transform.localPosition = offset;
            card.CardOffset = offset;
            card.SetOrderInLayer(-i);
        }
    }

    public void DecreaseCard(int value)
    {
        if (value <= 0) return;

        value = Mathf.Min(value, TotalStack);

        for (int i = 0; i < value; i++)
        {
            Cards[0].DestroyCard();
        }
    }

}
