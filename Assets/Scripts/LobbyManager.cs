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
    public GameObject StartButtonsPanel;
    public GameObject LevelSelectPanel;

    [Header("UI Panels - Multiplayer")]
    public GameObject MultiplayerButtonsPanel;
    public GameObject HostInterfacePanel;
    public GameObject JoinInterfacePanel;

    [Header("Multiplayer Elements")]
    public TMP_InputField IpInputField;
    public TextMeshProUGUI HostIpDisplayText;
    public TMP_Dropdown LevelSelectDropdown;

    [Header("Level Highscores")]
    public TextMeshProUGUI Level1BestText;
    public TextMeshProUGUI Level2BestText;

    [Header("Audio Settings")]
    public AudioSource MenuMusic;
    public Toggle MusicToggle;

    private bool _isWaitingForPlayer = false;

    private void Start()
    {
        StartButtonsPanel.SetActive(true);

        LevelSelectPanel.SetActive(false);
        MultiplayerButtonsPanel.SetActive(false);
        HostInterfacePanel.SetActive(false);
        JoinInterfacePanel.SetActive(false);

        bool isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        if (MusicToggle != null)
        {
            MusicToggle.isOn = isMusicOn;
        }

        if (MenuMusic != null)
        {
            MenuMusic.mute = !isMusicOn;
        }

        UpdateHighscores();
    }

    private void Update()
    {
        if (_isWaitingForPlayer && NetworkServer.active && NetworkServer.connections.Count == 2)
        {
            _isWaitingForPlayer = false;
            StartMatchAutomatically();
        }
    }

    public void OpenLevelSelect()
    {
        StartButtonsPanel.SetActive(false);
        LevelSelectPanel.SetActive(true);
    }

    public void ToggleMusic(bool isEnabled)
    {
        if (MenuMusic != null)
        {
            MenuMusic.mute = !isEnabled;
        }

        PlayerPrefs.SetInt("MusicEnabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadLevel(string sceneName)
    {
        NetworkManager.singleton.maxConnections = 1;

        NetworkManager.singleton.onlineScene = sceneName;

        NetworkManager.singleton.StartHost();
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
        StartButtonsPanel.SetActive(false);
        MultiplayerButtonsPanel.SetActive(true);
    }

    public void OpenHostInterface()
    {
        MultiplayerButtonsPanel.SetActive(false);
        HostInterfacePanel.SetActive(true);

        NetworkManager.singleton.maxConnections = 2;

        NetworkManager.singleton.onlineScene = "";

        NetworkManager.singleton.StartHost();
        _isWaitingForPlayer = true;

        if (HostIpDisplayText != null)
        {
            HostIpDisplayText.text = "IP: " + GetLocalIPAddress();
        }
    }

    public void OpenJoinInterface()
    {
        MultiplayerButtonsPanel.SetActive(false);
        JoinInterfacePanel.SetActive(true);
    }

    public void ConnectToHost()
    {
        string ipAddress = IpInputField.text;

        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = "localhost";
        }

        NetworkManager.singleton.onlineScene = "WAIT_FOR_LEVEL";
        NetworkManager.singleton.networkAddress = ipAddress;
        NetworkManager.singleton.StartClient();
    }

    public void BackToMainMenu()
    {
        LevelSelectPanel.SetActive(false);
        MultiplayerButtonsPanel.SetActive(false);
        HostInterfacePanel.SetActive(false);
        JoinInterfacePanel.SetActive(false);

        StartButtonsPanel.SetActive(true);

        StopNetwork();
    }

    private void UpdateHighscores()
    {
        if (Level1BestText != null)
        {
            Level1BestText.text = "Best: " + PlayerPrefs.GetInt("Highscore_Level1", 0);
        }

        if (Level2BestText != null)
        {
            Level2BestText.text = "Best: " + PlayerPrefs.GetInt("Highscore_Level2", 0);
        }
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

        return "IP not found";
    }

    private void StartMatchAutomatically()
    {
        string selectedLevelName = LevelSelectDropdown.options[LevelSelectDropdown.value].text;
        NetworkManager.singleton.ServerChangeScene(selectedLevelName);
    }
}