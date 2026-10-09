using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private bool allowDoubleJump = true;  // Pixel Adventure trae animación de doble salto
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Detección de suelo (Tilemap)")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;        // La capa donde pusiste el Tilemap

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Si se deja vacío, se busca solo
    [SerializeField] private Animator animator;             // Si se deja vacío, se busca solo

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool canDoubleJump;
    private float coyoteCounter;
    private float jumpBufferCounter;

    // Nombres de parámetros del Animator (créalos con estos nombres)
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AnimYVelocity = Animator.StringToHash("YVelocity");
    private static readonly int AnimDoubleJump = Animator.StringToHash("DoubleJump");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            canDoubleJump = allowDoubleJump;
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        HandleJump();

        // Salto variable
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

        // Voltear sprite (Pixel Adventure mira a la derecha por defecto)
        if (spriteRenderer != null)
        {
            if (moveInput > 0) spriteRenderer.flipX = false;
            else if (moveInput < 0) spriteRenderer.flipX = true;
        }

        UpdateAnimator();
    }

    private void HandleJump()
    {
        // Salto desde el suelo (con coyote time y buffer)
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }
        // Doble salto en el aire
        else if (Input.GetButtonDown("Jump") && !isGrounded && canDoubleJump && coyoteCounter <= 0f)
        {
            Jump();
            canDoubleJump = false;
            if (animator != null) animator.SetTrigger(AnimDoubleJump);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat(AnimSpeed, Mathf.Abs(moveInput));
        animator.SetBool(AnimGrounded, isGrounded);
        animator.SetFloat(AnimYVelocity, rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}