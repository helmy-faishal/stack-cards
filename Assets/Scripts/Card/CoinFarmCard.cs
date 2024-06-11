using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinFarmCard : Card
{
    [Tooltip("Prefab dari Coin Card")]
    [SerializeField] GameObject coinCardPrefab;

    [Tooltip("Maks radius penyebaran coin yang dihasilkan")]
    [SerializeField][Min(0)] float maxSpawnRadius = 2f;

    [Tooltip("Waktu yang dibutuhkan untuk memproduksi koin")]
    [SerializeField] float produceCoinTime = 1f;
    float currentProduceTime;

    [Tooltip("Image UI untuk menampilkan progress")]
    [SerializeField] Image fillImage;

    [Tooltip("Waktu yang dibutuhkan untuk memperbarui preferensi kartu")]
    [SerializeField] float preferedCardTime = 120f;
    float currentPreferedTime;
    CardType preferedType;

    bool IsProducing => selectedCard != null;

    float ProduceTimeMultiplier => IsProducing && selectedCard.Type == preferedType ? 0.5f : 1f;

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
        currentPreferedTime += Time.deltaTime;

        if (currentPreferedTime >= preferedCardTime)
        {
            GetPreferedCardType();
            currentPreferedTime = 0f;
        }
    }

    void GetPreferedCardType()
    {
        preferedType = acceptedCard[Random.Range(0, acceptedCard.Length)];
        gameManager.farm.SetPreferTypeCard($"Coin Farm prefer {preferedType} card");
    }

    void ProduceCoin()
    {
        if (!IsProducing) return;
        currentProduceTime += Time.deltaTime;

        if (currentProduceTime >= produceCoinTime * ProduceTimeMultiplier)
        {
            currentProduceTime = 0f;
            GenerateCoin();
        }

        SetFillImage(currentProduceTime);
    }

    protected override void HandleTriggerEnter(Collider2D collision)
    {
        SetFarmTrigger(collision);
    }

    protected override void HandleTriggerStay(Collider2D collision)
    {
        SetFarmTrigger(collision);
    }

    void SetFarmTrigger(Collider2D collision)
    {
        Card card = GetTriggeredCard(collision);

        if (card == null) return;

        if (card.Type == preferedType)
        {
            selectedCard = card;
            return;
        }

        selectedCard = card;
    }

    void SetFillImage(float value)
    {
        fillImage.fillAmount = value;
    }

    void GenerateCoin()
    {
        Vector3 spawnPosition = GameUtility.GetRandomScatterPosition(transform.position, maxSpawnRadius);
        Instantiate(coinCardPrefab, spawnPosition, Quaternion.identity);
    }
}
