using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ShopUI : MonoBehaviour
{
    public static ShopUI instance;

    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private TextMeshProUGUI standardPriceText;
    [SerializeField] private TextMeshProUGUI missilePriceText;

    private Node _targetNode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Hide();
    }

    private void Start()
    {
        standardPriceText.text = "$" + BuildManager.instance.standardTurretPrice;
        missilePriceText.text = "$" + BuildManager.instance.missileTurretPrice;
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
        _targetNode = node;
        transform.position = node.transform.position;
        mainCanvas.SetActive(true);
    }

    public void Hide()
    {
        _targetNode = null;
        mainCanvas.SetActive(false);
    }

    public void SelectStandard()
    {
        BuildManager.instance.BuildTurretOn(_targetNode, "Standard");
        Hide();
    }

    public void SelectMissile()
    {
        BuildManager.instance.BuildTurretOn(_targetNode, "Missile");
        Hide();
    }
}