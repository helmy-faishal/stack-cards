using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopCard : Card
{
    [SerializeField] bool _isPrioritzeHighestPrize = true;
    [System.Serializable]
    class ShopItem
    {
        [Min(1)] public int price = 1;
        public PackCardDataSO pack;
    }
    [SerializeField] ShopItem[] _shopItems;

    Dictionary<int, List<ShopItem>> _shopItemMap;
    int[] _prices;
    int _minPrice;

    StackableCard _selectedCard;

    bool _isProcessBuy;

    protected override void SetStart()
    {
        base.SetStart();
        ShopItemMapping();
        m_GameManager.MinShopPrice = _minPrice;
    }

    void ShopItemMapping()
    {
        _shopItemMap = new Dictionary<int, List<ShopItem>>();

        foreach (ShopItem item in _shopItems)
        {
            if (_shopItemMap.ContainsKey(item.price))
            {
                _shopItemMap[item.price].Add(item);
            }
            else
            {
                _shopItemMap[item.price] = new List<ShopItem>() { item };
            }
        }

        _prices = new int[_shopItemMap.Keys.Count];
        _shopItemMap.Keys.CopyTo(_prices, 0);

        Array.Sort(_prices);
        _minPrice = _prices[0];
        if (_isPrioritzeHighestPrize)
        {
            Array.Reverse(_prices);
        }
    }

    protected override bool CanTriggered => true;

    protected override void SetEndCardDragging(Card card)
    {
        base.SetEndCardDragging(card);

        ProcessTrigger();
    }

    void ProcessTrigger()
    {
        if (!HasTriggeredCard) return;

        List<Card> processedCard = new List<Card>();
        StackGroup selectedGroup = null;
        foreach (Card card in m_TriggeredCard)
        {
            if (card == null) continue;
            if (card is not StackableCard stackableCard) continue;
            if (selectedGroup == null)
            {
                selectedGroup = stackableCard.Group;
                processedCard.Add(card);
                continue;
            }

            stackableCard.MergeGroup(selectedGroup);
            selectedGroup = stackableCard.Group;
        }
        m_TriggeredCard = processedCard;
        ProcessBuy();
    }

    void ProcessBuy()
    {
        if(!HasTriggeredCard) return;
        if (_isProcessBuy) return;

        _isProcessBuy = true;

        _selectedCard = (StackableCard)m_TriggeredCard[0];

        int budget = _selectedCard.Group.TotalStack;
        List<ShopItem> itemPurchase = new List<ShopItem>();

        foreach (int price in _prices)
        {
            int total = budget / price;
            budget %= price;

            for (int i = 0; i < total; i++)
            {
                List<ShopItem> item = _shopItemMap[price];

                if (item.Count > 1)
                {
                    itemPurchase.Add(item[Random.Range(0, item.Count)]);
                }
                else
                {
                    itemPurchase.Add(item[0]);
                }
            }
        }

        //m_GameManager.TotalPackCard += itemPurchase.Count;
        GenerateItem(itemPurchase);
        DecreaseStack(budget);
        _isProcessBuy = false;
    }

    void GenerateItem(List<ShopItem> itemPurchase)
    {
        foreach (var item in itemPurchase)
        {
            Vector3 position = GameUtility.GetRandomScatterPosition(transform.position, GameConfig.instance.MaxSpawnRadius);
            PackCard packCard = GameFactory.instance.GetPackCard(position, MainTransform.parent, item.pack.CardName);
            packCard.CardDataSO = item.pack;
            m_GameManager.TotalPackCard += 1;
        }
    }

    void DecreaseStack(int remainBudget)
    {
        int totalDecrease = _selectedCard.Group.TotalStack - remainBudget;
        _selectedCard.Group.DecreaseCard(totalDecrease);

        _selectedCard = null;
    }
}
