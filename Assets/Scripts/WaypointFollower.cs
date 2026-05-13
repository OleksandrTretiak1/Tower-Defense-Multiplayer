using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    public int GetWaypointIndex() => currentWaypointIndex;
    public float GetDistanceToWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return Mathf.Infinity;

        if (currentWaypointIndex < waypoints.Length)
        {
            return Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position);
        }
        return 0f;
    }

    [SerializeField] private float speed = 2f;
    [SerializeField] private float rotationSpeed = 10f;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    void Awake()
    {
        GameObject pointsParent = GameObject.Find("Waypoints");
        if (pointsParent != null)
        {
            waypoints = new Transform[pointsParent.transform.childCount];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = pointsParent.transform.GetChild(i);
            }

            transform.position = waypoints[0].position;

            if (waypoints.Length > 1)
            {
                Vector2 dir = waypoints[1].position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                currentWaypointIndex = 1;
            }
        }
    }

    void Update()
    {
        if (waypoints == null || currentWaypointIndex >= waypoints.Length) return;

        Move();
        RotateTowardsTarget();
    }

    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position,
            waypoints[currentWaypointIndex].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.05f)
        {
            Vector2 dir = waypoints[currentWaypointIndex].position - transform.position;
            if (dir.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            currentWaypointIndex++;
        }

        if (currentWaypointIndex == waypoints.Length)
        {
            if (BaseHealth.instance != null)
            {
                EnemyHealth enemyHealth = GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    BaseHealth.instance.TakeDamage(enemyHealth.damageToBase);
                }
                else
                {
                    BaseHealth.instance.TakeDamage(10);
                }
            }

            Destroy(gameObject);
            return;
        }
    }

    void RotateTowardsTarget()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Vector2 direction = waypoints[currentWaypointIndex].position - transform.position;
        if (direction.magnitude < 0.1f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}