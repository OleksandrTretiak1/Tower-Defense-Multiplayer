using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [Header("Attributes")]
    private float range;
    private int damage;
    private float fireRate;
    private float fireCountdown = 0f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Setup Fields")]
    [SerializeField] private Transform partToRotate;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Hitscan & Visuals")]
    [SerializeField] private GameObject[] muzzleFlashObjs;
    [SerializeField] private float flashDuration = 0.05f;

    [Header("Upgrade System")]
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

    private Transform target;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        SetLevelStats();
        SetupLevelVisuals();
        DeactivateMuzzleFlashes();
    }

    void SetLevelStats()
    {
        switch (currentLevel)
        {
            case 1:
                damage = 10;
                fireRate = 1f;
                range = 5f;
                break;
            case 2:
                damage = 15;
                fireRate = 1.8f;
                range = 6f;
                break;
            case 3:
                damage = 5;
                fireRate = 5f;
                range = 7.5f;
                break;
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
        if (target == null) return;

        LockOnTarget();

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;

        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            UpgradeTower();
        }
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
        EnemyHealth enemy = target.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        if (muzzleFlashObjs != null)
        {
            StartCoroutine(FlashEffects());
        }

        if (shootingAudioSource != null)
        {
            AudioClip clipToPlay = shootSoundLevel1;

            switch (currentLevel)
            {
                case 2: clipToPlay = shootSoundLevel2; break;
                case 3: clipToPlay = shootSoundLevel3; break;
            }

            if (clipToPlay != null)
            {
                shootingAudioSource.PlayOneShot(clipToPlay);
            }
        }
    }

    IEnumerator FlashEffects()
    {
        foreach (GameObject flash in muzzleFlashObjs)
        {
            if (flash != null) flash.SetActive(true);
        }

        yield return new WaitForSeconds(flashDuration);

        foreach (GameObject flash in muzzleFlashObjs)
        {
            if (flash != null) flash.SetActive(false);
        }
    }

    public void UpgradeTower()
    {
        if (currentLevel >= 3) return;

        currentLevel++;

        SetLevelStats();
        SetupLevelVisuals();
        DeactivateMuzzleFlashes();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void DeactivateMuzzleFlashes()
    {
        if (muzzleFlashObjs == null) return;
        foreach (GameObject flash in muzzleFlashObjs)
        {
            if (flash != null) flash.SetActive(false);
        }
    }
}