using System.Collections.Generic;
using UnityEngine;

namespace redd096.PathFinding3D
{
    /// <summary>
    /// Used to create a single grid using every grid in the scene
    /// </summary>
    [AddComponentMenu("redd096/.PathFinding3D/Composite Grid A Star 3D")]
    public class CompositeGridAStar3D : GridAStar3D
    {
        [Header("Composite")]
        [Tooltip("If the node we want to reach makes the agent overlap with a node outside of every grid, we want our agent to move anyway? Then set true.\n" +
            "Or maybe we prefer to calculate it like a wall? Then set false")]
        [SerializeField] bool agentCanOverlapNodesOutsideGrids = true;

        List<GridAStar3D> gridsAStar = new List<GridAStar3D>();
        Vector3 gridWorldPosition;

        float leftCompositeGrid;
        float rightCompositeGrid;
        float forwardCompositeGrid;
        float backCompositeGrid;

        //return setted position
        public override Vector3 GridWorldPosition => gridWorldPosition;

        protected override void SetGrid()
        {
            //get every grid in scene and update composite grid
            GetGridsAndUpdateCompositeGrid();

            base.SetGrid();
        }

        protected override bool IsWalkable(Vector3 worldPosition, out bool agentCanMoveThrough)
        {
            //check is walkable, only if inside one of the grids in the array
            foreach (GridAStar3D gridAStar in gridsAStar)
            {
                if (gridAStar.IsInsideGrid(worldPosition))
                    return base.IsWalkable(worldPosition, out agentCanMoveThrough);
            }

            //else return false if outside of any grid (but if setted, agent can move through because is not a really wall, just is not walkable)
            agentCanMoveThrough = agentCanOverlapNodesOutsideGrids;
            return false;
        }

        #region private API

        void GetGridsAndUpdateCompositeGrid()
        {
            gridsAStar.Clear();
            gridWorldSize = Vector2.zero;

            //find every grid in scene
            foreach (GridAStar3D gridAStar in FindObjectsOfType<GridAStar3D>())
            {
                //remove self
                if (gridAStar == this)
                    continue;

                //and set or update extremes of the grid
                if (gridsAStar.Count <= 0)
                    SetGridExtremes(gridAStar);            //if this is the first grid to add, set its vars as default
                else
                    UpdateGridExtremes(gridAStar);

                //add to the list
                gridsAStar.Add(gridAStar);
            }

            //update grid world size
            UpdateGridWorldSize();
        }

        void SetGridExtremes(GridAStar3D gridAStar)
        {
            //set points same as on grid parameter
            leftCompositeGrid = gridAStar.GridWorldPosition.x - (gridAStar.GridWorldSize.x / 2);
            rightCompositeGrid = gridAStar.GridWorldPosition.x + (gridAStar.GridWorldSize.x / 2);
            forwardCompositeGrid = gridAStar.GridWorldPosition.z + (gridAStar.GridWorldSize.y / 2);
            backCompositeGrid = gridAStar.GridWorldPosition.z - (gridAStar.GridWorldSize.y / 2);
        }

        void UpdateGridExtremes(GridAStar3D gridAStar)
        {
            //calculate points on grid parameter
            float left = gridAStar.GridWorldPosition.x - (gridAStar.GridWorldSize.x / 2);
            float right = gridAStar.GridWorldPosition.x + (gridAStar.GridWorldSize.x / 2);
            float forward = gridAStar.GridWorldPosition.z + (gridAStar.GridWorldSize.y / 2);
            float back = gridAStar.GridWorldPosition.z - (gridAStar.GridWorldSize.y / 2);

            //check if update current points
            if (left < leftCompositeGrid)
                leftCompositeGrid = left;
            if (right > rightCompositeGrid)
                rightCompositeGrid = right;
            if (forward > forwardCompositeGrid)
                forwardCompositeGrid = forward;
            if (back < backCompositeGrid)
                backCompositeGrid = back;
        }

        void UpdateGridWorldSize()
        {
            //set world center of the grid
            gridWorldPosition =
                Vector3.right * (rightCompositeGrid - (rightCompositeGrid - leftCompositeGrid) * 0.5f)              //x
                + Vector3.forward * (forwardCompositeGrid - (forwardCompositeGrid - backCompositeGrid) * 0.5f);     //z

            //set grid world size
            gridWorldSize = new Vector2(rightCompositeGrid - leftCompositeGrid, forwardCompositeGrid - backCompositeGrid);
        }

        #endregion
    }
}