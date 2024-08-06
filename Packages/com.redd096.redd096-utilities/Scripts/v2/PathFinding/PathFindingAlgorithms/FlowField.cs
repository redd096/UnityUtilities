using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.PathFinding
{
    public class FlowField
    {
        /// <summary>
        /// Return path from start node, to best target request
        /// </summary>
        /// <param name="targetRequests"></param>
        /// <param name="grid"></param>
        /// <param name="canMoveDiagonal"></param>
        private void FindPath(FTargetRequest[] targetRequests, Node[,] grid, bool canMoveDiagonal, IAgent agent = null)
        {
            UpdateNodesStatus(grid);
            ResetFlowFieldGrid(grid);
            SetBestCosts(targetRequests, grid, agent);
            SetBestDirections(grid, canMoveDiagonal);
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
        /// Reset every node in the grid (not neighbours or penalty, just reset best cost and direction used for FlowField Pathfinding)
        /// </summary>
        /// <param name="grid"></param>
        private void ResetFlowFieldGrid(Node[,] grid)
        {
            foreach (Node node in grid)
            {
                //set default values
                node.BestCost = short.MaxValue;
                node.BestDirection = Vector2Int.zero;
            }
        }

        /// <summary>
        /// Set best cost for every node
        /// </summary>
        private void SetBestCosts(FTargetRequest[] targetRequests, Node[,] grid, IAgent agent)
        {
            foreach (FTargetRequest targetRequest in targetRequests)
            {
                //set target node at 0 or lower
                targetRequest.TargetNode.BestCost = (short)-targetRequest.Weight;

                //start from target node
                Queue<Node> cellsToCheck = new Queue<Node>();
                cellsToCheck.Enqueue(targetRequest.TargetNode);

                while (cellsToCheck.Count > 0)
                {
                    //get every neighbour in cardinal directions
                    Node currentNode = cellsToCheck.Dequeue();
                    foreach (Node currentNeighbour in currentNode.NeighboursCardinalDirections)
                    {
                        //if not walkable, ignore
                        if (currentNeighbour.IsWalkable == false) { continue; }

                        //if using agent and can't move on this node, skip to next Neighbour
                        if (agent != null && agent.CanMoveOnThisNode(currentNeighbour, grid) == false)
                            continue;

                        //else, calculate best cost
                        if (currentNeighbour.MovementPenalty + currentNode.BestCost < currentNeighbour.BestCost)
                        {
                            currentNeighbour.BestCost = (short)(currentNeighbour.MovementPenalty + currentNode.BestCost);
                            cellsToCheck.Enqueue(currentNeighbour);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set best direction for every node (from that node to best neighbour)
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="canMoveDiagonal"></param>
        private void SetBestDirections(Node[,] grid, bool canMoveDiagonal)
        {
            //foreach node
            foreach (Node currentNode in grid)
            {
                int bestCost = currentNode.BestCost;
                List<Node> currentNodeNeighbours = canMoveDiagonal ? currentNode.Neighbours : currentNode.NeighboursCardinalDirections;

                foreach (Node neighbour in currentNodeNeighbours)
                {
                    //find neighbour with lower best cost
                    if (neighbour.BestCost < bestCost)
                    {
                        //save best cost and set direction
                        bestCost = neighbour.BestCost;
                        currentNode.BestDirection = neighbour.GridPosition - currentNode.GridPosition;
                    }
                }
            }
        }

        private List<Vector3> SimplifyPath(Node startNode, Node[,] grid)
        {
            if (startNode == null)
                return null;

            //from start node
            Node currentNode = startNode;
            Vector2Int nextNodeGridPosition;
            List<Vector3> path = new List<Vector3>();

            //add every node position
            while (true)
            {
                path.Add(currentNode.WorldPosition);

                //check if this is the last node (there isn't best direction or there isn't node in direction)
                nextNodeGridPosition = currentNode.GridPosition + currentNode.BestDirection;
                if (currentNode.BestDirection == Vector2Int.zero 
                    || nextNodeGridPosition.x < 0 || nextNodeGridPosition.x >= grid.GetLength(0)
                    || nextNodeGridPosition.y < 0 || nextNodeGridPosition.y >= grid.GetLength(1))
                {
                    break;
                }

                //get next node in best direction
                currentNode = grid[nextNodeGridPosition.x, nextNodeGridPosition.y];
            }

            return path;
        }

        #endregion
    }
}