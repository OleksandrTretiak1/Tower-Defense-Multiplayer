using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }
}