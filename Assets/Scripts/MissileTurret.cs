using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Mirror;

public class MissileTurret : NetworkBehaviour
{
    [Header("Upgrade Economics")]
    public int upgradeCostLvl2 = 120;
    public int upgradeCostLvl3 = 250;

    [Header("Attributes")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Setup Fields")]
    [SerializeField] private Transform partToRotate;
    [SerializeField] private GameObject missilePrefabLvl1;
    [SerializeField] private GameObject missilePrefabLvl3;

    [Header("Visual Levels")]
    [SyncVar(hook = nameof(OnLevelChanged))]
    [SerializeField] private int currentLevel = 1;

    [SerializeField] private GameObject visualLevel1;
    [SerializeField] private GameObject visualLevel2;
    [SerializeField] private GameObject visualLevel3;

    [Header("Missiles to Hide (Reload Effect)")]
    [SerializeField] private GameObject lvl1_LeftMissile;
    [SerializeField] private GameObject lvl1_RightMissile;
    [SerializeField] private GameObject lvl2_LeftMissile;
    [SerializeField] private GameObject lvl2_RightMissile;
    [SerializeField] private GameObject lvl3_SuperMissile;

    [Header("Fire Points")]
    [SerializeField] private Transform firePointL;
    [SerializeField] private Transform firePointR;
    [SerializeField] private Transform firePointCenter;

    [Header("Audio")]
    [SerializeField] private AudioSource shootingAudioSource;
    [SerializeField] private AudioClip shootSound;

    private float _range;
    private float _fireRate;
    private float _fireCountdown = 0f;
    private int _shootSide = 0;

    [SyncVar] private GameObject _syncedTarget;

    private void Start()
    {
        SetLevelStats();
        SetupLevelVisuals();

        if (isServer)
        {
            InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
        }

        LinkToNode();
    }

    private void Update()
    {
        if (_syncedTarget != null)
        {
            LockOnTarget();
        }

        if (!isServer)
        {
            return;
        }

        if (_syncedTarget == null)
        {
            return;
        }

        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / _fireRate;
        }

        _fireCountdown -= Time.deltaTime;
    }

    [Server]
    private void UpdateTarget()
    {
        EnemyHealth bestTarget = null;
        int maxWaypoint = -1;
        float minDistanceToNext = Mathf.Infinity;

        foreach (EnemyHealth enemy in EnemyHealth.AllEnemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= _range)
            {
                WaypointFollower follower = enemy.GetComponent<WaypointFollower>();

                if (follower != null)
                {
                    int enemyWaypoint = follower.GetWaypointIndex();
                    float enemyDist = follower.GetDistanceToWaypoint();

                    if (enemyWaypoint > maxWaypoint || (enemyWaypoint == maxWaypoint && enemyDist < minDistanceToNext))
                    {
                        maxWaypoint = enemyWaypoint;
                        minDistanceToNext = enemyDist;
                        bestTarget = enemy;
                    }
                }
            }
        }

        _syncedTarget = (bestTarget != null) ? bestTarget.gameObject : null;
    }

    private void LockOnTarget()
    {
        Vector2 dir = _syncedTarget.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion lookRotation = Quaternion.Euler(0, 0, angle);
        partToRotate.rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    [Server]
    private void Shoot()
    {
        RpcPlayShootEffects(currentLevel, _shootSide, _syncedTarget);

        if (_shootSide == 0)
        {
            _shootSide = 1;
        }
        else
        {
            _shootSide = 0;
        }
    }

    [ClientRpc]
    private void RpcPlayShootEffects(int level, int side, GameObject targetObj)
    {
        if (targetObj == null)
        {
            return;
        }

        Transform spawnPoint = firePointCenter;
        GameObject visualToHide = null;

        if (level == 1)
        {
            spawnPoint = (side == 0) ? firePointL : firePointR;
            visualToHide = (side == 0) ? lvl1_LeftMissile : lvl1_RightMissile;
        }
        else if (level == 2)
        {
            spawnPoint = (side == 0) ? firePointL : firePointR;
            visualToHide = (side == 0) ? lvl2_LeftMissile : lvl2_RightMissile;
        }
        else if (level == 3)
        {
            spawnPoint = firePointCenter;
            visualToHide = lvl3_SuperMissile;
        }

        GameObject prefabToSpawn = (level == 3) ? missilePrefabLvl3 : missilePrefabLvl1;

        if (prefabToSpawn != null)
        {
            GameObject m = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            Projectile proj = m.GetComponent<Projectile>();

            if (proj != null)
            {
                proj.Seek(targetObj.transform);
            }
        }

        if (visualToHide != null)
        {
            StartCoroutine(ReloadEffect(visualToHide));
        }

        if (shootingAudioSource != null && shootSound != null)
        {
            shootingAudioSource.PlayOneShot(shootSound);
        }
    }

    [Server]
    public void UpgradeTower()
    {
        if (currentLevel >= 3)
        {
            return;
        }

        currentLevel++;
        SetLevelStats();
    }

    private void OnLevelChanged(int oldLevel, int newLevel)
    {
        SetupLevelVisuals();
    }

    private void SetLevelStats()
    {
        if (currentLevel == 1)
        {
            _fireRate = 0.8f;
            _range = 6f;
        }
        else if (currentLevel == 2)
        {
            _fireRate = 1.2f;
            _range = 7f;
        }
        else if (currentLevel == 3)
        {
            _fireRate = 0.2f;
            _range = 12f;
        }
    }

    private void SetupLevelVisuals()
    {
        if (visualLevel1 != null)
        {
            visualLevel1.SetActive(currentLevel == 1);
        }

        if (visualLevel2 != null)
        {
            visualLevel2.SetActive(currentLevel == 2);
        }

        if (visualLevel3 != null)
        {
            visualLevel3.SetActive(currentLevel == 3);
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    private IEnumerator ReloadEffect(GameObject missile)
    {
        missile.SetActive(false);
        yield return new WaitForSeconds(1.2f);
        missile.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    private void LinkToNode()
    {
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);

        foreach (Node n in allNodes)
        {
            if (Vector3.Distance(n.transform.position, transform.position) < 0.1f)
            {
                n.turret = this.gameObject;
                break;
            }
        }
    }
}