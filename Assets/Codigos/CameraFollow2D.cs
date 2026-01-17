using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offsets & Smoothing")]
    public Vector3 offset = new Vector3(0, 0, -10); // -10 on Z is standard for 2D
    public float smoothTime = 0.25f;
    private Vector3 currentVelocity = Vector3.zero;

    [Header("Axis Locks")]
    public bool freezeX = false;
    public bool freezeY = true; // Set to true to stop vertical following
    public bool freezeZ = true; // Usually true in 2D

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void LateUpdate() // LateUpdate ensures the player has moved first
    {
        if (target == null) return;

        // 1. Determine where the camera WANTs to go
        Vector3 targetPosition = target.position + offset;

        // 2. Check if we should ignore certain axes
        if (freezeX) targetPosition.x = transform.position.x;
        if (freezeY) targetPosition.y = initialPosition.y;
        if (freezeZ) targetPosition.z = initialPosition.z;

        // 3. Smoothly move the camera to that position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}