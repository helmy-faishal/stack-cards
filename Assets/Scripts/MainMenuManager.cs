using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script untuk mengatur main menu
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button cardButton;
    [SerializeField] Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        cardButton.onClick.AddListener(OpenCardGlossary);
        exitButton.onClick.AddListener(ExitGame);

        AudioManager.instance?.PlayMusic("MainMenu");
    }

    void PlayGame()
    {
        GameUtility.SwitchScene("Game");
    }

    void OpenCardGlossary()
    {
        GameUtility.SwitchScene("CardDisplay", true);
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
