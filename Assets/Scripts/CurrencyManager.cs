using UnityEngine;
using TMPro;
using Mirror;

public class CurrencyManager : NetworkBehaviour
{
    public static CurrencyManager instance;

    [Header("Settings")]
    [SerializeField] private int startingMoney = 200;

    [SyncVar(hook = nameof(OnMoneyChanged))]
    private int _currentMoney;

    [Header("UI")]
    [SerializeField] private SpriteNumberDisplay moneyVisual;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public override void OnStartServer()
    {
        _currentMoney = startingMoney;
    }

    void Start()
    {
        UpdateUI(_currentMoney);
    }

    [Server]
    public void AddMoney(int amount)
    {
        _currentMoney += amount;
    }

    [Server]
    public bool TrySpendMoney(int amount)
    {
        if (_currentMoney >= amount)
        {
            _currentMoney -= amount;
            return true;
        }
        return false;
    }

    private void OnMoneyChanged(int oldMoney, int newMoney)
    {
        UpdateUI(newMoney);
    }

    private void UpdateUI(int currentAmount)
    {
        if (moneyVisual != null)
        {
            moneyVisual.UpdateDisplay(currentAmount);
        }
    }
}