using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StackGroup : MonoBehaviour
{
    [Tooltip("Renderer yang menampilkan kartu")]
    [SerializeField] Renderer render;
    [Tooltip("Renderer yang menampilkan menampilkan border")]
    [SerializeField] Renderer border;
    [Tooltip("Canvas UI milik kartu")]
    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text groupText;

    public List<StackableCard> cards = new List<StackableCard>();
    public int TotalStack => cards.Count;

    Action OnStackChange;

    private void Start()
    {
        StackableCard card = GetComponentInChildren<StackableCard>();
        card.group = this;
        AddCard(card);

        if (render == null)
        {
            render = card.render;
        }

        if (border == null)
        {
            border = card.borderEffect;
        }

        if (canvas == null)
        {
            canvas = card.canvas;
        }
    }

    public void AddCard(StackableCard card)
    {
        card.render = render;
        card.borderEffect = border;
        card.canvas = canvas;
        cards.Add(card);
        OnStackChange += () =>
        {
            card.SetCardTrigger(false);
            ActiveFirstCardTrigger();
        };
        ShowStackChange();
    }

    public void RemoveCard(StackableCard card)
    {
        cards.Remove(card);
        OnStackChange -= () =>
        {
            card.SetCardTrigger(false);
            ActiveFirstCardTrigger();
        };
        ShowStackChange();
    }

    public void DecreaseCard(int value)
    {
        if (value <= 0) return;

        value = Mathf.Min(value, TotalStack);

        for (int i = 0; i < value; i++)
        {
            cards[0].DestroyCard();
        }
    }

    void ActiveFirstCardTrigger()
    {
        if (cards.Count > 0)
        {
            cards[0].SetCardTrigger(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ShowStackChange()
    {
        groupText.text = cards.Count.ToString();
        OnStackChange?.Invoke();
    }
}
