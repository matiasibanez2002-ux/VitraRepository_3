using UnityEngine;

public class WormController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float jumpForce = 12f;
    public float climbSpeed = 5f;
    public float wallSlideSpeed = 2f;

    [Header("Wall Jump")]
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 10f;

    [Header("Detection")]
    public LayerMask groundLayer;
    public float checkRadius = 0.3f;
    public Transform groundCheck;

    [Header("Slime Segments")]
    public Transform mainBody;
    public Transform leftBlob;
    public Transform rightBlob;

    [Range(0, 20)]
    public float uprightStiffness = 10f; // Higher = less flipping/spinning

    private Rigidbody2D rb;
    private Rigidbody2D leftRB;
    private Rigidbody2D rightRB;

    public bool isGrounded;
    public bool isTouchingWall;
    public bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (leftBlob) leftRB = leftBlob.GetComponent<Rigidbody2D>();
        if (rightBlob) rightRB = rightBlob.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                ApplyJump(Vector2.up * jumpForce);
            }
            else if (isTouchingWall)
            {
                float sideDir = facingRight ? -1 : 1;
                Vector2 wallJumpVector = new Vector2(sideDir * wallJumpForceX, wallJumpForceY);
                ApplyJump(wallJumpVector);
                Flip();
            }
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX > 0 && !facingRight) Flip();
        else if (moveX < 0 && facingRight) Flip();

        // Smoothly return segments to original scale after a squish
        HandleNaturalRecovery();
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Keep the blobs from doing 360s (Self-Righting Force)
        ApplyRotationMuscle(leftRB);
        ApplyRotationMuscle(rightRB);

        if (isTouchingWall && !isGrounded)
        {
            rb.gravityScale = 0;
            if (moveY > 0.1f)
                rb.velocity = new Vector2(rb.velocity.x, moveY * climbSpeed);
            else
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
        else
        {
            rb.gravityScale = 2.5f;
            rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
        }
    }

    // Prevents the "Snowball" parts from spinning 360 degrees
    void ApplyRotationMuscle(Rigidbody2D segmentRB)
    {
        if (segmentRB == null) return;
        // Forces the Z rotation back to 0 (or the parent's rotation)
        float nextAngle = Mathf.MoveTowardsAngle(segmentRB.rotation, transform.eulerAngles.z, uprightStiffness);
        segmentRB.MoveRotation(nextAngle);
    }

    void HandleNaturalRecovery()
    {
        // Slowly return all parts to normal size (1,1,1)
        if (mainBody) mainBody.localScale = Vector3.Lerp(mainBody.localScale, Vector3.one, Time.deltaTime * 5f);
        if (leftBlob) leftBlob.localScale = Vector3.Lerp(leftBlob.localScale, Vector3.one, Time.deltaTime * 5f);
        if (rightBlob) rightBlob.localScale = Vector3.Lerp(rightBlob.localScale, Vector3.one, Time.deltaTime * 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall")) isTouchingWall = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall")) isTouchingWall = false;
    }

    private void ApplyJump(Vector2 force)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(force, ForceMode2D.Impulse);

        // Squish the snowman down and stretch him up
        Vector3 jumpScale = new Vector3(0.7f, 1.4f, 1f);
        if (mainBody) mainBody.localScale = jumpScale;
        if (leftBlob) leftBlob.localScale = jumpScale;
        if (rightBlob) rightBlob.localScale = jumpScale;
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}