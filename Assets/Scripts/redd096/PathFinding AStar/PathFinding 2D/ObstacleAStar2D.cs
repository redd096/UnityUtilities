using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Used to create dynamic NotWalkable nodes on the grid
    /// </summary>
    [AddComponentMenu("redd096/Path Finding A Star/Obstacle A Star 2D")]
    public class ObstacleAStar2D : MonoBehaviour
    {
        [Header("Colliders to use - default get in childrens")]
        [SerializeField] Collider2D[] collidersToUse = default;

        //vars
        Node2D node;
        GridAStar2D grid;
        List<Node2D> nodesPosition = new List<Node2D>();

        //nodes to calculate
        Node2D leftNode;
        Node2D rightNode;
        Node2D upNode;
        Node2D downNode;

        void Awake()
        {
            //get every collider
            if (collidersToUse == null || collidersToUse.Length <= 0)
                collidersToUse = GetComponentsInChildren<Collider2D>();
        }

        /// <summary>
        /// Calculate new position on the grid and update nodes
        /// </summary>
        /// <param name="grid"></param>
        public void UpdatePositionOnGrid(GridAStar2D grid)
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
            foreach (Node2D node in nodesPosition)
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
            foreach (Collider2D col in collidersToUse)
            {
                if (col == null)
                    continue;

                //set node for this collider
                node = grid.GetNodeFromWorldPosition(col.bounds.center);

                //calculate nodes
                grid.GetNodesExtremesOfABox(node,
                    new Vector2(col.bounds.center.x - col.bounds.extents.x, col.bounds.center.y - col.bounds.extents.y),
                    new Vector2(col.bounds.center.x + col.bounds.extents.x, col.bounds.center.y + col.bounds.extents.y),
                    out leftNode, out rightNode, out downNode, out upNode);

                //check every node
                Node2D nodeToCheck;
                Vector2 offsetNodeToCheck = Vector2.zero;
                for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
                {
                    for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                    {
                        //use an offset to check if node is inside also if collider not reach center of the node
                        nodeToCheck = grid.GetNodeByCoordinates(x, y);
                        if (nodeToCheck.gridPosition.x != node.gridPosition.x) offsetNodeToCheck.x = nodeToCheck.gridPosition.x > node.gridPosition.x ? -grid.NodeRadius : grid.NodeRadius;
                        if (nodeToCheck.gridPosition.y != node.gridPosition.y) offsetNodeToCheck.y = nodeToCheck.gridPosition.y > node.gridPosition.y ? -grid.NodeRadius : grid.NodeRadius;

                        //if node is inside collider
                        if (Vector2.Distance(col.ClosestPoint(nodeToCheck.worldPosition + offsetNodeToCheck), nodeToCheck.worldPosition + offsetNodeToCheck) < Mathf.Epsilon)
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