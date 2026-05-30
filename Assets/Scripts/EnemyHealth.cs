using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class EnemyHealth : NetworkBehaviour
{
    public static List<EnemyHealth> AllEnemies = new List<EnemyHealth>();

    [SyncVar]
    [SerializeField] private int health = 50;

    [Header("Death Effects")]
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private AudioClip deathSound;

    [Header("Rewards")]
    [SerializeField] private int moneyReward = 10;

    [Header("Base Damage")]
    public int damageToBase = 10;

    private void OnEnable()
    {
        AllEnemies.Add(this);
    }

    private void OnDisable()
    {
        AllEnemies.Remove(this);
    }

    public void TakeDamage(int damage)
    {
        if (!isServer)
        {
            return;
        }

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    [Server]
    void Die()
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddMoney(moneyReward);
        }

        RpcHandleDeath(transform.position);

        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    void RpcHandleDeath(Vector3 deathPosition)
    {
        if (deathVFX != null)
        {
            Instantiate(deathVFX, deathPosition, Quaternion.identity);
        }

        if (deathSound != null)
        {
            PlaySound2D(deathSound);
        }
    }

    void PlaySound2D(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        GameObject tempGO = new GameObject("TempAudio_Death");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();

        aSource.clip = clip;
        aSource.spatialBlend = 0f;
        aSource.volume = 1f;
        aSource.playOnAwake = false;

        aSource.Play();

        Destroy(tempGO, clip.length);
    }
}