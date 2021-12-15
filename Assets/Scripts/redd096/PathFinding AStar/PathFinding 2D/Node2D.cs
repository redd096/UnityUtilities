using UnityEngine;

namespace redd096
{
    public class Node2D : IHeapItem2D<Node2D>
    {
        //variables constructor
        public bool isWalkable;
        public bool agentCanOverlap;            //used by agentAStar
        public Vector2 worldPosition;
        public Vector2Int gridPosition;

        //variables path finding
        public int gCost;                       //distance from start point
        public int hCost;                       //distance from end point
        public int fCost => gCost + hCost;      //sum of G cost and H cost

        //used to retrace path
        public Node2D parentNode;

        public Node2D(bool isWalkable, bool agentCanOverlap, Vector2 worldPosition, int x, int y)
        {
            this.isWalkable = isWalkable;
            this.agentCanOverlap = agentCanOverlap;
            this.worldPosition = worldPosition;
            this.gridPosition = new Vector2Int(x, y);
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
    }
}