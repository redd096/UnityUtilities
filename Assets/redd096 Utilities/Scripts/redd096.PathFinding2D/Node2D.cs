using System.Collections.Generic;
using UnityEngine;

namespace redd096.PathFinding2D
{
    public class Node2D : IHeapItem2D<Node2D>
    {
        //variables constructor
        bool isNodeWalkable;
        public bool agentCanMoveThrough;        //used by agentAStar
        public Vector2 worldPosition;
        public Vector2Int gridPosition;
        public int movementPenalty;

        //property is walkable, to check also obstacles
        public bool isWalkable => isNodeWalkable && obstaclesNotWalkable.Count <= 0;

        //variables path finding
        public int gCost;                       //distance from start point
        public int hCost;                       //distance from end point
        public int fCost => gCost + hCost;      //sum of G cost and H cost

        //used to retrace path
        public Node2D parentNode;

        //other variables
        public List<Node2D> neighbours = new List<Node2D>();
        List<ObstacleAStar2D> obstaclesOnThisNode = new List<ObstacleAStar2D>();
        List<ObstacleAStar2D> obstaclesNotWalkable = new List<ObstacleAStar2D>();

        public Node2D(bool isWalkable, bool agentCanMoveThrough, Vector2 worldPosition, int x, int y, int movementPenalty)
        {
            this.isNodeWalkable = isWalkable;
            this.agentCanMoveThrough = agentCanMoveThrough;
            this.worldPosition = worldPosition;
            this.gridPosition = new Vector2Int(x, y);
            this.movementPenalty = movementPenalty;
        }

        #region heap optimization

        public int HeapIndex { get; set; }

        public int CompareTo(Node2D nodeToCompare)
        {
            //compare F Cost, if equals, compare H Cost
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
                compare = hCost.CompareTo(nodeToCompare.hCost);

            //return negative value to check if lower
            return -compare;
        }

        #endregion

        #region obstacles

        /// <summary>
        /// Add obstacle to node (set unwalkable or add penalty)
        /// </summary>
        /// <param name="obstacle"></param>
        public void AddObstacle(ObstacleAStar2D obstacle)
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
                //or add movement penalty
                else
                {
                    movementPenalty += obstacle.AddPenalty;
                }
            }
        }

        /// <summary>
        /// Remove obstacle from node (reset walkable status or remove penalty)
        /// </summary>
        /// <param name="obstacle"></param>
        public void RemoveObstacle(ObstacleAStar2D obstacle)
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
                //or remove movement penalty
                else
                {
                    movementPenalty -= obstacle.AddPenalty;
                }
            }
        }


        /// <summary>
        /// Get obstacles on this node
        /// </summary>
        /// <returns></returns>
        public List<ObstacleAStar2D> GetObstaclesOnThisNode()
        {
            return obstaclesOnThisNode;
        }

        #endregion
    }
}