using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;

[System.Serializable]
public class EnemySubWave
{
    public GameObject enemyPrefab;
    public int count;
    public float rate;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public List<EnemySubWave> enemyGroups;
}

public class Spawner : NetworkBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private float timeBetweenWaves = 5f;

    [SerializeField] private SpriteNumberDisplay waveDisplay;

    [Header("Countdown Settings")]
    [SerializeField] private AudioClip tickSound;
    [SerializeField] private AudioClip startWaveSound;
    [SerializeField] private Color countdownColor = Color.red;
    [SerializeField] private Color waveColor = Color.white;
    [SerializeField] private float initialDelay = 15f;

    [Header("Music Settings")]
    public AudioSource backgroundMusic;

    private AudioSource _audioSource;
    [HideInInspector] [SyncVar] public int currentWaveIndex = 0;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        if (!isServer)
        {
            return;
        }

        StartCoroutine(SpawnAllWaves());
    }

    private IEnumerator SpawnAllWaves()
    {
        yield return new WaitForSeconds(0.2f);

        float countdown = initialDelay;

        while (countdown > 0)
        {
            RpcUpdateWaveUI(Mathf.CeilToInt(countdown), countdownColor);
            RpcPlayTickSound();

            yield return new WaitForSeconds(1f);
            countdown--;
        }

        RpcUpdateWaveUI(0, waveColor);
        RpcPlayStartWaveSound();
        RpcPlayBackgroundMusic();

        yield return new WaitForSeconds(0.5f);

        while (currentWaveIndex < waves.Length)
        {
            RpcUpdateWaveUI(currentWaveIndex + 1, waveColor);

            Wave currentWave = waves[currentWaveIndex];

            foreach (EnemySubWave group in currentWave.enemyGroups)
            {
                for (int i = 0; i < group.count; i++)
                {
                    GameObject enemy = Instantiate(group.enemyPrefab, transform.position, Quaternion.identity);

                    NetworkServer.Spawn(enemy);

                    yield return new WaitForSeconds(group.rate);
                }
            }

            currentWaveIndex++;

            if (currentWaveIndex < waves.Length)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        while (EnemyHealth.AllEnemies.Count > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.WinLevel();
        }
    }

    [ClientRpc]
    private void RpcUpdateWaveUI(int displayValue, Color color)
    {
        if (waveDisplay != null)
        {
            waveDisplay.SetColor(color);
            waveDisplay.UpdateDisplay(displayValue);
        }
    }

    [ClientRpc]
    private void RpcPlayTickSound()
    {
        if (tickSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(tickSound);
        }
    }

    [ClientRpc]
    private void RpcPlayStartWaveSound()
    {
        if (startWaveSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(startWaveSound);
        }
    }

    [ClientRpc]
    private void RpcPlayBackgroundMusic()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }
}