using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FlyingBirdAI : MonoBehaviour
{
    public Transform player;
    public Transform visual; // sprite child (optional)

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Required for 2D NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (player == null)
        {
            agent.SetDestination(transform.position);
            return;
        }

        agent.SetDestination(player.position);

        HandleFlip();
    }

    void HandleFlip()
    {
        if (visual == null) return;

        if (agent.velocity.x > 0.1f)
            visual.localScale = new Vector3(1, 1, 1);
        else if (agent.velocity.x < -0.1f)
            visual.localScale = new Vector3(-1, 1, 1);
    }
}