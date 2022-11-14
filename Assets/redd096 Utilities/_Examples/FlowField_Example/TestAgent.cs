using redd096.FlowField3DPathFinding;
using UnityEngine;

namespace Examples.FlowField
{
    [RequireComponent(typeof(Rigidbody), typeof(AgentFlowField3D))]
    public class TestAgent : MonoBehaviour
    {
        public float speed = 5;
        public float distanceToNode = 0.5f;

        Rigidbody rb;
        AgentFlowField3D agent;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            agent = GetComponent<AgentFlowField3D>();
        }

        private void FixedUpdate()
        {
            rb.velocity = agent.nextDirection * speed;
        }
    }
}