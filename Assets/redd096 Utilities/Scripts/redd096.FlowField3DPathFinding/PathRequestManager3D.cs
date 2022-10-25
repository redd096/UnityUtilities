using System.Collections.Generic;
using UnityEngine;

namespace redd096.FlowField3DPathFinding
{
    public abstract class PathRequestManager3D : MonoBehaviour
    {
        protected List<PathRequest> pathRequestList = new List<PathRequest>();
        PathRequest currentPathRequest;
        bool isProcessingPath;

        protected bool RemoveRequestFromQueue(PathRequest pathRequest)
        {
            //remove request from list
            if (pathRequestList.Contains(pathRequest))
            {
                pathRequestList.Remove(pathRequest);
                return true;
            }

            return false;
        }

        protected void RequestPath(PathRequest pathRequest)
        {
            //add path request to list
            pathRequestList.Add(pathRequest);

            //if it's the first request, process it
            TryProcessNext();
        }

        protected void OnFinishProcessingPath()
        {
            //set we finished to process path
            isProcessingPath = false;

            //if there is, start next request in the queue
            TryProcessNext();
        }

        void TryProcessNext()
        {
            //get next request from queue and start find path
            if (isProcessingPath == false && pathRequestList.Count > 0)
            {
                currentPathRequest = pathRequestList[0];
                isProcessingPath = true;
                ProcessPath(currentPathRequest);
            }
        }

        protected abstract void ProcessPath(PathRequest pathRequest);
    }

    #region classes

    public class PathRequest
    {
        public Vector3 startPosition;
        public Vector3 targetPosition;
        public System.Action<Path> func;
        public AgentFlowField3D agent;

        /// <summary>
        /// Struct used to request a path to PathFinding
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="func">function to call when finish processing path. Will pass the path as parameter</param>
        /// <param name="agent"></param>
        public PathRequest(Vector3 startPosition, Vector3 targetPosition, System.Action<Path> func, AgentFlowField3D agent = null)
        {
            this.startPosition = startPosition;
            this.targetPosition = targetPosition;
            this.func = func;
            this.agent = agent;
        }
    }

    public class Path
    {
        public List<Vector3> vectorPath;

        /// <summary>
        /// Struct used to pass found path
        /// </summary>
        /// <param name="vectorPath"></param>
        public Path(List<Vector3> vectorPath)
        {
            this.vectorPath = vectorPath;
        }
    }

    #endregion
}