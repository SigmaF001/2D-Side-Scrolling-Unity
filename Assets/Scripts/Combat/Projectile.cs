using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Set by PlayerCombat.Fire()
    [HideInInspector] public float Damage = 4f;
    [HideInInspector] public float Speed = 12f;
    [HideInInspector] public Vector2 Direction = Vector2.right;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 3f;

    private void Start() => Destroy(gameObject, lifetime);

    private void Update()
    {
        transform.Translate(Direction * Speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ยิงโดนศัตรู
        var enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(Damage);
            Destroy(gameObject);
            return;
        }

        // ยิงโดนสิ่งกีดขวาง
        if (!other.isTrigger && !other.CompareTag("Player"))
            Destroy(gameObject);
    }
}