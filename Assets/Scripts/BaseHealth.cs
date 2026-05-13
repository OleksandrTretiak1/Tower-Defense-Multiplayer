using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    public static BaseHealth instance;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound;
    private AudioSource audioSource;
    [SerializeField] private AudioClip gameOverSound;

    void Awake()
    {
        if (instance == null) instance = this;
        currentHealth = maxHealth;

        audioSource = GetComponentInChildren<AudioSource>();
    }

    void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        UpdateUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = currentHealth + "/" + maxHealth;

        if (healthBarFill != null)
            healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    void GameOver()
    {
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ShowGameOver();
        }
    }

    public void PlayFinalSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}