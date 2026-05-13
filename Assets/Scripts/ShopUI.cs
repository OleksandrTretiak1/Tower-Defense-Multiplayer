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

    private Node targetNode;

    void Awake()
    {
        if (instance == null) instance = this;
        Hide();
    }

    void Start()
    {
        standardPriceText.text = "$" + BuildManager.instance.standardTurretPrice;
        missilePriceText.text = "$" + BuildManager.instance.missileTurretPrice;
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
        targetNode = node;
        transform.position = node.transform.position;
        mainCanvas.SetActive(true);
    }

    public void Hide()
    {
        targetNode = null;
        mainCanvas.SetActive(false);
    }

    public void SelectStandard()
    {
        BuildManager.instance.BuildTurretOn(targetNode, "Standard");
        Hide();
    }

    public void SelectMissile()
    {
        BuildManager.instance.BuildTurretOn(targetNode, "Missile");
        Hide();
    }
}