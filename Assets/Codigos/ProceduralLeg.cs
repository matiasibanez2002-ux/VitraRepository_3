using UnityEngine;
using System.Collections;

public class ProceduralLeg : MonoBehaviour
{
    public Transform ikTarget;
    public Transform homePosition;
    public LayerMask groundLayer;

    public float stepSpeed = 8f;
    public float stepHeight = 0.4f;
    public bool isMoving { get; private set; }

    public void MoveTo(Vector3 targetPoint)
    {
        StopAllCoroutines();
        StartCoroutine(StepCoroutine(targetPoint));
    }

    IEnumerator StepCoroutine(Vector3 destination)
    {
        isMoving = true;
        Vector3 startPos = ikTarget.position;
        float normalizedTime = 0;

        while (normalizedTime < 1f)
        {
            normalizedTime += Time.deltaTime * stepSpeed;

            // Smooth step with a parabolic arc
            Vector3 currentPos = Vector3.Lerp(startPos, destination, normalizedTime);
            currentPos.y += Mathf.Sin(normalizedTime * Mathf.PI) * stepHeight;

            ikTarget.position = currentPos;
            yield return null;
        }

        ikTarget.position = destination;
        isMoving = false;
    }
}