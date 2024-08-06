using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.PathFinding
{
    /// <summary>
    /// This is every node of the grid. They can be walkable or not walkable
    /// </summary>
    public class Node : IHeapItem<Node>
    {
        //variables constructor
        private bool isWalkable;
        private bool isOverlappable;        //If a node is out of grids, is not walkable, but characters can overlap with it (because this is not a wall, but it's a hole)?
        public Vector3 WorldPosition;
        public Vector2Int GridPosition;
        private int movementPenalty;

        //neighbours
        public List<Node> NeighboursCardinalDirections = new List<Node>();      //neighbours only up, down, right and left
        public List<Node> Neighbours = new List<Node>();                        //every neighbour, also in diagonal direction

        //obstacles
        public List<IObstacle> ObstaclesOnThisNode = new List<IObstacle>();

        //public variables (call UpdateNodeStatus before, to update these variables)
        public bool IsWalkable;
        [Tooltip("If not walkable, calculate this node as a wall (false) or a hole (true)?")] public bool IsOverlappable;
        public int MovementPenalty;

        #region variables FlowField

        public short BestCost;
        public Vector2Int BestDirection;

        #endregion

        #region variables A*

        [Tooltip("Distance from start point")] public int GCost;
        [Tooltip("Distance from end point")] public int HCost;
        [Tooltip("Sum of G cost and H cost")] public int FCost => GCost + HCost;

        [Tooltip("Used to retrace path")] public Node ParentNode;

        #endregion

        /// <summary>
        /// Set this node default values
        /// </summary>
        /// <param name="movementPenalty">It must be 1 or greater</param>
        public Node(bool isWalkable, bool isOverlappable, Vector3 worldPosition, int x, int y, int movementPenalty)
        {
            //be sure movement penalty is 1 or greater
            if (movementPenalty < 1)
                movementPenalty = 1;

            this.isWalkable = isWalkable;
            this.isOverlappable = isOverlappable;
            this.WorldPosition = worldPosition;
            this.GridPosition = new Vector2Int(x, y);
            this.movementPenalty = movementPenalty;
        }

        #region heap optimization for A*

        public int HeapIndex { get; set; }

        public int CompareTo(Node nodeToCompare)
        {
            //compare F Cost, if equals, compare H Cost
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
                compare = HCost.CompareTo(nodeToCompare.HCost);

            //return negative value to check if lower
            return -compare;
        }

        #endregion

        #region public functions

        /// <summary>
        /// Update node status (walkable, overlappable, movement penalty...) by checking obstacles and other things
        /// </summary>
        public void UpdateNodeStatus()
        {
            //check obstacles
            List<IObstacle> obstaclesNotWalkable = ObstaclesOnThisNode.FindAll(x => x.IsUnwalkable());
            int obstaclesMovementPenalty = 0;
            ObstaclesOnThisNode.ForEach(x => obstaclesMovementPenalty += x.GetMovementPenalty());

            Debug.LogWarning("ASSICURARSI CHE QUESTO FOREACH FUNZIONA PER SETTARE LA PENALTY");
            if (obstaclesMovementPenalty != 0)
            {
                Debug.LogWarning($"node {GridPosition} - obstacles penalty: {obstaclesMovementPenalty}");
            }

            //update status
            IsWalkable = isWalkable && obstaclesNotWalkable.Count <= 0;
            IsOverlappable = IsWalkable || (isOverlappable && obstaclesNotWalkable.Count <= 0);
            MovementPenalty = movementPenalty + obstaclesMovementPenalty;
        }

        #endregion
    }
}