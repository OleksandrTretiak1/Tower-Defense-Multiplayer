using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class NodeUI : MonoBehaviour
{
    public static NodeUI instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainCanvas;

    [Header("Upgrade/Sell Elements")]
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI sellAmountText;

    private Node _targetNode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Hide();
    }

    private void Update()
    {
        if (mainCanvas.activeSelf && Pointer.current.press.wasPressedThisFrame)
        {
            if (!IsPointerOverUIElement())
            {
                Hide();
            }
        }
    }

    private bool IsPointerOverUIElement()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetTarget(Node node)
    {
        if (_targetNode == node && mainCanvas.activeSelf)
        {
            Hide();
            return;
        }

        _targetNode = node;
        transform.position = node.transform.position;

        int cost = _targetNode.GetUpgradeCost();
        if (cost <= 0)
        {
            upgradeCostText.text = "MAX";
        }
        else
        {
            upgradeCostText.text = "$" + cost;
        }

        sellAmountText.text = "$" + _targetNode.GetSellAmount();

        mainCanvas.SetActive(true);
    }

    public void Hide()
    {
        _targetNode = null;
        mainCanvas.SetActive(false);
    }

    public void Upgrade()
    {
        if (_targetNode != null)
        {
            _targetNode.RequestUpgrade();
            Hide();
        }
    }

    public void Sell()
    {
        if (_targetNode != null)
        {
            _targetNode.RequestSell();
            Hide();
        }
    }
}