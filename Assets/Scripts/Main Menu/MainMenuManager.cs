using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script untuk mengatur main menu
public class MainMenuManager : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] Button _playButton;
    [SerializeField] Button _cardButton;
    [SerializeField] Button _settingButton;
    [SerializeField] Button _exitButton;

    [Header("Panel")]
    [SerializeField] GameObject _settingPanel;

    // Start is called before the first frame update
    void Start()
    {
        _playButton.onClick.AddListener(PlayGame);
        
        _cardButton.onClick.AddListener(OpenCardDisplay);

        _settingButton.onClick.AddListener(OpenSetting);
        _settingPanel.SetActive(false);
        
        _exitButton.onClick.AddListener(ExitGame);

        AudioManager.Instance?.PlayMusic("MainMenu");
    }

    void PlayGame()
    {
        GameUtility.SwitchScene("Game");
    }

    void OpenCardDisplay()
    {
        GameUtility.SwitchScene("CardDisplay", true);
    }

    void OpenSetting()
    {
        _settingPanel.SetActive(true);
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
