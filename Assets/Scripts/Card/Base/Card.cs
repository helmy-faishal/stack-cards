using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool CanCombineCard { get; set; }

    public Renderer Render;
    [SerializeField] protected Renderer m_Border;
    [SerializeField] protected CardCanvas m_CardCanvas;

    public CardDataSO CardDataSO;
    public bool IsDestroyableBySystem { get; protected set; }
    public CardType CardType { get; protected set; }
    protected CardType[] m_AcceptedCard;
    protected int m_AcceptedCardMask;

    protected List<Card> m_TriggeredCard;
    public bool HasTriggeredCard => m_TriggeredCard.Count > 0;

    public virtual Transform MainTransform => transform;
    public Action<Card> OnCardDestroyed;

    protected GameControl m_GameControl;
    protected GameManager m_GameManager;
    protected int m_InitSortingID;

    private void Awake()
    {
        SetAwake();
    }

    protected virtual void SetAwake()
    {
        m_TriggeredCard = new List<Card>();
    }

    private void Start()
    {
        SetStart();
    }

    protected virtual void SetStart()
    {
        if (Render == null)
        {
            Debug.LogError("Render not set!");
        }

        if (m_CardCanvas == null)
        {
            Debug.LogError("CardCanvas not set!");
        }

        SetCardData();
        CanCombineCard = true;
        m_InitSortingID = Render.sortingLayerID;
        m_CardCanvas.Canvas.sortingLayerID = m_InitSortingID;

        SetCardInteractable(false);

        if (m_GameControl == null)
        {
            m_GameControl = FindObjectOfType<GameControl>();
        }
        m_GameControl.OnStartDrag += SetStartCardDragging;
        m_GameControl.OnEndDrag += SetEndCardDragging;

        if (m_GameManager == null)
        {
            m_GameManager = FindObjectOfType<GameManager>();
        }
        if (IsDestroyableBySystem)
        {
            m_GameManager.Destroyer.AddCard(this);
            OnCardDestroyed += m_GameManager.Destroyer.RemoveCard;
        }
    }

    private void OnDestroy()
    {
        SetOnDestroy();
    }

    protected virtual void SetOnDestroy()
    {
        if (m_GameControl != null)
        {
            m_GameControl.OnStartDrag += SetStartCardDragging;
            m_GameControl.OnEndDrag += SetEndCardDragging;
        }
    }

    void SetCardData()
    {
        IsDestroyableBySystem = CardDataSO.IsDestroyableBySystem;
        CardType = CardDataSO.Type;
        m_AcceptedCard = CardDataSO.AcceptedCardType;
        m_AcceptedCardMask = GameUtility.GetCardTypeMask(m_AcceptedCard);
        m_CardCanvas.SetCardText(CardDataSO.CardName);
        Render.material.color = CardDataSO.CardColor;
        SpriteRenderer spriteRenderer = Render as SpriteRenderer;
        spriteRenderer.sprite = CardDataSO.CardFrame;
    }

    protected virtual void SetStartCardDragging(Card card)
    {
        if (card == null) return;

        if (card == this)
        {
            IsDragging = true;
        }
        else if (GameUtility.CheckCardTypeMask(m_AcceptedCardMask, card.CardType, true))
        {
            if (CanCombineCard)
            {
                SetCardInteractable(true);
            }
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
            }
        }
    }

    public void ChangeSortingLayer(bool isDragging)
    {
        if (Render == null || m_CardCanvas == null) return;
        Render.sortingLayerID = isDragging ? SortingLayer.NameToID("Dragged") : m_InitSortingID;
        m_CardCanvas.Canvas.sortingLayerID = Render.sortingLayerID;
    }

    public void SetCardInteractable(bool active)
    {
        if (m_Border == null) return;

        m_Border.gameObject.SetActive(active);
    }

    public void SetOrderInLayer(int order)
    {
        Render.sortingOrder = order;
        m_CardCanvas.Canvas.sortingOrder = order;
    }

    public virtual void ChangeCardPosition(Vector3 position)
    {
        MainTransform.position = GameUtility.ClampPointInBounds(GameManager.PlayableBounds,position);
    }

    public virtual void DestroyCard()
    {
        if (IsDragging)
        {
            m_GameControl.OnEndDrag?.Invoke(this);
        }
        Destroy(MainTransform.gameObject);
    }

    protected Card GetTriggeredCard(Collider2D collision)
    {
        if (collision.TryGetComponent(out Card card))
        {
            if (GameUtility.CheckCardTypeMask(m_AcceptedCardMask, card.CardType, true))
            {
                return card;
            }
        }

        return null;
    }

    protected virtual bool CanTriggered => IsDragging;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanTriggered) return;

        HandleTriggerEnter(collision);
    }

    protected virtual void HandleTriggerEnter(Collider2D collision)
    {
        Card card = GetTriggeredCard(collision);

        if (card == null) return;

        m_TriggeredCard.Add(card);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CanTriggered) return;

        HandleTriggerExit(collision);
    }

    protected virtual void HandleTriggerExit(Collider2D collision)
    {
        Card card = GetTriggeredCard(collision);
        if (card == null) return;

        m_TriggeredCard.Remove(card);
    }
}
