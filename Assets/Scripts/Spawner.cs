using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

public class Spawner : MonoBehaviour
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

    private AudioSource audioSource;
    [HideInInspector] public int currentWaveIndex = 0;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Start()
    {
        if (backgroundMusic != null)
            backgroundMusic.Stop();

        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        yield return new WaitForSeconds(0.2f);

        float countdown = initialDelay;

        while (countdown > 0)
        {
            waveDisplay.UpdateDisplay(Mathf.CeilToInt(countdown));
            waveDisplay.SetColor(countdownColor);

            if (tickSound != null)
            {
                audioSource.PlayOneShot(tickSound);
            }

            yield return new WaitForSeconds(1f);
            countdown--;
        }

        waveDisplay.SetColor(waveColor);
        waveDisplay.UpdateDisplay(0);

        if (startWaveSound != null)
        {
            audioSource.PlayOneShot(startWaveSound);
        }

        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }

        yield return new WaitForSeconds(0.5f);

        while (currentWaveIndex < waves.Length)
        {
            waveDisplay.UpdateDisplay(currentWaveIndex + 1);

            Wave currentWave = waves[currentWaveIndex];

            foreach (EnemySubWave group in currentWave.enemyGroups)
            {
                for (int i = 0; i < group.count; i++)
                {
                    Instantiate(group.enemyPrefab, transform.position, Quaternion.identity);
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
}