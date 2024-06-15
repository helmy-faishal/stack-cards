using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardCanvas : MonoBehaviour
{
    [SerializeField] TMP_Text _cardText;
    public Canvas Canvas;

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
    }

    public void SetCardText(string text)
    {
        _cardText.text = text;
    }
}
