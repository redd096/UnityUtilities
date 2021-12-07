using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/Path Finding A Star/Composite Grid A Star")]
    public class CompositeGridAStar : GridAStar
    {
        List<GridAStar> gridsAStar = new List<GridAStar>();
        Vector3 gridWorldPosition;

        float leftCompositeGrid;
        float rightCompositeGrid;
        float upCompositeGrid;
        float downCompositeGrid;

        //return setted position
        public override Vector3 GridWorldPosition => gridWorldPosition;

        protected override void SetGridSize()
        {
            //get every grid in scene and update composite grid
            GetGridsAndUpdateCompositeGrid();

            base.SetGridSize();
        }

        protected override bool IsWalkable(Vector3 worldPosition)
        {
            //check is walkable, only if inside one of the grids in the array
            foreach (GridAStar gridAStar in gridsAStar)
            {
                if (gridAStar.IsInsideGrid(worldPosition))
                    return base.IsWalkable(worldPosition);
            }

            //else return false if outside of any grid
            return false;
        }

        #region private API

        void GetGridsAndUpdateCompositeGrid()
        {
            gridsAStar.Clear();
            gridWorldSize = Vector2.zero;

            //find every grid in scene
            foreach (GridAStar gridAStar in FindObjectsOfType<GridAStar>())
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

        void SetGridExtremes(GridAStar gridAStar)
        {
            //set points same as on grid parameter
            leftCompositeGrid = gridAStar.GridWorldPosition.x - (gridAStar.GridWorldSize.x / 2);
            rightCompositeGrid = gridAStar.GridWorldPosition.x + (gridAStar.GridWorldSize.x / 2);
            upCompositeGrid = (useZ ? gridAStar.GridWorldPosition.z : gridAStar.GridWorldPosition.y) + (gridAStar.GridWorldSize.y / 2);
            downCompositeGrid = (useZ ? gridAStar.GridWorldPosition.z : gridAStar.GridWorldPosition.y) - (gridAStar.GridWorldSize.y / 2);
        }

        void UpdateGridExtremes(GridAStar gridAStar)
        {
            //calculate points on grid parameter
            float left = gridAStar.GridWorldPosition.x - (gridAStar.GridWorldSize.x / 2);
            float right = gridAStar.GridWorldPosition.x + (gridAStar.GridWorldSize.x / 2);
            float up = (useZ ? gridAStar.GridWorldPosition.z : gridAStar.GridWorldPosition.y) + (gridAStar.GridWorldSize.y / 2);
            float down = (useZ ? gridAStar.GridWorldPosition.z : gridAStar.GridWorldPosition.y) - (gridAStar.GridWorldSize.y / 2);

            //check if update current points
            if (left < leftCompositeGrid)
                leftCompositeGrid = left;
            if (right > rightCompositeGrid)
                rightCompositeGrid = right;
            if (up > upCompositeGrid)
                upCompositeGrid = up;
            if (down < downCompositeGrid)
                downCompositeGrid = down;
        }

        void UpdateGridWorldSize()
        {
            //set world center of the grid
            gridWorldPosition =
                Vector3.right * (rightCompositeGrid - (rightCompositeGrid - leftCompositeGrid) * 0.5f)                              //x
                + (useZ ? Vector3.forward : Vector3.up) * (upCompositeGrid - (upCompositeGrid - downCompositeGrid) * 0.5f);         //z or y

            //set grid world size
            gridWorldSize = new Vector2(rightCompositeGrid - leftCompositeGrid, upCompositeGrid - downCompositeGrid);
        }

        #endregion
    }
}