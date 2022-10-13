//https://www.youtube.com/watch?app=desktop&v=tSe6ZqDKB0Y

using UnityEngine;

namespace redd096.FlowField3DPathFinding
{
    [AddComponentMenu("redd096/.FlowField3DPathFinding/Path Finding FlowField 3D")]
    public class PathFindingFlowField3D : MonoBehaviour
    {
        public GridFlowField3D curFlowField;
        public GridDebug gridDebug;

        private void InitializeFlowField()
        {
            curFlowField.CreateGrid();
            gridDebug.SetFlowField(curFlowField);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                InitializeFlowField();

                curFlowField.CreateCostField();

                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
                Node3D destinationCell = curFlowField.GetCellFromWorldPos(worldMousePos);
                curFlowField.CreateIntegrationField(destinationCell);

                curFlowField.CreateFlowField();

                gridDebug.DrawFlowField();
            }
        }
    }
}