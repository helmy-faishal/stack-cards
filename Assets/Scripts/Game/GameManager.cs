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
    public GameControl control;
    public GameCardDestroyer destroyer;
    public GameFarm farm;
    public GameMission mission;

    [Header("Boundaries")]
    [Tooltip("Referensi kartu yang digunakan dalam game")]
    [SerializeField] GameObject cardReference;
    Bounds bounds;
    public static Bounds PlayableBounds { get; private set; }

    [Header("End Panel UI")]
    // Objek yang terdapat dalam End Panel
    [SerializeField] GameObject endPanel;
    [SerializeField] Button menuButton;
    [SerializeField] Button restartButton;
    [SerializeField] TMP_Text infoText;

    public int MinShopPrice { get; set; }
    public int TotalPackCard { get; set; }

    private void Start()
    {
        if (control == null)
        {
            control = FindObjectOfType<GameControl>();
        }

        if (destroyer == null)
        {
            destroyer = FindObjectOfType<GameCardDestroyer>();
        }
        destroyer.OnCardUpdate += CheckGameCondition;

        if (farm == null)
        {
            farm = FindObjectOfType<GameFarm>();
        }
        farm.OnCoinIncrease += CheckGameCondition;
        farm.OnCoinDecrease += CheckGameCondition;

        if (mission == null)
        {
            mission = FindObjectOfType<GameMission>();
        }
        mission.OnMissionFinished += () =>
        {
            ShowEndPanel(true);
        };

        SetPlayableBoundaries();
        PlayableBounds = bounds;

        endPanel.SetActive(false);
        menuButton.onClick.AddListener(() =>
        {
            GameUtility.SwitchScene("MainMenu");
            Time.timeScale = 1f;
        });
        restartButton.onClick.AddListener(() =>
        {
            GameUtility.RestartScene();
            Time.timeScale = 1f;
        });

        AudioManager.instance?.PlayMusic("Game");
    }

    // Mengecek kondisi apakah masih dapat melanjutkan permainan
    void CheckGameCondition()
    {
        if (farm.TotalCoin < MinShopPrice && destroyer.TotalCard == 0 && TotalPackCard <= 0)
        {
            ShowEndPanel(false);
        }
    }

    void ShowEndPanel(bool win)
    {
        endPanel.SetActive(true);
        string text;
        if (win)
        {
            text = "<color=#33AD49>You Win!</color>\r\nCongrats!";
        }
        else
        {
            text = "<color=red>You Lose!</color>\r\nUnfortunately you run out of cards!";
        }
        
        infoText.text = text;
        Time.timeScale = 0f;
    }

    // Menghitung area playable berdasarkan
    // (ukuran layar - ukuran referensi kartu)
    void SetPlayableBoundaries()
    {
        float cardWidth = 0;
        float cardHeight = 0;
        foreach (Renderer render in cardReference.GetComponentsInChildren<Renderer>(true))
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

        bounds = new Bounds();
        bounds.Encapsulate(minScreen);
        bounds.Encapsulate(maxScreen);
    }

    private void OnDrawGizmos()
    {
        if (bounds != null)
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
