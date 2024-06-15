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
    [SerializeField] Image _timerFill;

    [Header("Effect")]
    [Tooltip("Effect yang dihasilkan saat kartu dihancurkan")]
    [SerializeField] ParticleSystem _destroyEffect;

    private List<Card> _cards = new List<Card>();
    float _currentTime;

    public int TotalCard => _cards.Count;

    public Action OnCardUpdate;

    private void Start()
    {
        _currentTime = GameConfig.instance.DestroyCardDelay;
    }

    public void AddCard(Card card)
    {
        _cards.Add(card);
        OnCardUpdate?.Invoke();
    }

    public void RemoveCard(Card card)
    {
        _cards.Remove(card);
        OnCardUpdate?.Invoke();
    }

    private void Update()
    {
        _currentTime -= Time.deltaTime;
        SetTimerFill(_currentTime/GameConfig.instance.DestroyCardDelay);

        if (_currentTime < 0f)
        {
            SystemDestroyCard();
            _currentTime = GameConfig.instance.DestroyCardDelay;
        }
    }

    void SystemDestroyCard()
    {
        if (_cards.Count <= 0f) return;
        try
        {
            int index = Random.Range(0, _cards.Count);
            Card card = _cards[index];

            Instantiate(_destroyEffect, card.transform.position, Quaternion.identity);
            AudioManager.Instance?.PlaySFX("CardDestroy");

            RemoveCard(card);
            card.DestroyCard();
        }
        catch (Exception)
        {
            return;
        }
    }

    void SetTimerFill(float fill)
    {
        if (fill < 0.5f)
        {
            _timerFill.color = Color.red;
        }
        else
        {
            _timerFill.color = Color.green;
        }

        _timerFill.fillAmount = fill;
    }
}
