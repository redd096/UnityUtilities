//https://www.youtube.com/watch?app=desktop&v=tSe6ZqDKB0Y

using System.Collections;
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

        //obstacles
        Coroutine updateObstaclePositionOnGridCoroutine;
        Queue<ObstacleFlowField3D> obstaclesQueue = new Queue<ObstacleFlowField3D>();

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
            //add to queue
            if (obstaclesQueue.Contains(obstacle) == false)
                obstaclesQueue.Enqueue(obstacle);

            //start coroutine if not already running
            if (updateObstaclePositionOnGridCoroutine == null)
                updateObstaclePositionOnGridCoroutine = StartCoroutine(UpdateObstaclePositionOnGridCoroutine());
        }

        #endregion

        protected override async void ProcessPath(PathRequest pathRequest)
        {
            //be sure the grid is created
            if (Grid.IsGridCreated() == false)
                Grid.BuildGrid();

            //create flow field to target position
            Node3D targetNode = Grid.GetNodeFromWorldPosition(pathRequest.targetPosition);

            await Task.Run(() => Grid.SetFlowField(targetNode));

            //start coroutine to set path for everyone
            StartCoroutine(SetPathsCoroutine(pathRequest));
        }

        IEnumerator SetPathsCoroutine(PathRequest pathRequest)
        {
            //set finished every path request with same target
            foreach (PathRequest request in new List<PathRequest>(pathRequestList))
            {
                if (request.targetPosition == pathRequest.targetPosition)
                {
                    //if called from agent, call is finished to process path
                    if (request.agent)
                        request.agent.OnFinishProcessingPath(request);

                    //call function passing the path as parameter
                    request.func?.Invoke(SetPath(request));

                    //remove from the list
                    pathRequestList.Remove(request);
                }
            }

            yield return null;

            //set finished to process path
            OnFinishProcessingPath();
        }

        Path SetPath(PathRequest request)
        {
            //from start node
            List<Vector3> vectorPath = new List<Vector3>();
            Node3D currentNode = Grid.GetNodeFromWorldPosition(request.startPosition);
            Node3D targetNode = Grid.GetNodeFromWorldPosition(request.targetPosition);

            //add every node until last one
            while (currentNode != targetNode && currentNode.bestDirection != Vector2Int.zero)
            {
                currentNode = Grid.GetNodeByCoordinates(currentNode.gridPosition.x + currentNode.bestDirection.x, currentNode.gridPosition.y + currentNode.bestDirection.y);
                vectorPath.Add(currentNode.worldPosition);
            }

            //return path
            return new Path(vectorPath);
        }

        /// <summary>
        /// Update every position of every obstacle in queue
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateObstaclePositionOnGridCoroutine()
        {
            while (obstaclesQueue.Count > 0)
            {
                //get obstacle from queue and update its position
                ObstacleFlowField3D obstacle = obstaclesQueue.Dequeue();
                if (obstacle)
                    obstacle.UpdatePositionOnGrid(Grid);

                yield return null;
            }

            updateObstaclePositionOnGridCoroutine = null;
        }
    }
}