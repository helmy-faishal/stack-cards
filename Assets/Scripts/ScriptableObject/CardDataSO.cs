using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card Data", menuName="Card Data")]
public class CardDataSO : ScriptableObject
{
    [SerializeField] CardType _cardType;
    public CardType Type => _cardType;
    [SerializeField] string _cardName;
    public string CardName => _cardName;
    [SerializeField] bool _isDestroyableBySystem;
    public bool IsDestroyableBySystem => _isDestroyableBySystem;


    [SerializeField] CardType[] _acceptedCardType;
    public CardType[] AcceptedCardType => _acceptedCardType;

    [SerializeField] Color _cardColor;
    public Color CardColor => _cardColor;

    [SerializeField] Sprite _cardFrame;
    public Sprite CardFrame => _cardFrame;
}
