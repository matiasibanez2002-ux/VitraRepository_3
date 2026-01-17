using UnityEngine;

public class WormVisuals : MonoBehaviour
{
    public Transform[] segments; // Drag Head, Body, Tail here in order
    private LineRenderer line;

    void Start() => line = GetComponent<LineRenderer>();

    void Update()
    {
        line.positionCount = segments.Length;
        for (int i = 0; i < segments.Length; i++)
        {
            // Smoothly draw the line through the centers of your 3 circles
            line.SetPosition(i, segments[i].position);
        }
    }
}