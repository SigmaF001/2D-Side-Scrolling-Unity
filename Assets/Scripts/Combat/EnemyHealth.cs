using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 20f;

    public float MaxHealth     { get; protected set; }
    public float CurrentHealth { get; protected set; }
    public bool  IsDead        => CurrentHealth <= 0f;

    public event Action<EnemyHealth> OnDeath;

    protected virtual void Awake()
    {
        MaxHealth     = maxHealth;
        CurrentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;
        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        Debug.Log($"[Enemy] {name}  HP: {CurrentHealth:F0}/{MaxHealth:F0}");
        OnDamaged(amount);
        if (IsDead) Die();
    }

    /// <summary>Override for hit-flash, knockback, etc.</summary>
    protected virtual void OnDamaged(float amount) { }

    protected virtual void Die()
    {
        OnDeath?.Invoke(this);
        OnKilled();
    }

    /// <summary>Override in sub-classes for drop logic, split logic, etc.</summary>
    protected virtual void OnKilled() => Destroy(gameObject);
}