using System.Collections.Generic;
using UnityEngine;
using redd096.Attributes;

namespace redd096.PathFinding3D
{
    /// <summary>
    /// Used to create dynamic NotWalkable nodes on the grid
    /// </summary>
    [AddComponentMenu("redd096/Path Finding A Star/Obstacle A Star 3D")]
    public class ObstacleAStar3D : MonoBehaviour
    {
        enum ETypeCollider { sphere, box }

        [Header("Collider Obstacle")]
        [SerializeField] Vector3 offset = Vector3.zero;
        [SerializeField] ETypeCollider typeCollider = ETypeCollider.box;
        [EnableIf("typeCollider", ETypeCollider.box)] [SerializeField] Vector3 sizeCollider = Vector3.one;
        [EnableIf("typeCollider", ETypeCollider.sphere)] [SerializeField] float radiusCollider = 1;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        //vars
        GridAStar3D grid;
        List<Node3D> nodesPosition = new List<Node3D>();    //nodes with this obstacle

        //nodes to calculate
        Node3D centerNode;
        Node3D leftNode;
        Node3D rightNode;
        Node3D forwardNode;
        Node3D backNode;
        Node3D nodeToCheck;

        void OnDrawGizmos()
        {
            if (drawDebug)
            {
                Gizmos.color = Color.cyan;

                //draw box
                if (typeCollider == ETypeCollider.box)
                {
                    Gizmos.DrawWireCube(transform.position + offset, sizeCollider);
                }
                //draw sphere
                else
                {
                    Gizmos.DrawWireSphere(transform.position + offset, radiusCollider);
                }

                Gizmos.color = Color.white;
            }
        }

        void Update()
        {
            //update obstacle position
            if (PathFindingAStar3D.instance)
                PathFindingAStar3D.instance.UpdateObstaclePositionOnGrid(this);
        }

        void OnDisable()
        {
            //remove obstacle from grid
            RemoveFromPreviousNodes();
        }

        #region public API

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

        /// <summary>
        /// Remove from current nodes
        /// </summary>
        public void RemoveFromPreviousNodes()
        {
            //remove this from previous nodes
            foreach (Node3D node in nodesPosition)
            {
                if (node != null && node.obstaclesOnThisNode.Contains(this))
                    node.obstaclesOnThisNode.Remove(this);
            }

            //clear list
            nodesPosition.Clear();
        }

        /// <summary>
        /// Calculate new position on the grid and add to new nodes (is better use UpdatePositionOnGrid to set grid and remove from previous nodes)
        /// </summary>
        public void SetNewNodes()
        {
            if (grid == null)
                return;

            //set nodes using box or circle
            if (typeCollider == ETypeCollider.box)
                SetNodesUsingBox();
            else
                SetNodesUsingCircle();
        }

        #endregion

        #region private API

        void SetNodesUsingBox()
        {
            //calculate nodes
            //use an offset to check if node is inside also if collider not reach center of the node (add grid.NodeRadius in the half size)
            centerNode = grid.GetNodeFromWorldPosition(transform.position + offset);
            grid.GetNodesExtremesOfABox(centerNode, transform.position + offset, (sizeCollider * 0.5f) + (Vector3.one * grid.NodeRadius), out leftNode, out rightNode, out backNode, out forwardNode);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    //set it
                    if (nodeToCheck.obstaclesOnThisNode.Contains(this) == false)
                        nodeToCheck.obstaclesOnThisNode.Add(this);

                    //and add to the list
                    if (nodesPosition.Contains(nodeToCheck) == false)
                        nodesPosition.Add(nodeToCheck);
                }
            }
        }

        void SetNodesUsingCircle()
        {
            //calculate nodes
            //use an offset to check if node is inside also if collider not reach center of the node (add grid.NodeRadius in the half size)
            centerNode = grid.GetNodeFromWorldPosition(transform.position + offset);
            grid.GetNodesExtremesOfABox(centerNode, transform.position + offset, (Vector3.one * radiusCollider) + (Vector3.one * grid.NodeRadius), out leftNode, out rightNode, out backNode, out forwardNode);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    //if inside radius (+ node radius offset)
                    if (Vector3.Distance(centerNode.worldPosition, nodeToCheck.worldPosition) <= radiusCollider + grid.NodeRadius)
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

        //void SetNodesUsingColliders()
        //{
        //    //foreach collider
        //    foreach (Collider col in GetComponentsInChildren<Collider>())
        //    {
        //        if (col == null)
        //            continue;
        //
        //        //calculate nodes
        //        //use an offset to check if node is inside also if collider not reach center of the node (add grid.NodeRadius in the half size)
        //        centerNode = grid.GetNodeFromWorldPosition(col.bounds.center);
        //        grid.GetNodesExtremesOfABox(centerNode, col.bounds.center, col.bounds.extents + (Vector3.one * grid.NodeRadius), out leftNode, out rightNode, out backNode, out forwardNode);
        //
        //        //check every node
        //        Node3D nodeToCheck;
        //        for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
        //        {
        //            for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
        //            {
        //                nodeToCheck = grid.GetNodeByCoordinates(x, y);
        //
        //                //if node is inside collider (+ node radius offset)
        //                if (Vector3.Distance(col.ClosestPoint(nodeToCheck.worldPosition), nodeToCheck.worldPosition) < Mathf.Epsilon + grid.NodeRadius)
        //                {
        //                    //set it
        //                    if (nodeToCheck.obstaclesOnThisNode.Contains(this) == false)
        //                        nodeToCheck.obstaclesOnThisNode.Add(this);
        //
        //                    //and add to the list
        //                    if (nodesPosition.Contains(nodeToCheck) == false)
        //                        nodesPosition.Add(nodeToCheck);
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}