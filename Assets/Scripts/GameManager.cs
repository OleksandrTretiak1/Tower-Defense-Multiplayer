using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;

    [Header("Audio Settings")]
    public Spawner spawner;
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public AudioClip winSound;

    [Header("UI Texts")]
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI highscoreText;

    private bool _isPaused = false;
    private bool _isGameOver = false;

    private AudioSource MusicSource => spawner != null ? spawner.backgroundMusic : null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxToggle.isOn = PlayerPrefs.GetInt("SfxEnabled", 1) == 1;

        ApplyMusic(musicToggle.isOn);
        ApplySfx(sfxToggle.isOn);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && !_isGameOver)
        {
            TogglePause();
        }
    }

    public void ShowGameOver()
    {
        _isGameOver = true;
        Time.timeScale = 0f;

        if (resultTitleText != null)
        {
            resultTitleText.text = "DEFEAT";
        }

        UpdateScoreData();
        gameOverMenu.SetActive(true);
    }

    public void WinLevel()
    {
        if (NetworkServer.active)
        {
            RpcWinLevel();
        }
    }

    public void TogglePause()
    {
        if (_isGameOver)
        {
            return;
        }

        _isPaused = !_isPaused;
        pauseMenu.SetActive(_isPaused);

        Time.timeScale = _isPaused ? 0f : 1f;
    }

    public void OnPauseButtonPressed() => TogglePause();

    public void ApplyMusic(bool isEnabled)
    {
        if (MusicSource != null)
        {
            MusicSource.mute = !isEnabled;
        }
        PlayerPrefs.SetInt("MusicEnabled", isEnabled ? 1 : 0);
    }

    public void ApplySfx(bool isEnabled)
    {
        AudioListener.pause = !isEnabled;
        PlayerPrefs.SetInt("SfxEnabled", isEnabled ? 1 : 0);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    [ClientRpc]
    private void RpcWinLevel()
    {
        StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(2f);

        _isGameOver = true;

        if (winSound != null && BaseHealth.instance != null)
        {
            BaseHealth.instance.PlayFinalSound(winSound);
        }

        Time.timeScale = 0f;

        if (resultTitleText != null)
        {
            resultTitleText.text = "VICTORY!";
        }

        UpdateScoreData();
        gameOverMenu.SetActive(true);
    }

    private void UpdateScoreData()
    {
        if (spawner != null)
        {
            int currentWave = spawner.currentWaveIndex;
            string levelKey = "Highscore_" + SceneManager.GetActiveScene().name;
            int savedHighscore = PlayerPrefs.GetInt(levelKey, 0);

            if (currentWave > savedHighscore)
            {
                savedHighscore = currentWave;
                PlayerPrefs.SetInt(levelKey, savedHighscore);
                PlayerPrefs.Save();
            }

            if (currentWaveText != null)
            {
                currentWaveText.text = "Waves: " + currentWave;
            }

            if (highscoreText != null)
            {
                highscoreText.text = "Best: " + savedHighscore;
            }
        }
    }
}