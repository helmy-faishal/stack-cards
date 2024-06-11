using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kelas dasar untuk semua kartu
public abstract class Card : MonoBehaviour
{
    [Tooltip("Apakah kartu dapat dihancurkan oleh sistem?")]
    public bool isDestroyableBySystem = false;

    [Tooltip("Renderer yang menampilkan kartu")]
    public Renderer render;

    [Tooltip("Renderer yang menampilkan menampilkan border")]
    public Renderer borderEffect;

    [Tooltip("Canvas UI milik kartu")]
    public Canvas canvas;

    [Tooltip("Tipe pada kartu")]
    [SerializeField] protected CardType type;
    public CardType Type => type;

    [Tooltip("Jenis kartu yang dapat diterima ata berinteraksi dengan kartu ini")]
    [SerializeField] protected CardType[] acceptedCard;
    // Masking dari kartu yang dapat diterima
    protected int acceptedCardMask;

    protected Action OnStopDragging;
    bool _isDragging;
    public bool IsDragging
    {
        get { return _isDragging; }
        set
        {
            if (_isDragging == value) return;

            _isDragging = value;

            ChangeSortingLayer(_isDragging);

            SetCardInteractable(_isDragging);

            if (!_isDragging)
            {
                OnStopDragging?.Invoke();
                selectedCard = null;
            }
        }
    }

    protected int initSortingID;
    void ChangeSortingLayer(bool drag)
    {
        if (render == null || canvas == null) return;
        render.sortingLayerID = drag ? SortingLayer.NameToID("Dragged") : initSortingID;
        canvas.sortingLayerID = render.sortingLayerID;
    }

    // Kartu yang saat ini berinteraksi
    protected Card selectedCard;

    public Transform MainTransform => transform.parent;
    public Action<Card> OnCardDestroyed;

    protected GameManager gameManager;
    BoxCollider2D boxCollider;
    

    private void Awake()
    {
        SetAwake();
    }

    protected virtual void SetAwake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        SetCardTrigger(true);
    }

    public void SetCardTrigger(bool active)
    {
        if (boxCollider == null) return;
        boxCollider.enabled = active;
    }

    private void Start()
    {
        SetStart();
    }

    protected virtual void SetStart()
    {
        acceptedCardMask = GameUtility.GetCardTypeMask(acceptedCard);

        if (render == null)
        {
            render = MainTransform.GetComponentInChildren<Renderer>();
        }
        initSortingID = render.sortingLayerID;

        SetCardInteractable(false);

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (isDestroyableBySystem)
        {
            gameManager.destroyer.AddCard(this);
            OnCardDestroyed += gameManager.destroyer.RemoveCard;
        }

        gameManager.control.OnCardStartDrag += SetStartCardDragging;
        gameManager.control.OnCardEndDrag += SetEndCardDragging;
    }

    private void OnDestroy()
    {
        SetOnDestroy();
    }

    protected virtual void SetOnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.control.OnCardStartDrag -= SetStartCardDragging;
            gameManager.control.OnCardEndDrag -= SetEndCardDragging;
        }
    }

    protected virtual void SetStartCardDragging(Card card)
    {
        if (card == null) return;

        if (card == this)
        {
            IsDragging = true;
        }
        else if (GameUtility.CheckCardTypeMask(acceptedCardMask,card.type, true))
        {
            SetCardInteractable(true);
        }
    }

    protected virtual void SetEndCardDragging(Card card)
    {
        if (card == this)
        {
            IsDragging = false;
        }
        else
        {
            SetCardInteractable(false);
        }
    }

    public void SetCardInteractable(bool active)
    {
        if (borderEffect == null) return;

        borderEffect.gameObject.SetActive(active);
    }

    public virtual void DestroyCard()
    {
        Destroy(MainTransform.gameObject);
    }

    protected Card GetTriggeredCard(Collider2D collision)
    {
        if (collision.TryGetComponent(out Card card))
        {
            if (GameUtility.CheckCardTypeMask(acceptedCardMask, card.type, true))
            {
                return card;
            }
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTriggerEnter(collision);
    }

    protected virtual void HandleTriggerEnter(Collider2D collision)
    {
        Card card = GetTriggeredCard(collision);

        if (card == null) return;

        if (selectedCard == null)
        {
            selectedCard = card;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleTriggerStay(collision);
    }

    protected virtual void HandleTriggerStay(Collider2D collision)
    {
        Card card = GetTriggeredCard(collision);
        if (card == null) return;

        if (selectedCard == null)
        {
            selectedCard = card;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HandleTriggerExit(collision);
    }

    protected virtual void HandleTriggerExit(Collider2D collision)
    {
        Card card = GetTriggeredCard(collision);
        if (card == null) return;

        if (selectedCard == card)
        {
            selectedCard = null;
        }
    }
}
