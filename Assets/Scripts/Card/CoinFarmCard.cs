using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinFarmCard : Card
{
    float _currentProduceTime;
    float _currentPreferedTime;
    CardType _preferedType;

    Card _selectedCard;
    bool IsProducing => _selectedCard != null;

    float ProduceTimeMultiplier => IsProducing && _selectedCard.CardType == _preferedType ? 0.5f : 1f;
    [SerializeField] Image _fillImage;

    protected override void SetStart()
    {
        base.SetStart();

        SetFillImage(0f);
        GetPreferedCardType();
    }

    private void Update()
    {
        SetPreferedCard();
        ProduceCoin();
    }

    void SetPreferedCard()
    {
        _currentPreferedTime += Time.deltaTime;

        if (_currentPreferedTime >= GameConfig.instance.PreferedCardTime)
        {
            GetPreferedCardType();
            _currentPreferedTime = 0f;
        }
    }

    void GetPreferedCardType()
    {
        _preferedType = m_AcceptedCard[Random.Range(0, m_AcceptedCard.Length)];
        m_GameManager.Farm.SetPreferTypeCard($"Coin Farm prefer {_preferedType} card");
    }

    void ProduceCoin()
    {
        if (!IsProducing)
        {
            if (!CanCombineCard)
            {
                CanCombineCard = true;
                SetOrderInLayer(0);
            }
            return;
        }
        _currentProduceTime += Time.deltaTime;

        if (_currentProduceTime >= GameConfig.instance.ProduceCoinTime * ProduceTimeMultiplier)
        {
            _currentProduceTime = 0f;
            GenerateCoin();
        }

        SetFillImage(_currentProduceTime);
    }

    void SetFillImage(float value)
    {
        _fillImage.fillAmount = value;
    }

    void GenerateCoin()
    {
        Vector3 spawnPosition = GameUtility.GetRandomScatterPosition(transform.position, GameConfig.instance.MaxSpawnRadius);

        CardDataSO cardData = CardConfig.instance.GetCardData(CardType.Coin);

        StackableCard card = GameFactory.instance.GetStackableCard(spawnPosition, MainTransform.parent, cardData.CardName);
        card.Render.sortingLayerID = SortingLayer.NameToID("Coin");
        card.CardDataSO = cardData;
        m_GameManager.Farm.OnCoinIncrease?.Invoke();
        card.OnCardDestroyed += (_) =>
        {
            m_GameManager.Farm.OnCoinDecrease?.Invoke();
        };
    }

    protected override bool CanTriggered => true;

    protected override void SetEndCardDragging(Card card)
    {
        base.SetEndCardDragging(card);

        CheckCardTriggered();
    }

    void CheckCardTriggered()
    {
        if (m_TriggeredCard.Count <= 0)
        {
            if (_selectedCard != null)
            {
                _selectedCard.CanCombineCard = true;
                this.CanCombineCard = true;
            }
            _selectedCard = null;
            SetOrderInLayer(0);
            return;
        }

        SetOrderInLayer(-1);
        foreach (Card card in m_TriggeredCard)
        {
            if (_selectedCard == card)
            {
                _selectedCard.ChangeCardPosition(transform.position);
                break;
            }

            if (card is StackableCard stackableCard)
            {
                stackableCard.RemoveFromStack();
                _selectedCard = stackableCard;
                stackableCard.Group.transform.position = transform.position;
            }
            else
            {
                _selectedCard = card;
                card.transform.position = transform.position;
            }

            m_TriggeredCard.Clear();
            m_TriggeredCard.Add(card);
            _selectedCard.CanCombineCard = false;
            this.CanCombineCard = false;
            break;
        }
    }
}
