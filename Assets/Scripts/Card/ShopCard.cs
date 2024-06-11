using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopCard : Card
{
    [Tooltip("Apakah shop akan memprioritaskan barang dengan harga tertinggi?")]
    [SerializeField] bool isPrioritzeHighestPrize = true;
    [System.Serializable]
    class ShopItem
    {
        [Min(1)] public int price = 1;
        public GameObject item;
    }
    [Tooltip("Item-item yang dapat ditukar dengan koin")]
    [SerializeField] ShopItem[] shopItems;

    Dictionary<int, List<ShopItem>> shopItemMap;
    int[] prices;
    int minPrice;

    StackGroup targetGroup;
    bool HasTarget => targetGroup != null && selectedCard != null;

    bool isProcessBuy;

    protected override void SetStart()
    {
        base.SetStart();
        ShopItemMapping();
        gameManager.MinShopPrice = minPrice;
    }

    void ShopItemMapping()
    {
        shopItemMap = new Dictionary<int, List<ShopItem>>();

        foreach (ShopItem item in shopItems)
        {
            if (shopItemMap.ContainsKey(item.price))
            {
                shopItemMap[item.price].Add(item);
            }
            else
            {
                shopItemMap[item.price] = new List<ShopItem>() { item };
            }
        }

        prices = new int[shopItemMap.Keys.Count];
        shopItemMap.Keys.CopyTo(prices, 0);
        
        Array.Sort(prices);
        minPrice = prices[0];
        if (isPrioritzeHighestPrize)
        {
            Array.Reverse(prices);
        }
    }

    protected override void HandleTriggerEnter(Collider2D collision)
    {
        base.HandleTriggerEnter(collision);

        SetTargetGroup();
    }

    protected override void HandleTriggerStay(Collider2D collision)
    {
        base.HandleTriggerStay(collision);

        SetTargetGroup();
    }

    void SetTargetGroup()
    {
        if (selectedCard is not StackableCard) return;

        StackableCard card = (StackableCard)selectedCard;
        if (targetGroup == null)
        {
            if (card.group.TotalStack < minPrice) return;

            targetGroup = card.group;
        }

        if (card.group.TotalStack > targetGroup.TotalStack)
        {
            targetGroup = card.group;
        }
    }

    protected override void HandleTriggerExit(Collider2D collision)
    {
        base.HandleTriggerExit(collision);
        if (selectedCard == null)
        {
            targetGroup = null;
        }
    }

    private void Update()
    {
        if (!HasTarget) return;
        if (isProcessBuy) return;

        BuyItem();
    }

    void BuyItem()
    {
        if (!HasTarget) return;
        if (selectedCard.IsDragging) return;

        isProcessBuy = true;

        int budget = targetGroup.TotalStack;
        List<ShopItem> itemPurchase = new List<ShopItem>();

        foreach (int price in prices)
        {
            int total = budget / price;
            budget %= price;

            for (int i = 0; i < total; i++)
            {
                List<ShopItem> item = shopItemMap[price];

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

        gameManager.TotalPackCard += itemPurchase.Count;
        GenerateItem(itemPurchase);
        DecreaseStack(budget);
        isProcessBuy = false;
    }

    void GenerateItem(List<ShopItem> itemPurchase)
    {
        foreach (var item in itemPurchase)
        {
            Vector3 position = GameUtility.GetRandomScatterPosition(transform.position, 2f);
            Instantiate(item.item, position, Quaternion.identity);
        }
    }

    void DecreaseStack(int remainBudget)
    {
        int totalDecrease = targetGroup.TotalStack - remainBudget;
        targetGroup.DecreaseCard(totalDecrease);

        targetGroup = null;
    }
}
