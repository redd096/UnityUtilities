﻿namespace redd096
{
    using UnityEngine;

    public class Node : IHeapItem<Node>
    {
        //variables constructor
        public bool isWalkable;
        public Vector3 worldPosition;
        public Vector2Int gridPosition;

        //variables path finding
        public int gCost;                       //distance from start point
        public int hCost;                       //distance from end point
        public int fCost => gCost + hCost;      //sum of G cost and H cost

        //used to retrace path
        public Node parentNode;

        public Node(bool isWalkable, Vector3 worldPosition, int x, int y)
        {
            this.isWalkable = isWalkable;
            this.worldPosition = worldPosition;
            this.gridPosition = new Vector2Int(x, y);
        }

        #region heap optimization

        public int HeapIndex { get; set; }

        public int CompareTo(Node nodeToCompare)
        {
            //compare F Cost, if equals, compare H Cost
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
                compare = hCost.CompareTo(nodeToCompare.hCost);

            //return negative value to check if lower
            return -compare;
        }

        #endregion
    }
}