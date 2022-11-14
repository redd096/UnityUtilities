//https://www.youtube.com/watch?app=desktop&v=tSe6ZqDKB0Y

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace redd096.FlowField3DPathFinding
{
    [AddComponentMenu("redd096/.FlowField3DPathFinding/Path Finding FlowField 3D")]
    public class PathFindingFlowField3D : PathRequestManager3D
    {
        public static PathFindingFlowField3D instance;

        [Header("Default use Find Object of Type")]
        public GridFlowField3D Grid = default;

        void Awake()
        {
            instance = this;

            if (Grid == null)
                Grid = FindObjectOfType<GridFlowField3D>();
        }

        #region public API

        /// <summary>
        /// Find path to target
        /// </summary>
        /// <param name="pathRequest"></param>
        public void FindPath(PathRequest pathRequest)
        {
            //start processing path or add to queue
            RequestPath(pathRequest);
        }

        /// <summary>
        /// Remove request from queue. If request isn't in queue, or is already processing, return false
        /// </summary>
        /// <param name="pathRequest"></param>
        public bool CancelRequest(PathRequest pathRequest)
        {
            return RemoveRequestFromQueue(pathRequest);
        }

        /// <summary>
        /// Update obstacle position on the grid (used for pathfinding)
        /// </summary>
        /// <param name="obstacle"></param>
        public void UpdateObstaclePositionOnGrid(ObstacleFlowField3D obstacle)
        {
            //be sure the grid is created
            if (Grid.IsGridCreated() == false)
                Grid.BuildGrid();

            obstacle.UpdatePositionOnGrid(Grid);
        }

        #endregion

        protected override async void ProcessPath(PathRequest pathRequest)
        {
            //be sure the grid is created
            if (Grid.IsGridCreated() == false)
                Grid.BuildGrid();

            //create flow field to targets
            await Task.Run(() => Grid.SetFlowField(pathRequest.targetRequests));

            //advice agents and remove requests from the list
            await Task.Run(() => FinishProcessing(pathRequest));

            //set finished to process path
            OnFinishProcessingPath();
        }

        void FinishProcessing(PathRequest pathRequest)
        {
            //advice every agent of finished processing path, and remove path requests from the list
            foreach (PathRequest request in new List<PathRequest>(pathRequestList))
            {
                if (request.targetRequests == pathRequest.targetRequests)
                {
                    //if called from agent, call is finished to process path
                    if (request.agent)
                        request.agent.OnFinishProcessingPath(request);

                    //remove from the list
                    pathRequestList.Remove(request);
                }
            }
        }

    }
}