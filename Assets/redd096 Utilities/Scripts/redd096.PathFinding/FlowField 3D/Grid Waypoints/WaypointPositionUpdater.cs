using UnityEngine;

namespace redd096.PathFinding.FlowField3D
{
    /// <summary>
    /// Used to update position of this waypoint for the grid
    /// </summary>
    [RequireComponent(typeof(WaypointFlowField))]
    [AddComponentMenu("redd096/.PathFinding/FlowField 3D/Grid Waypoints/Waypoint Position Updater 3D")]
    public class WaypointPositionUpdater : MonoBehaviour
    {
        WaypointFlowField waypoint;
        Vector3 previousPosition;

        private void Start()
        {
            waypoint = GetComponent<WaypointFlowField>();
        }

        void Update()
        {
            //if moved
            if (transform.position != previousPosition)
            {
                //update also node position on grid
                UpdatePosition();
            }
        }
        void UpdatePosition()
        {
            if (waypoint == null)
                return;

            //if there is a grid
            if (PathFindingFlowField.instance && PathFindingFlowField.instance.Grid != null && PathFindingFlowField.instance.Grid.IsGridCreated())
            {
                //and it's waypoint grid
                if (PathFindingFlowField.instance.Grid is GridFlowFieldWaypoints grid)
                {
                    //update position on node
                    previousPosition = transform.position;
                    Node node = grid.GetNodeFromWaypoint(waypoint);
                    if (node != null)
                        node.worldPosition = waypoint.transform.position;
                }
            }
        }
    }
}