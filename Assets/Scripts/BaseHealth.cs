using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class BaseHealth : NetworkBehaviour
{
    public static BaseHealth instance;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [SyncVar(hook = nameof(OnHealthChanged))]
    private float _currentHealth;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip gameOverSound;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _audioSource = GetComponentInChildren<AudioSource>();
    }

    public override void OnStartServer()
    {
        _currentHealth = maxHealth;
    }

    private void Start()
    {
        UpdateUI(_currentHealth);
    }

    [Server]
    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    public void PlayFinalSound(AudioClip clip)
    {
        if (clip != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void OnHealthChanged(float oldHealth, float newHealth)
    {
        UpdateUI(newHealth);

        if (newHealth < oldHealth && newHealth > 0)
        {
            if (damageSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(damageSound);
            }
        }

        if (newHealth <= 0 && oldHealth > 0)
        {
            GameOver();
        }
    }

    private void UpdateUI(float currentVal)
    {
        if (healthText != null)
        {
            healthText.text = currentVal + "/" + maxHealth;
        }

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentVal / maxHealth;
        }
    }

    private void GameOver()
    {
        if (gameOverSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(gameOverSound);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ShowGameOver();
        }
    }
}