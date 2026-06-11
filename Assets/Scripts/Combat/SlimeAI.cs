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

    [Header("Wander")]
    [SerializeField] private float wanderSpeed = 1.2f;
    [SerializeField] private float wanderMinWalkTime = 1f;
    [SerializeField] private float wanderMaxWalkTime = 2.5f;
    [SerializeField] private float wanderMinPauseTime = 0.8f;
    [SerializeField] private float wanderMaxPauseTime = 2f;

    private Rigidbody2D _rb;
    private Collider2D _collider;
    private EnemyHealth _health;
    private Animator _animator;
    private Transform _player;

    private float _nextAttackTime;
    private bool _isDying;
    private bool _attackTriggered;

    private Vector2 _wanderDir;
    private float _wanderTimer;
    private bool _isWanderPausing = true;

    private enum State { Idle, Wander, Chase, Attack, Hurt }
    private State _state = State.Idle;

    private static readonly int AnimIsMoving = Animator.StringToHash("isMoving");
    private static readonly int AnimAttack = Animator.StringToHash("Attack");
    private static readonly int AnimDied = Animator.StringToHash("Died");
    private static readonly int AnimHurt = Animator.StringToHash("Hurt");

#region Unity Lifecycle

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _health = GetComponent<EnemyHealth>();
        _animator = GetComponent<Animator>();

        _health.OnDeath += OnSlimeDeath;
        _health.OnHurt += OnSlimeHurt;
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) _player = playerObj.transform;
        else Debug.LogWarning("[SlimeAI] ไม่พบ Player — ตรวจสอบว่า Player มี Tag 'Player'");

        _wanderTimer = Random.Range(wanderMinPauseTime, wanderMaxPauseTime);
        _isWanderPausing = true;
    }

    private void FixedUpdate()
    {
        if (_isDying || _health.IsDead || _player == null) return;
        UpdateState();
        ExecuteState();
    }

#endregion

#region State Machine

    private Vector2 BodyCenter =>
        _collider != null ? _collider.bounds.center : (Vector2)transform.position;

    private void UpdateState()
    {
        if (_state == State.Hurt) return;

        float dist = Vector2.Distance(BodyCenter, _player.position);

        if (dist <= attackRange) _state = State.Attack;
        else if (dist <= detectionRange) _state = State.Chase;
        else _state = State.Idle;
    }

    private void ExecuteState()
    {
        switch (_state)
        {
            case State.Idle:
                ExecuteWander();
                break;

            case State.Chase:
                ResetWanderCycle(); // รีเซ็ตรอบ wander เมื่อออกจาก Idle
                Vector2 dir = (_player.position - transform.position).normalized;
                _rb.linearVelocity = new Vector2(dir.x * moveSpeed, _rb.linearVelocity.y);
                FlipTowards(dir.x);
                _animator?.SetBool(AnimIsMoving, true);
                _attackTriggered = false;
                break;

            case State.Attack:
                ResetWanderCycle();
                _rb.linearVelocity = Vector2.zero;
                _animator?.SetBool(AnimIsMoving, false);

                if (Time.time >= _nextAttackTime && !_attackTriggered)
                {
                    _animator?.SetTrigger(AnimAttack);
                    _attackTriggered = true;
                }

                TryAttack();
                break;

            case State.Hurt:
                _rb.linearVelocity = Vector2.zero;
                _animator?.SetBool(AnimIsMoving, false);
                break;
        }
    }

    private void ExecuteWander()
    {
        _wanderTimer -= Time.fixedDeltaTime;

        if (_wanderTimer <= 0f)
            ToggleWanderPhase();

        if (_isWanderPausing)
        {
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, 0.25f);
            _animator?.SetBool(AnimIsMoving, false);
        }
        else
        {
            _rb.linearVelocity = new Vector2(_wanderDir.x * wanderSpeed, _rb.linearVelocity.y);
            FlipTowards(_wanderDir.x);
            _animator?.SetBool(AnimIsMoving, true);
        }

        _attackTriggered = false;
    }

    private void ToggleWanderPhase()
    {
        _isWanderPausing = !_isWanderPausing;

        if (_isWanderPausing)
        {
            _wanderTimer = Random.Range(wanderMinPauseTime, wanderMaxPauseTime);
        }
        else
        {
            float x = Random.value > 0.5f ? 1f : -1f;
            _wanderDir   = new Vector2(x, 0f).normalized;
            _wanderTimer = Random.Range(wanderMinWalkTime, wanderMaxWalkTime);
        }
    }

    private void ResetWanderCycle()
    {
        if (_isWanderPausing) return; // ไม่ต้องทำอะไรถ้าอยู่ใน phase pause อยู่แล้ว
        _isWanderPausing = true;
        _wanderTimer = Random.Range(wanderMinPauseTime, wanderMaxPauseTime);
    }

    private void TryAttack()
    {
        if (Time.time < _nextAttackTime) return;

        _nextAttackTime  = Time.time + attackCooldown;
        _attackTriggered = false;

        var ph = PlayerHealth.Instance ?? _player.GetComponent<PlayerHealth>();
        if (ph == null)
        {
            Debug.LogWarning("[SlimeAI] ไม่พบ PlayerHealth บน Player");
            return;
        }

        ph.TakeDamage(attackDamage);
    }


    private void OnSlimeHurt(float _)
    {
        if (_isDying || _health.IsDead) return;

        _state = State.Hurt;
        _rb.linearVelocity = Vector2.zero;
        _animator?.SetBool(AnimIsMoving, false);
        _animator?.SetTrigger(AnimHurt);
    }

    public void ExitHurtState()
    {
        if (_state == State.Hurt)
            _state = State.Idle;
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
        var col    = GetComponent<Collider2D>();
        Vector3 center = col != null ? col.bounds.center : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(BodyCenter, attackRange);
    }

#endregion
}
