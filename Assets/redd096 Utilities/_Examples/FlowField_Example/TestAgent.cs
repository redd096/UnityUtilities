using redd096.PathFinding.FlowField3D;
using UnityEngine;

namespace Examples.FlowField
{
    [RequireComponent(typeof(Rigidbody), typeof(AgentFlowField))]
    public class TestAgent : MonoBehaviour
    {
        public float speed = 5;

        Rigidbody rb;
        AgentFlowField agent;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            agent = GetComponent<AgentFlowField>();
        }

        private void FixedUpdate()
        {
            rb.velocity = agent.nextDirection * speed;
        }
    }
}