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

    private Node targetNode;

    void Awake()
    {
        if (instance == null) instance = this;
        Hide();
    }

    void Update()
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
        if (EventSystem.current.IsPointerOverGameObject()) return true;

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
        if (targetNode == node && mainCanvas.activeSelf)
        {
            Hide();
            return;
        }

        targetNode = node;
        transform.position = node.transform.position;

        int cost = targetNode.GetUpgradeCost();
        if (cost <= 0)
        {
            upgradeCostText.text = "MAX";
        }
        else
        {
            upgradeCostText.text = "$" + cost;
        }

        sellAmountText.text = "$" + targetNode.GetSellAmount();

        mainCanvas.SetActive(true);
    }

    public void Hide()
    {
        targetNode = null;
        mainCanvas.SetActive(false);
    }

    public void Upgrade()
    {
        if (targetNode != null)
        {
            targetNode.RequestUpgrade();
            Hide();
        }
    }

    public void Sell()
    {
        if (targetNode != null)
        {
            targetNode.RequestSell();
            Hide();
        }
    }
}