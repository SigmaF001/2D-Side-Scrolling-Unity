using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float     groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    // ── Runtime ──────────────────────────────────────────────
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpPressed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        CheckGrounded();
        ReadInput();
        FlipSprite(horizontalInput);
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, _rb.linearVelocity.y);

        if (jumpPressed && isGrounded)
        {
            Jump();
            jumpPressed = false;
        }
    }

    private void ReadInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float left  = (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  ? -1f : 0f;
        float right = (kb.dKey.isPressed || kb.rightArrowKey.isPressed) ?  1f : 0f;
        horizontalInput = left + right;

        if (kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
            jumpPressed = true;
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
    }

    private void CheckGrounded()
    {
        Vector2 origin = groundCheck
            ? (Vector2)groundCheck.position
            : (Vector2)transform.position;

        isGrounded = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayer);
    }

    private void FlipSprite(float dirX)
    {
        if (Mathf.Abs(dirX) < 0.01f) return;
        if (_sr != null)
                _sr.flipX = dirX < 0;
    }

    public bool  IsGrounded => isGrounded;
    public float HorizontalInput => horizontalInput;
    public bool  IsMoving => Mathf.Abs(horizontalInput) > 0.01f;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}