using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    [Header("Settings")]
    [SerializeField] private int startingMoney = 200;
    private int currentMoney;

    [Header("UI")]
    [SerializeField] private SpriteNumberDisplay moneyVisual;

    void Awake()
    {
        if (instance == null) instance = this;
        currentMoney = startingMoney;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateUI();
            return true;
        }

        return false;
    }

    void UpdateUI()
    {
        if (moneyVisual != null)
        {
            moneyVisual.UpdateDisplay(currentMoney);
        }
    }
}