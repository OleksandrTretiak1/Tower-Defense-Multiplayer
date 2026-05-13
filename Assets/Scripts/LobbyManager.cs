using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject buttonsPanel;
    public GameObject levelSelectPanel;

    [Header("Level Highscores")]
    public TextMeshProUGUI level1BestText;
    public TextMeshProUGUI level2BestText;

    [Header("Audio Settings")]
    public AudioSource menuMusic;
    public Toggle musicToggle;

    void Start()
    {
        buttonsPanel.SetActive(true);
        levelSelectPanel.SetActive(false);

        bool isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        if (musicToggle != null) musicToggle.isOn = isMusicOn;
        if (menuMusic != null) menuMusic.mute = !isMusicOn;

        UpdateHighscores();
    }

    private void UpdateHighscores()
    {
        if (level1BestText != null)
            level1BestText.text = "Best: " + PlayerPrefs.GetInt("Highscore_Level1", 0);

        if (level2BestText != null)
            level2BestText.text = "Best: " + PlayerPrefs.GetInt("Highscore_Level2", 0);
    }

    public void OpenLevelSelect()
    {
        buttonsPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        levelSelectPanel.SetActive(false);
        buttonsPanel.SetActive(true);
    }

    public void ToggleMusic(bool isEnabled)
    {
        if (menuMusic != null) menuMusic.mute = !isEnabled;
        PlayerPrefs.SetInt("MusicEnabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("Highscore_Level1");
        PlayerPrefs.DeleteKey("Highscore_Level2");

        PlayerPrefs.Save();

        UpdateHighscores();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}