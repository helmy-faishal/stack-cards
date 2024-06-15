using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackCard : Card
{
    [SerializeField] ParticleSystem _openEffect;
    [SerializeField] float _openDelay = 2f;
    bool _isOpeningPack;

    PackCardDataSO _cardData;

    protected override void SetStart()
    {
        if (CardDataSO is not PackCardDataSO)
        {
            Debug.LogError("Card data is not type of PackCardDataSO");
            return;
        }

        base.SetStart();
        _cardData = (PackCardDataSO)CardDataSO;

        if (m_GameControl != null)
        {
            m_GameControl.OnSingleTapPressed += OpenPack;
            m_GameControl.OnMultiTapPressed += OpenPack;
        }
    }

    protected override void SetOnDestroy()
    {
        base.SetOnDestroy();
        if (m_GameControl != null)
        {
            m_GameControl.OnSingleTapPressed -= OpenPack;
            m_GameControl.OnMultiTapPressed -= OpenPack;
        }
    }

    void OpenPack(Card card)
    {
        if (card != this) return;
        if (_isOpeningPack) return;
        if (_cardData.ContentOutput <= 0) return;

        StartCoroutine(OpenPackCoroutine());
    }

    IEnumerator OpenPackCoroutine()
    {
        _isOpeningPack = true;
        _openEffect.Play();

        yield return new WaitForSeconds(_openDelay);
        for (int i = 0; i < _cardData.ContentOutput; i++)
        {
            GenerateContent();
        }
        _openEffect.Stop();
        DestroyCard();
    }

    public override void DestroyCard()
    {
        base.DestroyCard();
        m_GameManager.TotalPackCard -= 1;
    }

    void GenerateContent()
    {
        Vector3 spawnPosition = GameUtility.GetRandomScatterPosition(transform.position, GameConfig.instance.MaxSpawnRadius);

        CardDataSO selectedCard = GetRandomContent();

        StackableCard card = GameFactory.instance.GetStackableCard(spawnPosition, MainTransform.parent, selectedCard.CardName);
        card.CardDataSO = selectedCard;
    }

    CardDataSO GetRandomContent()
    {
        return _cardData.PackContent[Random.Range(0, _cardData.PackContent.Length)];
    }
}
