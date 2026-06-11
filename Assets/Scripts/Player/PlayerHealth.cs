using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    public event Action<float, float> OnAPChanged;  // (current, max)
    public event Action OnPlayerDied;

    [Header("Arcane Power")]
    [SerializeField] private float maxAP = 100f;

    [Header("Respawn Settings")]
    [SerializeField] private GameObject respawnPoint;

    public float MaxAP     { get; private set; }
    public float CurrentAP { get; private set; }
    public bool  IsDead    => CurrentAP <= 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        MaxAP = maxAP;
        CurrentAP = maxAP;
    }


    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;
        CurrentAP = Mathf.Max(0f, CurrentAP - amount);
        OnAPChanged?.Invoke(CurrentAP, MaxAP);
        Debug.Log($"[Player] AP: {CurrentAP:F0}/{MaxAP:F0}");

        if (IsDead) { Die(); }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;
        CurrentAP = Mathf.Min(MaxAP, CurrentAP + amount);
        OnAPChanged?.Invoke(CurrentAP, MaxAP);
    }

    public void ResetAP()
    {
        CurrentAP = MaxAP;
        OnAPChanged?.Invoke(CurrentAP, MaxAP);
        Debug.Log("[Player] AP fully restored.");
    }

    private void Die()
    {
        Debug.Log("[Player] Died, resetting AP.");
        OnPlayerDied?.Invoke();
        // ResetAP();
        GetComponent<Animator>()?.SetBool("Died", true);
    }

    public void RespawnAtCheckpoint()
    {
        if (respawnPoint != null)
        {
            GetComponent<Animator>()?.SetBool("Died", false);
            transform.position = respawnPoint.transform.position;
            Debug.Log("[Player] Respawned at checkpoint.");
            ResetAP();
        }
    }
}
