﻿using System.Collections.Generic;
using UnityEngine;

public class GridAStar : MonoBehaviour
{
    #region variables

    [Header("Update")]
    [SerializeField] bool update = false;

    [Header("Layer Mask Unwalkable")]
    [SerializeField] LayerMask unwalkableMask = default;

    [Header("Grid")]
    [SerializeField] Vector2 gridWorldSize = Vector2.one;
    [SerializeField] float nodeRadius = 1;

    //grid
    Node[,] grid;

    float nodeDiameter;
    Vector2Int gridSize;

    #endregion

    void Start()
    {
        SetGridSize();
        CreateGrid();
    }

    void OnValidate()
    {
        if(update)
        {
            update = false;

            SetGridSize();
            CreateGrid();
        }
    }

    void OnDrawGizmos()
    {
        //draw area
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        //draw every node in grid
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                //set color if walkable or not
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                //Gizmos.DrawSphere(node.worldPosition, nodeRadius);
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    #region create grid

    void SetGridSize()
    {
        //set diameter for every node
        nodeDiameter = nodeRadius * 2;

        //set grid size
        gridSize.x = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSize.y = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    void CreateGrid()
    {
        //reset grid and find bottom left world position
        grid = new Node[gridSize.x, gridSize.y];
        Vector3 worldBottomLeft = transform.position + (Vector3.left * gridWorldSize.x / 2) + (Vector3.back * gridWorldSize.y / 2);

        //create grid
        for (int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                //find world position and if walkable
                Vector3 worldPosition = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool isWalkable = !(Physics.CheckSphere(worldPosition, nodeRadius, unwalkableMask));

                //set new node in grid
                grid[x, y] = new Node(isWalkable, worldPosition, x, y);
            }
        }
    }

    #endregion

    #region public API

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                //this is the node we are using as parameter
                if (x == 0 && y == 0)
                    continue;

                //find grid position
                int checkX = node.gridPosition.x + x;
                int checkY = node.gridPosition.y + y;

                //if that position is inside the grid, add to neighbours
                if(checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        //find percent
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //get coordinates from it
        int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);

        //return node
        return grid[x, y];
    }

    #endregion
}