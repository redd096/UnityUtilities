using redd096;
using redd096.PathFinding.FlowField3D;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.FlowField
{
    public class Test : MonoBehaviour
    {
        [Header("Left Click to set destinations. Right click to start pathfinding")]
        [SerializeField] TestAgent[] agents;

        [Header("Target with weights")]
        [SerializeField] TargetRequest[] targetRequests;

        Vector3 worldMousePos;
        List<Vector3> targetsPosition = new List<Vector3>();

        private void OnDrawGizmos()
        {
            //draw every target request
            Gizmos.color = Color.yellow;
            foreach (TargetRequest targetRequest in targetRequests)
                Gizmos.DrawWireCube(targetRequest.targetPosition, Vector3.one * 2);

            if (Application.isPlaying == false)
                return;

            //draw click position
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(worldMousePos, Vector3.one * 2);

            //draw node from click position
            Gizmos.color = Color.cyan;
            Node3D node = PathFindingFlowField3D.instance.Grid.GetNodeFromWorldPosition(worldMousePos);
            Gizmos.DrawWireCube(node.worldPosition, Vector3.one * 2);

            Gizmos.color = Color.white;
        }

        void Update()
        {
            //left click to set targets
            if (InputRedd096.GetLeftMouseButtonDown())
            {
                Vector3 mousePos = new Vector3(InputRedd096.mousePosition.x, InputRedd096.mousePosition.y, Camera.main.transform.position.y);
                worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

                targetsPosition.Add(worldMousePos);
            }
            //right click to start pathfinding
            else if (InputRedd096.GetRightMouseButtonDown())
            {
                //sum target requests + target positions
                TargetRequest[] requests = new TargetRequest[targetRequests.Length + targetsPosition.Count];
                for (int i = 0; i < requests.Length; i++)
                    requests[i] = i < targetRequests.Length ? targetRequests[i] : new TargetRequest(targetsPosition[i - targetRequests.Length]);

                foreach (TestAgent agent in agents)
                {
                    AgentFlowField3D agentFlowField3D = agent.GetComponent<AgentFlowField3D>();
                    if (agentFlowField3D && agentFlowField3D.IsDone())
                        agentFlowField3D.FindPath(requests);
                }

                targetsPosition.Clear();
            }
        }
    }
}