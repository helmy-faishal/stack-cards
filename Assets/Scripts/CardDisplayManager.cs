using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script yang mengatur scene yang menampilkan kartu
public class CardDisplayManager : MonoBehaviour
{
    [SerializeField] Button nextButton;
    [SerializeField] Button backButton;
    [SerializeField] Button closeButton;
    [SerializeField] Transform pagesParent;

    Action OnChangePage;

    int totalPages;
    int currentPage;

    private void Start()
    {
        totalPages = pagesParent.childCount;
        currentPage = 0;
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
        for (int i = 0; i < totalPages; i++)
        {
            GameObject page = pagesParent.GetChild(i).gameObject;
            int index = i;

            OnChangePage += () =>
            {
                page.SetActive(index == currentPage);
            };
        }
    }

    void SetupButton()
    {
        closeButton.onClick.AddListener(CloseDisplay);

        nextButton.onClick.AddListener(GoToNext);
        OnChangePage += () =>
        {
            nextButton.interactable = currentPage < totalPages - 1;
        };

        backButton.onClick.AddListener(GoToBack);
        OnChangePage += () =>
        {
            backButton.interactable = currentPage > 0;
        };
    }

    void GoToNext()
    {
        currentPage++;
        OnChangePage?.Invoke();
    }

    void GoToBack()
    {
        currentPage--;
        OnChangePage?.Invoke();
    }

    void CloseDisplay()
    {
        GameUtility.UnloadScene("CardDisplay");
    }
}
