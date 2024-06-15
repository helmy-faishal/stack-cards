using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script yang mengatur scene yang menampilkan kartu
public class CardDisplayManager : MonoBehaviour
{
    [SerializeField] Button _nextButton;
    [SerializeField] Button _backButton;
    [SerializeField] Button _closeButton;
    [SerializeField] Transform _pagesParent;

    Action OnChangePage;

    int _totalPages;
    int _currentPage;

    private void Start()
    {
        _totalPages = _pagesParent.childCount;
        _currentPage = 0;
        SetupPages();
        SetupButton();

        OnChangePage?.Invoke();
    }

    private void OnDestroy()
    {
        OnChangePage = null;
    }

    void SetupPages()
    {
        for (int i = 0; i < _totalPages; i++)
        {
            GameObject page = _pagesParent.GetChild(i).gameObject;
            int index = i;

            OnChangePage += () =>
            {
                page.SetActive(index == _currentPage);
            };
        }
    }

    void SetupButton()
    {
        _closeButton.onClick.AddListener(CloseDisplay);

        _nextButton.onClick.AddListener(GoToNext);
        OnChangePage += () =>
        {
            _nextButton.interactable = _currentPage < _totalPages - 1;
        };

        _backButton.onClick.AddListener(GoToBack);
        OnChangePage += () =>
        {
            _backButton.interactable = _currentPage > 0;
        };
    }

    void GoToNext()
    {
        _currentPage++;
        OnChangePage?.Invoke();
    }

    void GoToBack()
    {
        _currentPage--;
        OnChangePage?.Invoke();
    }

    void CloseDisplay()
    {
        GameUtility.UnloadScene("CardDisplay");
    }
}
