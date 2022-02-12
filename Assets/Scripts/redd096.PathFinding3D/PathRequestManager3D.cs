using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace redd096.PathFinding3D
{
    [AddComponentMenu("redd096/Path Finding A Star/Path Request Manager A Star 3D")]
    public abstract class PathRequestManagerAStar3D : MonoBehaviour
    {
        struct PathRequestStruct
        {
            public Vector3 startPosition;
            public Vector3 targetPosition;
            public System.Action<List<Vector3>> func;
            public AgentAStar3D agent;
            public bool returnNearestPointToTarget;

            public PathRequestStruct(Vector3 startPosition, Vector3 targetPosition, System.Action<List<Vector3>> func, AgentAStar3D agent, bool returnNearestPointToTarget)
            {
                this.startPosition = startPosition;
                this.targetPosition = targetPosition;
                this.func = func;
                this.agent = agent;
                this.returnNearestPointToTarget = returnNearestPointToTarget;
            }
        }

        List<PathRequestStruct> pathRequestQueue = new List<PathRequestStruct>();
        PathRequestStruct currentPathRequest;
        bool isProcessingPath;

        protected void ProcessPath(Vector3 startPosition, Vector3 targetPosition, System.Action<List<Vector3>> func, AgentAStar3D agent, bool returnNearestPointToTarget)
        {
            //add path request to queue
            PathRequestStruct pathRequest = new PathRequestStruct(startPosition, targetPosition, func, agent, returnNearestPointToTarget);
            pathRequestQueue.Add(pathRequest);

            //if is the first request, process it
            TryProcessNext();
        }

        protected void OnFinishProcessingPath(List<Vector3> path)
        {
            //call function passing the path as parameter
            currentPathRequest.func?.Invoke(path);
            isProcessingPath = false;

            //if there is, start next request in the queue
            TryProcessNext();
        }

        void TryProcessNext()
        {
            //get next request from queue and start find path
            if (isProcessingPath == false && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue[0];
                pathRequestQueue.RemoveAt(0);
                isProcessingPath = true;
                StartCoroutine(FindPathCoroutine(currentPathRequest.startPosition, currentPathRequest.targetPosition, currentPathRequest.agent, currentPathRequest.returnNearestPointToTarget));
            }
        }

        protected abstract IEnumerator FindPathCoroutine(Vector3 startPosition, Vector3 targetPosition, AgentAStar3D agent, bool returnNearestPointToTarget);
    }
}