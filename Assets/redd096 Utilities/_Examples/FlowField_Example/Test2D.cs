using redd096.PathFinding.FlowField2D;
using System.Collections.Generic;
using UnityEngine;

namespace redd096.Examples.FlowField
{
    [AddComponentMenu("redd096/Examples/FlowField/Test 2D")]
    public class Test2D : MonoBehaviour
    {
        [Header("Left Click to set destinations. Right click to start pathfinding")]
        [SerializeField] float speed = 5;
        [SerializeField] GameObject[] agents;

        [Header("Targets with weights")]
        [SerializeField] TargetRequest[] targetRequests = default;

        //targets on click
        List<Vector2> targetsPosition = new List<Vector2>();

        private void OnDrawGizmos()
        {
            //draw every target request
            Gizmos.color = Color.yellow;
            if (targetRequests != null)
                foreach (TargetRequest targetRequest in targetRequests)
                    Gizmos.DrawWireCube(targetRequest.targetPosition, Vector2.one * 2);

            if (Application.isPlaying == false)
                return;

            foreach (Vector2 targetPosition in targetsPosition)
            {
                //draw click position
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(targetPosition, Vector2.one * 2);

                //draw node from click position
                Gizmos.color = Color.cyan;
                Node node = PathFindingFlowField.instance.Grid.GetNodeFromWorldPosition(targetPosition);
                Gizmos.DrawWireCube(node.worldPosition, Vector2.one * 2);
            }

            Gizmos.color = Color.white;
        }

        void Start()
        {
            //add components
            foreach (GameObject agent in agents)
            {
                Rigidbody2D rb = agent.GetComponent<Rigidbody2D>();
                if (rb == null)
                    rb = agent.AddComponent<Rigidbody2D>();

                //be sure rigidbody has no gravity
                rb.gravityScale = 0;
                rb.freezeRotation = true;

                if (agent.GetComponent<AgentFlowField>() == null)
                    agent.AddComponent<AgentFlowField>();

            }
        }

        void Update()
        {
            //left click to set targets
            if (InputRedd096.GetLeftMouseButtonDown())
            {
                Vector3 mousePos = new Vector3(InputRedd096.mousePosition.x, InputRedd096.mousePosition.y, -Camera.main.transform.position.z);
                Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

                targetsPosition.Add(worldMousePos);
            }
            //right click to start pathfinding
            else if (InputRedd096.GetRightMouseButtonDown())
            {
                //sum target requests + target positions
                TargetRequest[] requests = new TargetRequest[targetRequests.Length + targetsPosition.Count];
                for (int i = 0; i < requests.Length; i++)
                    requests[i] = i < targetRequests.Length ? targetRequests[i] : new TargetRequest(targetsPosition[i - targetRequests.Length]);

                foreach (GameObject agent in agents)
                {
                    AgentFlowField agentFlowField = agent.GetComponent<AgentFlowField>();
                    if (agentFlowField && agentFlowField.IsDone())
                        agentFlowField.FindPath(requests);
                }

                targetsPosition.Clear();
            }
        }

        void FixedUpdate()
        {
            //move every agent
            foreach (GameObject agent in agents)
            {
                Rigidbody2D rb = agent.GetComponent<Rigidbody2D>();
                AgentFlowField agentFlowField = agent.GetComponent<AgentFlowField>();

                if (rb && agentFlowField)
                    rb.velocity = agentFlowField.nextDirection * speed;
            }
        }
    }
}