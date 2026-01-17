using UnityEngine;

public class SquishySegment : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 originalScale;

    [Header("Squish Settings")]
    public float squishAmount = 0.2f; // How much it thins out
    public float stretchAmount = 0.2f; // How much it elongates
    public float lerpSpeed = 10f; // How fast it returns to normal

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        float velocityY = rb.velocity.y;

        // Calculate the target scale based on vertical velocity
        // If moving up/down fast, stretch Y and thin X
        // If hitting the ground (velocity near 0), we will handle that in OnCollisionEnter
        Vector3 targetScale = originalScale;

        if (Mathf.Abs(velocityY) > 1f)
        {
            float stretch = Mathf.Clamp(velocityY * 0.02f, -squishAmount, stretchAmount);
            targetScale = new Vector3(originalScale.x - stretch, originalScale.y + stretch, originalScale.z);
        }

        // Smoothly move toward the target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }

    // Impact squish (when hitting the floor)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > 5f)
        {
            // Flatten the circle on impact
            transform.localScale = new Vector3(originalScale.x + squishAmount, originalScale.y - squishAmount, originalScale.z);
        }
    }
}