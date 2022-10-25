using redd096.FlowField3DPathFinding;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AgentFlowField3D))]
public class TestAgent : MonoBehaviour
{
    public float speed = 5;
    public float distanceToNode = 0.5f;

    Rigidbody rb;
    Path path;

    private void OnDrawGizmos()
    {
        if (path != null && path.vectorPath != null && path.vectorPath.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < path.vectorPath.Count; i++)
            {
                Gizmos.DrawLine(path.vectorPath[i], i == 0 ? transform.position : path.vectorPath[i - 1]);
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (path != null && rb != null)
        {
            if (path.vectorPath != null && path.vectorPath.Count > 0)
            {
                rb.velocity = (path.vectorPath[0] - transform.position) * speed;

                if (Vector3.Distance(transform.position, path.vectorPath[0]) < distanceToNode)
                    path.vectorPath.RemoveAt(0);

                return;
            }
        }

        rb.velocity = Vector3.zero;
    }

    public void OnPathFound(Path path)
    {
        this.path = path;
    }
}
