using redd096.v1.PathFinding.FlowField3D;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using Input = redd096.InputNew;
#endif

namespace redd096.Examples.FlowField
{
    [AddComponentMenu("redd096/Examples/FlowField/Test 3D")]
    public class Test3D : MonoBehaviour
    {
        [Header("Left Click to set destinations. Right click to start pathfinding")]
        [SerializeField] float speed = 5;
        [SerializeField] GameObject[] agents;

        [Header("Targets with weights")]
        [SerializeField] TargetRequest[] targetRequests = default;

        //targets on click
        List<Vector3> targetsPosition = new List<Vector3>();

        private void OnDrawGizmos()
        {
            //draw every target request
            Gizmos.color = Color.yellow;
            if (targetRequests != null)
                foreach (TargetRequest targetRequest in targetRequests)
                    Gizmos.DrawWireCube(targetRequest.targetPosition, Vector3.one * 2);

            if (Application.isPlaying == false)
                return;

            foreach (Vector3 targetPosition in targetsPosition)
            {
                //draw click position
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(targetPosition, Vector3.one * 2);

                //draw node from click position
                Gizmos.color = Color.cyan;
                Node node = PathFindingFlowField.instance.Grid.GetNodeFromWorldPosition(targetPosition);
                Gizmos.DrawWireCube(node.worldPosition, Vector3.one * 2);
            }

            Gizmos.color = Color.white;
        }

        void Start()
        {
            //add components
            foreach (GameObject agent in agents)
            {
                Rigidbody rb = agent.GetComponent<Rigidbody>();
                if (rb == null)
                    rb = agent.AddComponent<Rigidbody>();

                //be sure rigidbody has no gravity
                rb.useGravity = false;
                rb.freezeRotation = true;

                if (agent.GetComponent<AgentFlowField>() == null)
                    agent.AddComponent<AgentFlowField>();

            }
        }

        void Update()
        {
            //left click to set targets
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

                targetsPosition.Add(worldMousePos);
            }
            //right click to start pathfinding
            else if (Input.GetMouseButtonDown(1))
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
                Rigidbody rb = agent.GetComponent<Rigidbody>();
                AgentFlowField agentFlowField = agent.GetComponent<AgentFlowField>();

                if (rb && agentFlowField)
                    rb.velocity = agentFlowField.nextDirection * speed;
            }
        }
    }
}