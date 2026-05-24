using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Net;
using System.Net.Sockets;

public class LobbyManager : MonoBehaviour
{
    [Header("UI Panels - Singleplayer")]
    public GameObject buttonsPanel;
    public GameObject levelSelectPanel;

    [Header("UI Panels - Multiplayer")]
    public GameObject multiplayerButtonsPanel;
    public GameObject hostInterfacePanel;
    public GameObject joinInterfacePanel;

    [Header("Multiplayer Elements")]
    public TMP_InputField ipInputField;
    public TextMeshProUGUI hostIpDisplayText;
    public TMP_Dropdown levelSelectDropdown;

    [Header("Level Highscores")]
    public TextMeshProUGUI level1BestText;
    public TextMeshProUGUI level2BestText;

    [Header("Audio Settings")]
    public AudioSource menuMusic;
    public Toggle musicToggle;

    private bool isWaitingForPlayer = false;

    void Start()
    {
        buttonsPanel.SetActive(true);

        levelSelectPanel.SetActive(false);
        multiplayerButtonsPanel.SetActive(false);
        hostInterfacePanel.SetActive(false);
        joinInterfacePanel.SetActive(false);

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

    public void OpenMultiplayerMenu()
    {
        buttonsPanel.SetActive(false);
        multiplayerButtonsPanel.SetActive(true);
    }

    public void OpenHostInterface()
    {
        multiplayerButtonsPanel.SetActive(false);
        hostInterfacePanel.SetActive(true);

        NetworkManager.singleton.StartHost();
        isWaitingForPlayer = true;
        if (hostIpDisplayText != null)
        {
            hostIpDisplayText.text = "IP: " + GetLocalIPAddress();
        }
    }

    public void OpenJoinInterface()
    {
        multiplayerButtonsPanel.SetActive(false);
        joinInterfacePanel.SetActive(true);
    }
    public void ConnectToHost()
    {
        string ipAddress = ipInputField.text;

        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = "localhost";
        }

        NetworkManager.singleton.networkAddress = ipAddress;
        NetworkManager.singleton.StartClient();
    }

    public void BackToMainMenu()
    {
        levelSelectPanel.SetActive(false);
        multiplayerButtonsPanel.SetActive(false);
        hostInterfacePanel.SetActive(false);
        joinInterfacePanel.SetActive(false);

        buttonsPanel.SetActive(true);

        StopNetwork();
    }

    private void StopNetwork()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "IP не знайдено";
    }

    void Update()
    {
        if (isWaitingForPlayer && NetworkServer.active && NetworkServer.connections.Count == 2)
        {
            isWaitingForPlayer = false;
            StartMatchAutomatically();
        }
    }

    private void StartMatchAutomatically()
    {
        string selectedLevelName = levelSelectDropdown.options[levelSelectDropdown.value].text;
        NetworkManager.singleton.ServerChangeScene(selectedLevelName);
    }
}