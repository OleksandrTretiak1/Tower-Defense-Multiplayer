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
        SetTimeScale();

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
        if (_isGameOver) return;

        _isPaused = !_isPaused;
        pauseMenu.SetActive(_isPaused);

        SetTimeScale();
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

        if (NetworkServer.active)
        {
            ServerRestartGame();
        }
        else if (NetworkClient.active && NetworkPlayer.LocalPlayer != null)
        {
            NetworkPlayer.LocalPlayer.CmdRestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    [Server]
    public void ServerRestartGame()
    {
        NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (NetworkServer.active)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.active)
        {
            NetworkManager.singleton.StopClient();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
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

        SetTimeScale();

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

            if (currentWaveText != null) currentWaveText.text = "Waves: " + currentWave;
            if (highscoreText != null) highscoreText.text = "Best: " + savedHighscore;
        }
    }

    private void SetTimeScale()
    {
        if (_isGameOver)
        {
            Time.timeScale = 0f;
            return;
        }

        if (NetworkClient.active && !NetworkServer.active)
        {
            Time.timeScale = 1f;
            return;
        }

        if (NetworkServer.active)
        {
            if (NetworkServer.connections.Count > 1)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = (_isPaused || _isGameOver) ? 0f : 1f;
            }
        }
        else
        {
            Time.timeScale = (_isPaused || _isGameOver) ? 0f : 1f;
        }
    }
}