using UnityEngine;
using Mirror;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 25;
    public float explosionRadius = 1f;

    [Header("Visual Effects")]
    public GameObject explosionPrefab;

    private Transform _target;
    private Vector3 _lastTargetPosition;

    void Update()
    {
        if (_target != null)
        {
            _lastTargetPosition = _target.position;
        }

        Vector2 dir = (Vector2)_lastTargetPosition - (Vector2)transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void Seek(Transform targetTransform)
    {
        _target = targetTransform;

        if (_target != null)
        {
            _lastTargetPosition = _target.position;
        }
    }

    private void HitTarget()
    {
        if (explosionPrefab != null)
        {
            GameObject exp = Instantiate(explosionPrefab, transform.position, transform.rotation);
            exp.transform.localScale = Vector3.one * explosionRadius * 2f;
        }

        if (NetworkServer.active)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    EnemyHealth e = collider.GetComponent<EnemyHealth>();

                    if (e != null)
                    {
                        e.TakeDamage(damage);
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}