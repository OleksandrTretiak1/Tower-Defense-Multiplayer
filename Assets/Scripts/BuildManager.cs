using UnityEngine;
using Mirror;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [Header("Turret Prefabs")]
    [SerializeField] private GameObject standardTurretPrefab;
    [SerializeField] private GameObject missileTurretPrefab;

    [Header("Prices")]
    public int standardTurretPrice = 50;
    public int missileTurretPrice = 150;

    [Header("Global Build Sounds")]
    [SerializeField] private AudioClip globalBuildSound;
    [SerializeField] private AudioClip globalUpgradeSound;
    [SerializeField] private AudioClip globalSellSound;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void BuildTurretOn(Node node, string type)
    {
        if (NetworkPlayer.LocalPlayer != null)
        {
            NetworkPlayer.LocalPlayer.CmdBuildTurret(node.transform.position, type);
        }
    }

    public void ServerBuildTurret(Node node, string type)
    {
        GameObject turretToBuild = null;
        int price = 0;

        if (type == "Standard")
        {
            turretToBuild = standardTurretPrefab;
            price = standardTurretPrice;
        }
        else if (type == "Missile")
        {
            turretToBuild = missileTurretPrefab;
            price = missileTurretPrice;
        }

        if (turretToBuild != null && CurrencyManager.instance.TrySpendMoney(price))
        {
            GameObject turret = Instantiate(turretToBuild, node.transform.position, Quaternion.identity);
            node.turret = turret;
            node.SetTurret(turret, price);

            NetworkServer.Spawn(turret);

            PlayBuildSound("build");
        }
    }

    public void PlayBuildSound(string type)
    {
        AudioClip clipToPlay = null;

        if (type == "build")
        {
            clipToPlay = globalBuildSound;
        }
        else if (type == "upgrade")
        {
            clipToPlay = globalUpgradeSound;
        }
        else if (type == "sell")
        {
            clipToPlay = globalSellSound;
        }

        if (clipToPlay != null)
        {
            GameObject tempGO = new GameObject("TempAudio_Build");
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = clipToPlay;
            aSource.spatialBlend = 0f;
            aSource.volume = 0.5f;
            aSource.Play();
            Destroy(tempGO, clipToPlay.length);
        }
    }
}