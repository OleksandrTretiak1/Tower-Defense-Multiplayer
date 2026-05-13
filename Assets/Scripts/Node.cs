using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Node : MonoBehaviour, IPointerClickHandler
{
    public GameObject turret;
    private int totalMoneyInvested;

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        if (Pointer.current != null)
        {
            eventDataCurrentPosition.position = Pointer.current.position.ReadValue();
        }
        else
        {
            eventDataCurrentPosition.position = eventData.position;
        }

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject != gameObject)
            {
                if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    return;
                }
            }
        }

        if (turret != null)
        {
            if (ShopUI.instance != null) ShopUI.instance.Hide();

            if (NodeUI.instance != null)
            {
                NodeUI.instance.SetTarget(this);
            }
            return;
        }

        if (NodeUI.instance != null)
        {
            NodeUI.instance.Hide();
        }

        if (ShopUI.instance != null)
        {
            ShopUI.instance.SetTarget(this);
        }
    }

    public void SetTurret(GameObject _turret, int price)
    {
        turret = _turret;
        totalMoneyInvested = price;
    }

    public void UpgradeTurretOnNode()
    {
        if (turret == null) return;

        int cost = GetUpgradeCost();
        if (cost <= 0) return;

        if (CurrencyManager.instance.TrySpendMoney(cost))
        {
            Turret t = turret.GetComponent<Turret>();
            MissileTurret mt = turret.GetComponent<MissileTurret>();

            if (t != null) t.UpgradeTower();
            if (mt != null) mt.UpgradeTower();

            totalMoneyInvested += cost;
            BuildManager.instance.PlayBuildSound("upgrade");
        }
    }

    public void SellTurretOnNode()
    {
        if (turret == null) return;

        CurrencyManager.instance.AddMoney(GetSellAmount());
        BuildManager.instance.PlayBuildSound("sell");

        Destroy(turret);
        turret = null;
        totalMoneyInvested = 0;
    }

    public int GetSellAmount()
    {
        return Mathf.RoundToInt(totalMoneyInvested * 0.5f);
    }

    public int GetUpgradeCost()
    {
        if (turret == null) return 0;

        Turret t = turret.GetComponent<Turret>();
        MissileTurret mt = turret.GetComponent<MissileTurret>();

        if (t != null)
        {
            int lvl = t.GetCurrentLevel();
            if (lvl == 1) return t.upgradeCostLvl2;
            if (lvl == 2) return t.upgradeCostLvl3;
            return 0;
        }

        if (mt != null)
        {
            int lvl = mt.GetCurrentLevel();
            if (lvl == 1) return mt.upgradeCostLvl2;
            if (lvl == 2) return mt.upgradeCostLvl3;
            return 0;
        }

        return 0;
    }
}