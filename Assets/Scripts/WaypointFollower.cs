using UnityEngine;
using Mirror;

public class WaypointFollower : NetworkBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;

    private void Awake()
    {
        GameObject pointsParent = GameObject.Find("Waypoints");

        if (pointsParent != null)
        {
            _waypoints = new Transform[pointsParent.transform.childCount];

            for (int i = 0; i < _waypoints.Length; i++)
            {
                _waypoints[i] = pointsParent.transform.GetChild(i);
            }

            transform.position = _waypoints[0].position;

            if (_waypoints.Length > 1)
            {
                Vector2 dir = _waypoints[1].position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                _currentWaypointIndex = 1;
            }
        }
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (_waypoints == null || _currentWaypointIndex >= _waypoints.Length)
        {
            return;
        }

        Move();
        RotateTowardsTarget();
    }

    public int GetWaypointIndex()
    {
        return _currentWaypointIndex;
    }

    public float GetDistanceToWaypoint()
    {
        if (_waypoints == null || _waypoints.Length == 0)
        {
            return Mathf.Infinity;
        }

        if (_currentWaypointIndex < _waypoints.Length)
        {
            return Vector2.Distance(transform.position, _waypoints[_currentWaypointIndex].position);
        }

        return 0f;
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, _waypoints[_currentWaypointIndex].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _waypoints[_currentWaypointIndex].position) < 0.05f)
        {
            Vector2 dir = _waypoints[_currentWaypointIndex].position - transform.position;

            if (dir.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            _currentWaypointIndex++;
        }

        if (_currentWaypointIndex == _waypoints.Length)
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

            NetworkServer.Destroy(gameObject);
            return;
        }
    }

    private void RotateTowardsTarget()
    {
        if (_currentWaypointIndex >= _waypoints.Length)
        {
            return;
        }

        Vector2 direction = _waypoints[_currentWaypointIndex].position - transform.position;

        if (direction.magnitude < 0.1f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}