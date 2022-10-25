using redd096;
using redd096.FlowField3DPathFinding;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TestAgent[] agents;

    Vector3 worldMousePos;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(worldMousePos, Vector3.one * 2);

        Gizmos.color = Color.cyan;
        Node3D node = PathFindingFlowField3D.instance.Grid.GetNodeFromWorldPosition(worldMousePos);
        Gizmos.DrawWireCube(node.worldPosition, Vector3.one * 2);
    }

    void Update()
    {
        if (InputRedd096.GetLeftMouseButtonDown())
        {
            Vector3 mousePos = new Vector3(InputRedd096.mousePosition.x, InputRedd096.mousePosition.y, Camera.main.transform.position.y);
            worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

            foreach (TestAgent agent in agents)
            {
                AgentFlowField3D agentFlowField3D = agent.GetComponent<AgentFlowField3D>();
                if (agentFlowField3D && agentFlowField3D.IsDone())
                    agentFlowField3D.FindPath(agent.transform.position, worldMousePos, agent.OnPathFound);
            }
        }
    }
}
