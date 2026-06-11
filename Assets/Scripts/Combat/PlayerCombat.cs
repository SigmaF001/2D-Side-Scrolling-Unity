using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Wand Config")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform  firePoint;

    [Header("Damage (random range)")]
    [SerializeField] private float minDamage = 3f;
    [SerializeField] private float maxDamage = 5f;

    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 12f;

    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float apCostPerShot = 0f;

    private float _nextFireTime;
    private bool  isFacingRight;
    private PlayerHealth _health;

    private void Awake() => _health = GetComponent<PlayerHealth>();

    public void Shoot()
    {
        isFacingRight = GetComponent<PlayerController>().isFacingRight;
        if (projectilePrefab == null || _health.IsDead) return;
        if (_health.CurrentAP < apCostPerShot) { Debug.Log("[Combat] Not enough AP!"); return; }

        _nextFireTime = Time.time + fireRate;
        if (apCostPerShot > 0) _health.TakeDamage(apCostPerShot);

        Vector3 origin = firePoint ? firePoint.position : transform.position;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;

        var projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        var proj = projectile.GetComponent<Projectile>();
        if (proj)
        {
            proj.Damage = Random.Range(minDamage, maxDamage);
            proj.Speed = projectileSpeed;
            proj.Direction = direction;
        }

        Vector3 scale = projectile.transform.localScale;
        projectile.transform.localScale = new Vector3(isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x), scale.y, scale.z);

        if (_health.CurrentAP <= 0f) { _health.TakeDamage(0); }
    }
}