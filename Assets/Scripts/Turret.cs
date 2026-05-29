using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Mirror;

public class Turret : NetworkBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Setup Fields")]
    [SerializeField] private Transform partToRotate;

    [Header("Hitscan & Visuals")]
    [SerializeField] private GameObject[] muzzleFlashObjs;
    [SerializeField] private float flashDuration = 0.05f;

    [Header("Upgrade System")]
    [SyncVar(hook = nameof(OnLevelChanged))]
    [SerializeField] private int currentLevel = 1;

    [SerializeField] private GameObject visualLevel1;
    [SerializeField] private GameObject visualLevel2;
    [SerializeField] private GameObject visualLevel3;

    [SerializeField] private AudioClip shootSoundLevel1;
    [SerializeField] private AudioClip shootSoundLevel2;
    [SerializeField] private AudioClip shootSoundLevel3;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource shootingAudioSource;

    [Header("Upgrade Economics")]
    public int upgradeCostLvl2 = 50;
    public int upgradeCostLvl3 = 100;

    private float _range;
    private int _damage;
    private float _fireRate;
    private float _fireCountdown = 0f;

    [SyncVar] private GameObject _syncedTarget;

    void Start()
    {
        SetLevelStats();
        SetupLevelVisuals();
        DeactivateMuzzleFlashes();

        if (isServer)
        {
            InvokeRepeating("UpdateTarget", 0f, 0.5f);
        }

        LinkToNode();
    }

    void Update()
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
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

    public int GetCurrentLevel()
    {
        return currentLevel;
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
        EnemyHealth enemy = _syncedTarget.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(_damage);
        }

        RpcPlayShootEffects(currentLevel);
    }

    [ClientRpc]
    private void RpcPlayShootEffects(int level)
    {
        if (muzzleFlashObjs != null)
        {
            StartCoroutine(FlashEffects());
        }

        if (shootingAudioSource != null)
        {
            AudioClip clipToPlay = shootSoundLevel1;

            if (level == 2)
            {
                clipToPlay = shootSoundLevel2;
            }
            else if (level == 3)
            {
                clipToPlay = shootSoundLevel3;
            }

            if (clipToPlay != null)
            {
                shootingAudioSource.PlayOneShot(clipToPlay);
            }
        }
    }

    private void SetLevelStats()
    {
        if (currentLevel == 1)
        {
            _damage = 10;
            _fireRate = 1f;
            _range = 5f;
        }
        else if (currentLevel == 2)
        {
            _damage = 15;
            _fireRate = 1.8f;
            _range = 6f;
        }
        else if (currentLevel == 3)
        {
            _damage = 5;
            _fireRate = 5f;
            _range = 7.5f;
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

    private void OnLevelChanged(int oldLevel, int newLevel)
    {
        SetupLevelVisuals();
        DeactivateMuzzleFlashes();
    }

    private void DeactivateMuzzleFlashes()
    {
        if (muzzleFlashObjs == null)
        {
            return;
        }

        foreach (GameObject flash in muzzleFlashObjs)
        {
            if (flash != null)
            {
                flash.SetActive(false);
            }
        }
    }

    private IEnumerator FlashEffects()
    {
        foreach (GameObject flash in muzzleFlashObjs)
        {
            if (flash != null)
            {
                flash.SetActive(true);
            }
        }

        yield return new WaitForSeconds(flashDuration);

        foreach (GameObject flash in muzzleFlashObjs)
        {
            if (flash != null)
            {
                flash.SetActive(false);
            }
        }
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