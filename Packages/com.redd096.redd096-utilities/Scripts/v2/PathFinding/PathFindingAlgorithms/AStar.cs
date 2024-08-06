using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.PathFinding
{
    public class AStar
    {
        /// <summary>
        /// Return path from start to target node
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="targetNode"></param>
        /// <param name="canMoveDiagonal"></param>
        /// <param name="returnNearestPointToTarget">If path not found, try get path to nearest node from the target</param>
        /// <returns></returns>
        public List<Vector3> FindPath(Node startNode, Node targetNode, Node[,] grid, bool canMoveDiagonal, bool returnNearestPointToTarget, IAgent agent = null)
        {
            /*
             * OPEN list - the set of nodes to be evaluated
             * CLOSED list - the set of nodes already evaluated
             * 
             * G cost - distance from start point
             * H cost - distance from end point
             * F cost - sum of G cost and H cost
             */

            //=================================================================

            /*
             * add the start node to OPEN
             * 
             * loop
             *  Current = node in OPEN with the lowest F cost
             *  remove Current from OPEN and add to CLOSED
             *  
             *  if Current is the target node (path has been found)
             *   return path
             *  
             *  foreach Neighbour of the Current node
             *   if Neighbour is not walkable or Neighbour is in CLOSED
             *      skip to the next Neighbour
             *      
             *  if new path to Neighbour is shorter OR Neighbour is not in OPEN
             *      set F cost of Neighbour
             *      set parent of Neighbour to Current
             *      if Neighbour is not in OPEN
             *          add Neighbour to OPEN
             */

            Heap<Node> openList = new Heap<Node>(grid.Length);      //nodes to be evaluated
            HashSet<Node> closedList = new HashSet<Node>();         //already evaluated

            //add the start node to OPEN
            openList.Add(startNode);

            Node currentNode = null;
            UpdateNodesStatus(grid);
            while (openList.Count > 0)
            {
                #region before heap optimization
                /*
                //Current = node in OPEN with the lowest F cost
                currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    //if F cost is lower or is the same but H cost is lower
                    if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)
                    {
                        currentNode = openList[i];
                    }
                }

                //remove Current from OPEN and add to CLOSED
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                */
                #endregion

                //Current = node in OPEN with the lowest F cost
                //remove Current from OPEN and add to CLOSED
                currentNode = openList.RemoveFirst();                //all optimized with heap
                closedList.Add(currentNode);

                //if Current is the target node (path has been found), return path
                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, currentNode);
                }

                //foreach Neighbour of the Current node
                List<Node> currentNodeNeighbours = canMoveDiagonal ? currentNode.Neighbours : currentNode.NeighboursCardinalDirections;
                foreach (Node neighbour in currentNodeNeighbours)
                {
                    //if Neighbour is not walkable or is in CLOSED, skip to next Neighbour
                    if (neighbour.IsWalkable == false || closedList.Contains(neighbour))
                        continue;

                    //if using agent and can't move on this node, skip to next Neighbour
                    if (agent != null && agent.CanMoveOnThisNode(neighbour, grid) == false)
                        continue;

                    //get distance to Neighbour
                    int newCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour, canMoveDiagonal) + neighbour.MovementPenalty;   //+ movement penalty

                    //if new path to Neighbour is shorter or Neighbour is not in OPEN
                    if (newCostToNeighbour < neighbour.GCost || openList.Contains(neighbour) == false)
                    {
                        //set F cost of Neighbour
                        neighbour.GCost = newCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode, canMoveDiagonal);

                        //set parent of Neighbour to Current
                        neighbour.ParentNode = currentNode;

                        //if Neighbour is not in OPEN, add it
                        if (openList.Contains(neighbour) == false)
                            openList.Add(neighbour);
                    }
                }
            }

            //=================================================================

            //IF NO PATH FOUND, TRY FIND PATH TO NEAREST POINT TO TARGET
            if (returnNearestPointToTarget)
            {
                //set start node because the start is not setted
                startNode.HCost = GetDistance(startNode, targetNode, canMoveDiagonal);

                //find the walkable node nearest to target point
                Node nearestNode = startNode;
                foreach (Node node in closedList)
                {
                    if (node.IsWalkable && node.HCost < nearestNode.HCost)  //if walkable and nearest to target point
                    {
                        //if using agent and can't move on this node, skip to next Neighbour
                        if (agent != null && agent.CanMoveOnThisNode(node, grid) == false)
                            continue;

                        nearestNode = node;
                    }
                }

                //find path only if nearest node is not the start node
                if (startNode != nearestNode)
                {
                    return FindPath(startNode, nearestNode, grid, canMoveDiagonal, false, agent);
                }
            }

            return null;
        }

        #region private API

        /// <summary>
        /// Recalculate status of every node (obstacles, walkable, etc...)
        /// </summary>
        /// <param name="grid"></param>
        private void UpdateNodesStatus(Node[,] grid)
        {
            foreach (Node node in grid)
            {
                node.UpdateNodeStatus();
            }
        }

        /// <summary>
        /// Calculate distance between 2 nodes
        /// </summary>
        private int GetDistance(Node nodeA, Node nodeB, bool canMoveDiagonal)
        {
            /* 
             * 
             * we use 10 from one point to another ( like 1.0 )
             * we use 14 in oblique ( like hypotenuse sqr(1.0 + 1.0) )
             * 
             * so in this example, if we have nodeA at (0,0) and nodeB at (5,2)
             * we move 2 times in oblique to reach y axis, then move 3 times along x axis
             * 14y + 10(x-y)
             * 14 *2 + 10 * (5 - 2)
             * 
             * y
             * 
             * 2  /- - - B
             * 1 / 
             * A 1 2 3 4 5  x
             * 
             */

            //get distance on X and Y
            int distX = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
            int distY = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);

            if (canMoveDiagonal)
            {
                //if distance on X is greater, move oblique (14) to reach Y axis, then move along X axis (10)
                if (distX > distY)
                    return 14 * distY + 10 * (distX - distY);

                //else move oblique (14) to reach X axis, then move along Y axis (10)
                return 14 * distX + 10 * (distY - distX);
            }
            else
            {
                //if can't move diagonal, move along X axis (10), then along Y axis (10)
                return 10 * distX + 10 * distY;
            }
        }

        /// <summary>
        /// Retrace path from start to end
        /// </summary>
        private List<Vector3> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();

            //start from the end node
            Node currentNode = endNode;

            //while not reached start node
            while (currentNode != startNode)
            {
                //add current node and move to next one
                path.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }

            //simplify path (before reverse, to be sure to take last node direction)
            List<Vector3> vectorPath = SimplifyPath(path);

            //reverse list to get from start to end
            vectorPath.Reverse();

            return vectorPath;
        }

        /// <summary>
        /// Return path as a list of Vector, and pass only nodes when change direction
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<Vector3> SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            //Vector2Int oldDirection = Vector2Int.zero;
            //
            ////calculate direction between every node in the path
            //Vector2Int newDirection;
            //for (int i = 1; i < path.Count; i++)
            //{
            //    //if direction is different, add this node position to the path
            //    newDirection = path[i - 1].gridPosition - path[i].gridPosition;
            //    if (newDirection != oldDirection)
            //    {
            //        waypoints.Add(path[i].worldPosition);
            //        oldDirection = newDirection;
            //    }
            //}

            for (int i = 0; i < path.Count; i++)
                waypoints.Add(path[i].WorldPosition);

            return waypoints;
        }

        #endregion
    }
}