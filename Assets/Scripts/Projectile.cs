using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private Vector3 lastTargetPosition;
    public float speed = 10f;
    public int damage = 25;
    public float explosionRadius = 1f;

    [Header("Visual Effects")]
    public GameObject explosionPrefab;

    public void Seek(Transform _target)
    {
        target = _target;
        if (target != null) lastTargetPosition = target.position;
    }

    void Update()
    {
        if (target != null)
        {
            lastTargetPosition = target.position;
        }

        Vector2 dir = (Vector2)lastTargetPosition - (Vector2)transform.position;
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

    void HitTarget()
    {
        if (explosionPrefab != null)
        {
            GameObject exp = Instantiate(explosionPrefab, transform.position, transform.rotation);

            exp.transform.localScale = Vector3.one * explosionRadius * 2f;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                EnemyHealth e = collider.GetComponent<EnemyHealth>();
                if (e != null) e.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}