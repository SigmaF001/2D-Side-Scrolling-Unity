using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private bool isMinislime = false;

    [Header("Split")]
    [SerializeField] private GameObject miniSlimePrefab;
    [SerializeField] private int splitCount = 2;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float detectionRange = 6f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float attackCooldown = 1f;

    private Rigidbody2D _rb;
    private EnemyHealth _health;
    private Animator _animator;
    private Transform _player;

    private float _nextAttackTime;
    private bool _isDying;

    private bool _attackTriggered;

    private enum State { Idle, Chase, Attack }
    private State _state = State.Idle;

    private static readonly int AnimIsMoving = Animator.StringToHash("isMoving");
    private static readonly int AnimAttack = Animator.StringToHash("Attack");
    private static readonly int AnimDied = Animator.StringToHash("Died");

#region Unity Lifecycle

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<EnemyHealth>();
        _animator = GetComponent<Animator>();
        _health.OnDeath += OnSlimeDeath;
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) _player = playerObj.transform;
        else Debug.LogWarning("[SlimeAI] ไม่พบ Player — ตรวจสอบว่า Player มี Tag 'Player'");
    }

    private void FixedUpdate()
    {
        if (_isDying || _health.IsDead || _player == null) return;
        UpdateState();
        ExecuteState();
    }

#endregion

#region State Machine

    private void UpdateState()
    {
        float dist = Vector2.Distance(transform.position, _player.position);

        if (dist <= attackRange)         _state = State.Attack;
        else if (dist <= detectionRange) _state = State.Chase;
        else                             _state = State.Idle;
    }

    private void ExecuteState()
    {
        switch (_state)
        {
            case State.Idle:
                _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, 0.2f);
                _animator?.SetBool(AnimIsMoving, false);
                _attackTriggered = false;
                break;

            case State.Chase:
                Vector2 dir = (_player.position - transform.position).normalized;
                _rb.linearVelocity = new Vector2(dir.x * moveSpeed, _rb.linearVelocity.y);
                FlipTowards(dir.x);
                _animator?.SetBool(AnimIsMoving, true);
                _attackTriggered = false;
                break;

            case State.Attack:
                _rb.linearVelocity = Vector2.zero;
                _animator?.SetBool(AnimIsMoving, false);

                bool cooldownReady = Time.time >= _nextAttackTime;
                if (cooldownReady && !_attackTriggered)
                {
                    _animator?.SetTrigger(AnimAttack);
                    _attackTriggered = true;
                }

                TryAttack();
                break;
        }
    }

    private void TryAttack()
    {
        if (Time.time < _nextAttackTime) return;

        _nextAttackTime  = Time.time + attackCooldown;
        _attackTriggered = false;

        // ใช้ singleton ก่อน ถ้าไม่มีค่อย GetComponent
        var ph = PlayerHealth.Instance ?? _player.GetComponent<PlayerHealth>();
        if (ph == null)
        {
            Debug.LogWarning("[SlimeAI] ไม่พบ PlayerHealth บน Player");
            return;
        }

        ph.TakeDamage(attackDamage);
    }

    private void FlipTowards(float dirX)
    {
        if (Mathf.Abs(dirX) < 0.01f) return;
        float absX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(
            dirX > 0 ? absX : -absX,
            transform.localScale.y,
            transform.localScale.z
        );
    }

#endregion

#region Death & Split
    private void OnSlimeDeath(EnemyHealth _)
    {
        _isDying = true;
        _rb.linearVelocity = Vector2.zero;

        if (!isMinislime && miniSlimePrefab != null)
        {
            for (int i = 0; i < splitCount; i++)
            {
                Vector3 offset = new Vector3(i % 2 == 0 ? 0.4f : -0.4f, 0.2f, 0f);
                Instantiate(miniSlimePrefab, transform.position + offset, Quaternion.identity);
            }
        }

        _animator?.SetTrigger(AnimDied);
    }
#endregion

#region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endregion
}