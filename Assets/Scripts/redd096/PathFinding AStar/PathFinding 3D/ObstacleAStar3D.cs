using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Used to create dynamic NotWalkable nodes on the grid
    /// </summary>
    [AddComponentMenu("redd096/Path Finding A Star/Obstacle A Star 3D")]
    public class ObstacleAStar3D : MonoBehaviour
    {
        [Header("Colliders to use - default get in childrens")]
        [SerializeField] Collider[] collidersToUse = default;

        //vars
        Node3D node;
        GridAStar3D grid;
        List<Node3D> nodesPosition = new List<Node3D>();

        //nodes to calculate
        Node3D leftNode;
        Node3D rightNode;
        Node3D forwardNode;
        Node3D backNode;

        void Awake()
        {
            //get every collider
            if (collidersToUse == null || collidersToUse.Length <= 0)
                collidersToUse = GetComponentsInChildren<Collider>();
        }

        /// <summary>
        /// Calculate new position on the grid and update nodes
        /// </summary>
        /// <param name="grid"></param>
        public void UpdatePositionOnGrid(GridAStar3D grid)
        {
            if (grid == null)
                return;

            //set vars
            this.grid = grid;

            //update nodes
            RemoveFromPreviousNodes();
            SetNewNodes();
        }

        #region private API

        void RemoveFromPreviousNodes()
        {
            //remove this from previous nodes
            foreach (Node3D node in nodesPosition)
            {
                if (node.obstaclesOnThisNode.Contains(this))
                    node.obstaclesOnThisNode.Remove(this);
            }

            //clear list
            nodesPosition.Clear();
        }

        void SetNewNodes()
        {
            //foreach collider
            foreach (Collider col in collidersToUse)
            {
                if (col == null)
                    continue;

                //set node for this collider
                node = grid.GetNodeFromWorldPosition(col.bounds.center);

                //calculate nodes
                grid.GetNodesExtremesOfABox(node,
                    new Vector2(col.bounds.center.x - col.bounds.extents.x, col.bounds.center.z - col.bounds.extents.z),
                    new Vector2(col.bounds.center.x + col.bounds.extents.x, col.bounds.center.z + col.bounds.extents.z),
                    out leftNode, out rightNode, out backNode, out forwardNode);

                //check every node
                Node3D nodeToCheck;
                Vector3 offsetNodeToCheck = Vector3.zero;
                for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
                {
                    for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
                    {
                        //use an offset to check if node is inside also if collider not reach center of the node
                        nodeToCheck = grid.GetNodeByCoordinates(x, y);
                        if (nodeToCheck.gridPosition.x != node.gridPosition.x) offsetNodeToCheck.x = nodeToCheck.gridPosition.x > node.gridPosition.x ? -grid.NodeRadius : grid.NodeRadius;
                        if (nodeToCheck.gridPosition.y != node.gridPosition.y) offsetNodeToCheck.z = nodeToCheck.gridPosition.y > node.gridPosition.y ? -grid.NodeRadius : grid.NodeRadius;

                        //if node is inside collider
                        if (Vector3.Distance(col.ClosestPoint(nodeToCheck.worldPosition + offsetNodeToCheck), nodeToCheck.worldPosition + offsetNodeToCheck) < Mathf.Epsilon)
                        {
                            //set it
                            if (nodeToCheck.obstaclesOnThisNode.Contains(this) == false)
                                nodeToCheck.obstaclesOnThisNode.Add(this);

                            //and add to the list
                            if (nodesPosition.Contains(nodeToCheck) == false)
                                nodesPosition.Add(nodeToCheck);
                        }
                    }
                }
            }
        }

        #endregion
    }
}