using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    public bool isFacingRight = true;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float     groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;
    private bool isGrounded;
    private float movementInput;
    private bool jumpPressed;
    private PlayerInput playerInput;

#region Unity Lifecycle
    private void Awake()
    {
        playerInput = new PlayerInput();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        CheckGrounded();
        ReadInput();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(movementInput * moveSpeed, _rb.linearVelocity.y);

        if (jumpPressed && isGrounded)
        {
            Jump();
            jumpPressed = false;
        }
    }
#endregion

#region Input Handling
    private void ReadInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        Vector2 vectorMove = playerInput.Player.Move.ReadValue<Vector2>();
        movementInput = vectorMove.x;

        if (movementInput == 0f)
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }

        if (movementInput > 0f && !isFacingRight)
        {
            FlipSprite();
        }
        else if (movementInput < 0f && isFacingRight)
        {
            FlipSprite();
        }

        if (playerInput.Player.Jump.WasPressedThisFrame())
            jumpPressed = true;

        if (playerInput.Player.Attack.WasPressedThisFrame())
            animator.SetTrigger("Attack");
    }
#endregion

#region Movement
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

    private void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public bool  IsGrounded => isGrounded;
    public float HorizontalInput => movementInput;
    public bool IsMoving => Mathf.Abs(movementInput) > 0.01f;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endregion
}