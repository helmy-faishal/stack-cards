using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// Script yang mengatur kartu yang dapat dihancurkan selama permainan
public class GameCardDestroyer : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("UI untuk menampilkan timer")]
    [SerializeField] Image timerFill;

    [Header("Effect")]
    [Tooltip("Effect yang dihasilkan saat kartu dihancurkan")]
    [SerializeField] ParticleSystem destroyEffect;

    [Header("Card Destroyer")]
    [Tooltip("Jeda waktu antar penghancuran kartu")]
    [SerializeField] float destroyCardDelay = 1f;
    private List<Card> cards = new List<Card>();
    float currentTime;

    public int TotalCard => cards.Count;

    public Action OnCardUpdate;

    private void Start()
    {
        currentTime = destroyCardDelay;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
        OnCardUpdate?.Invoke();
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        OnCardUpdate?.Invoke();
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        SetTimerFill(currentTime);

        if (currentTime < 0f)
        {
            SystemDestroyCard();
            currentTime = destroyCardDelay;
        }
    }

    void SystemDestroyCard()
    {
        if (cards.Count > 0f)
        {
            int index = Random.Range(0, cards.Count);
            Card card = cards[index];

            Instantiate(destroyEffect, card.transform.position, Quaternion.identity);
            AudioManager.instance?.PlaySFX("CardDestroy");

            RemoveCard(card);
            card.DestroyCard();
        }
    }

    void SetTimerFill(float fill)
    {
        if (fill < 0.5f)
        {
            timerFill.color = Color.red;
        }
        else
        {
            timerFill.color = Color.green;
        }

        timerFill.fillAmount = fill;
    }
}
