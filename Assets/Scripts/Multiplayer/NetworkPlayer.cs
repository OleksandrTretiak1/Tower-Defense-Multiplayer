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
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        Node targetNode = null;

        foreach (Node n in allNodes)
        {
            if (Vector3.Distance(n.transform.position, nodePos) < 0.1f)
            {
                targetNode = n;
                break;
            }
        }

        if (targetNode != null)
        {
            BuildManager.instance.ServerBuildTurret(targetNode, turretType);
        }
    }
}