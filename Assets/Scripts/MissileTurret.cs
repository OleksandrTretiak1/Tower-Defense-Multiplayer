using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class MissileTurret : MonoBehaviour //Turret
{
    [Header("Attributes")]
    private float range;
    private float fireRate;
    private float fireCountdown = 0f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Setup Fields")]
    [SerializeField] private Transform partToRotate;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private GameObject missilePrefabLvl1;
    [SerializeField] private GameObject missilePrefabLvl3;

    [Header("Visual Levels")]
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

    [Header("Upgrade Economics")]
    public int upgradeCostLvl2 = 120;
    public int upgradeCostLvl3 = 250;

    private Transform target;
    private int shootSide = 0;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        SetLevelStats();
        SetupLevelVisuals();
    }

    void SetLevelStats()
    {
        switch (currentLevel)
        {
            case 1: fireRate = 0.8f; range = 6f; break;
            case 2: fireRate = 1.2f; range = 7f; break;
            case 3: fireRate = 0.2f; range = 12f; break;
        }
    }

    void SetupLevelVisuals()
    {
        if (visualLevel1 != null) visualLevel1.SetActive(currentLevel == 1);
        if (visualLevel2 != null) visualLevel2.SetActive(currentLevel == 2);
        if (visualLevel3 != null) visualLevel3.SetActive(currentLevel == 3);
    }

    void UpdateTarget()
    {
        EnemyHealth bestTarget = null;
        int maxWaypoint = -1;
        float minDistanceToNext = Mathf.Infinity;

        foreach (EnemyHealth enemy in EnemyHealth.AllEnemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= range)
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
        target = (bestTarget != null) ? bestTarget.transform : null;
    }

    void Update()
    {
        if (target != null)
        {
            LockOnTarget();
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
        }

        fireCountdown -= Time.deltaTime;

        if (Keyboard.current.uKey.wasPressedThisFrame) UpgradeTower();
    }

    void LockOnTarget()
    {
        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion lookRotation = Quaternion.Euler(0, 0, angle);
        partToRotate.rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void Shoot()
    {
        Transform spawnPoint = firePointCenter;
        GameObject visualToHide = null;

        if (currentLevel == 1)
        {
            spawnPoint = (shootSide == 0) ? firePointL : firePointR;
            visualToHide = (shootSide == 0) ? lvl1_LeftMissile : lvl1_RightMissile;
            shootSide = (shootSide == 0) ? 1 : 0;
        }
        else if (currentLevel == 2)
        {
            spawnPoint = (shootSide == 0) ? firePointL : firePointR;
            visualToHide = (shootSide == 0) ? lvl2_LeftMissile : lvl2_RightMissile;
            shootSide = (shootSide == 0) ? 1 : 0;
        }
        else if (currentLevel == 3)
        {
            spawnPoint = firePointCenter;
            visualToHide = lvl3_SuperMissile;
        }

        GameObject prefabToSpawn = (currentLevel == 3) ? missilePrefabLvl3 : missilePrefabLvl1;
        GameObject m = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        m.GetComponent<Projectile>().Seek(target);

        if (visualToHide != null) StartCoroutine(ReloadEffect(visualToHide));

        if (shootingAudioSource != null && shootSound != null)
            shootingAudioSource.PlayOneShot(shootSound);
    }

    IEnumerator ReloadEffect(GameObject missile)
    {
        missile.SetActive(false);
        yield return new WaitForSeconds(1.2f);
        missile.SetActive(true);
    }

    public void UpgradeTower()
    {
        if (currentLevel >= 3) return;
        currentLevel++;
        SetLevelStats();
        SetupLevelVisuals();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}