using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer LocalPlayer;

    public override void OnStartLocalPlayer()
    {
        LocalPlayer = this;
    }

    [Command]
    public void CmdBuildTurret(Vector3 nodePos, string turretType)
    {
        Node targetNode = FindNode(nodePos);

        if (targetNode != null)
        {
            BuildManager.instance.ServerBuildTurret(targetNode, turretType);
        }
    }

    [Command]
    public void CmdUpgradeTurret(Vector3 nodePos)
    {
        Node targetNode = FindNode(nodePos);

        if (targetNode != null)
        {
            targetNode.ServerUpgradeTurret();
        }
    }

    [Command]
    public void CmdSellTurret(Vector3 nodePos)
    {
        Node targetNode = FindNode(nodePos);

        if (targetNode != null)
        {
            targetNode.ServerSellTurret();
        }
    }

    private Node FindNode(Vector3 pos)
    {
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);

        foreach (Node n in allNodes)
        {
            if (Vector3.Distance(n.transform.position, pos) < 0.1f)
            {
                return n;
            }
        }

        return null;
    }
}