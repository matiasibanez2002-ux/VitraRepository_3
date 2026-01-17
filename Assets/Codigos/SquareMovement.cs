using UnityEngine;

public class SquareMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 7f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;

    [Header("Legs")]
    public ProceduralLeg[] legPairA; // Assign 2 legs
    public ProceduralLeg[] legPairB; // Assign 2 legs
    public float stepDistance = 0.6f;
    public float predictionScale = 0.2f;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool usePairA = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // 1. Movement
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // 2. Leg Management
        CheckLegs();
    }

    void CheckLegs()
    {
        // Determine which pair should be checked
        ProceduralLeg[] activePair = usePairA ? legPairA : legPairB;
        ProceduralLeg[] inactivePair = usePairA ? legPairB : legPairA;

        bool needsStep = false;

        // Check if any leg in the active pair is too far from its home
        foreach (var leg in activePair)
        {
            if (Vector2.Distance(leg.ikTarget.position, leg.homePosition.position) > stepDistance)
                needsStep = true;
        }

        // If active pair needs to move AND the other pair is already planted
        if (needsStep && !IsAnyLegMoving(inactivePair))
        {
            foreach (var leg in activePair)
            {
                // Predict where the home point will be in the future based on velocity
                Vector3 predictedPos = leg.homePosition.position + (Vector3)rb.velocity * predictionScale;

                // Raycast to find ground at predicted spot
                RaycastHit2D hit = Physics2D.Raycast(predictedPos + Vector3.up, Vector2.down, 2f, groundLayer);
                Vector3 finalPos = hit.collider ? (Vector3)hit.point : predictedPos;

                leg.MoveTo(finalPos);
            }
            usePairA = !usePairA; // Toggle pairs
        }
    }

    bool IsAnyLegMoving(ProceduralLeg[] legs)
    {
        foreach (var leg in legs) if (leg.isMoving) return true;
        return false;
    }

    bool IsGrounded() => Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
}