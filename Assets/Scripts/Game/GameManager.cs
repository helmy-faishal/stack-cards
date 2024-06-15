using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Script yang mengatur jalannya permainan
public class GameManager : MonoBehaviour
{
    // Script-script yang mengatur game
    [Header("Game Script")]
    public GameCardDestroyer Destroyer;
    public GameFarm Farm;
    public GameMission Mission;

    [Header("Boundaries")]
    [Tooltip("Referensi kartu yang digunakan dalam game")]
    [SerializeField] GameObject _cardReference;
    Bounds _bounds;
    public static Bounds PlayableBounds { get; private set; }

    [Header("End Panel UI")]
    // Objek yang terdapat dalam End Panel
    [SerializeField] GameObject _endPanel;
    [SerializeField] Button _menuButton;
    [SerializeField] Button _restartButton;
    [SerializeField] TMP_Text _infoText;

    public int MinShopPrice { get; set; }
    public int TotalPackCard { get; set; }

    private void Start()
    {
        if (Destroyer == null)
        {
            Destroyer = FindObjectOfType<GameCardDestroyer>();
        }
        Destroyer.OnCardUpdate += CheckGameCondition;

        if (Farm == null)
        {
            Farm = FindObjectOfType<GameFarm>();
        }
        Farm.OnCoinIncrease += CheckGameCondition;
        Farm.OnCoinDecrease += CheckGameCondition;

        if (Mission == null)
        {
            Mission = FindObjectOfType<GameMission>();
        }
        Mission.OnMissionFinished += () =>
        {
            ShowEndPanel(true);
        };

        SetPlayableBoundaries();
        PlayableBounds = _bounds;

        _endPanel.SetActive(false);
        _menuButton.onClick.AddListener(() =>
        {
            GameUtility.SwitchScene("MainMenu");
            Time.timeScale = 1f;
        });
        _restartButton.onClick.AddListener(() =>
        {
            GameUtility.RestartScene();
            Time.timeScale = 1f;
        });

        AudioManager.Instance?.PlayMusic("Game");
    }

    // Mengecek kondisi apakah masih dapat melanjutkan permainan
    void CheckGameCondition()
    {
        if (Farm.TotalCoin < MinShopPrice && Destroyer.TotalCard == 0 && TotalPackCard <= 0)
        {
            ShowEndPanel(false);
        }
    }

    void ShowEndPanel(bool win)
    {
        _endPanel.SetActive(true);
        string text;
        if (win)
        {
            text = "<color=#33AD49>You Win!</color>\r\nCongrats!";
        }
        else
        {
            text = "<color=red>You Lose!</color>\r\nUnfortunately you run out of cards!";
        }
        
        _infoText.text = text;
        Time.timeScale = 0f;
    }

    // Menghitung area playable berdasarkan
    // (ukuran layar - ukuran referensi kartu)
    void SetPlayableBoundaries()
    {
        float cardWidth = 0;
        float cardHeight = 0;
        foreach (Renderer render in _cardReference.GetComponentsInChildren<Renderer>(true))
        {
            if (render.bounds.extents.x > cardWidth)
            {
                cardWidth = render.bounds.extents.x;
            }

            if (render.bounds.extents.y > cardHeight)
            {
                cardHeight = render.bounds.extents.y;
            }
        }

        Vector2 minScreen = Camera.main.ScreenToWorldPoint(Vector3.zero);
        minScreen.x += cardWidth;
        minScreen.y += cardHeight;

        Vector2 maxScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        maxScreen.x -= cardWidth;
        maxScreen.y -= cardHeight;

        _bounds = new Bounds();
        _bounds.Encapsulate(minScreen);
        _bounds.Encapsulate(maxScreen);
    }

    private void OnDrawGizmos()
    {
        if (_bounds != null)
        {
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }
    }
}
