using System.Collections.Generic;
using UnityEngine;

namespace redd096.PathFinding.FlowField2D
{
    public class Node
    {
        //variables constructor
        bool isNodeWalkable;
        public bool agentCanMoveThrough;        //used by agentAStar
        public Vector3 worldPosition;
        public Vector2Int gridPosition;
        public int movementPenalty;

        //property is walkable, to check also obstacles
        public bool isWalkable => isNodeWalkable && obstaclesNotWalkable.Count <= 0;

        //variables path finding
        public short bestCost;
        public Vector2Int bestDirection;

        //other variables
        public List<Node> neighboursCardinalDirections = new List<Node>();              //neighbours only up, down, right and left
        public List<Node> neighbours = new List<Node>();                                //every neighbour, also in diagonal direction
        List<ObstacleFlowField> obstaclesOnThisNode = new List<ObstacleFlowField>();
        List<ObstacleFlowField> obstaclesNotWalkable = new List<ObstacleFlowField>();

        public Node(bool isWalkable, bool agentCanMoveThrough, Vector3 worldPosition, int x, int y, int movementPenalty)
        {
            this.isNodeWalkable = isWalkable;
            this.agentCanMoveThrough = agentCanMoveThrough;
            this.worldPosition = worldPosition;
            this.gridPosition = new Vector2Int(x, y);
            this.movementPenalty = movementPenalty;
        }

        #region obstacles

        /// <summary>
        /// Add obstacle to node (set unwalkable or add penalty)
        /// </summary>
        /// <param name="obstacle"></param>
        public void AddObstacle(ObstacleFlowField obstacle)
        {
            //add obstacles to the list
            if (obstaclesOnThisNode.Contains(obstacle) == false)
                obstaclesOnThisNode.Add(obstacle);

            if (obstacle)
            {
                //add to list unwalkable
                if (obstacle.IsUnwalkable)
                {
                    if (obstaclesNotWalkable.Contains(obstacle) == false)
                        obstaclesNotWalkable.Add(obstacle);
                }

                //add movement penalty (if add penalty is false, penalty will be 0)
                movementPenalty += obstacle.AddPenalty;
            }
        }

        /// <summary>
        /// Remove obstacle from node (reset walkable status or remove penalty)
        /// </summary>
        /// <param name="obstacle"></param>
        public void RemoveObstacle(ObstacleFlowField obstacle)
        {
            //remove obstacles from the list
            if (obstaclesOnThisNode.Contains(obstacle))
                obstaclesOnThisNode.Remove(obstacle);

            if (obstacle)
            {
                //remove from list unwalkable
                if (obstacle.IsUnwalkable)
                {
                    if (obstaclesNotWalkable.Contains(obstacle))
                        obstaclesNotWalkable.Remove(obstacle);
                }

                //remove movement penalty (if add penalty is false, penalty will be 0)
                movementPenalty -= obstacle.AddPenalty;
            }
        }

        /// <summary>
        /// Get obstacles on this node
        /// </summary>
        /// <returns></returns>
        public List<ObstacleFlowField> GetObstaclesOnThisNode()
        {
            return obstaclesOnThisNode;
        }

        #endregion
    }
}