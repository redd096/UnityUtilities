using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/Path Finding A Star/Path Request Manager A Star 2D")]
    public abstract class PathRequestManagerAStar2D : MonoBehaviour
    {
        struct PathRequestStruct
        {
            public Vector2 startPosition;
            public Vector2 targetPosition;
            public System.Action<List<Node2D>> func;
            public AgentAStar2D agent;
            public bool returnNearestPointToTarget;

            public PathRequestStruct(Vector2 startPosition, Vector2 targetPosition, System.Action<List<Node2D>> func, AgentAStar2D agent, bool returnNearestPointToTarget)
            {
                this.startPosition = startPosition;
                this.targetPosition = targetPosition;
                this.func = func;
                this.agent = agent;
                this.returnNearestPointToTarget = returnNearestPointToTarget;
            }
        }

        Queue<PathRequestStruct> pathRequestQueue = new Queue<PathRequestStruct>();
        PathRequestStruct currentPathRequest;
        bool isProcessingPath;

        protected void ProcessPath(Vector2 startPosition, Vector2 targetPosition, System.Action<List<Node2D>> func, AgentAStar2D agent, bool returnNearestPointToTarget)
        {
            //add path request to queue
            PathRequestStruct pathRequest = new PathRequestStruct(startPosition, targetPosition, func, agent, returnNearestPointToTarget);
            pathRequestQueue.Enqueue(pathRequest);

            //if is the first request, process it
            TryProcessNext();
        }

        protected void OnFinishProcessingPath(List<Node2D> path)
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
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                StartCoroutine(FindPathCoroutine(currentPathRequest.startPosition, currentPathRequest.targetPosition, currentPathRequest.agent, currentPathRequest.returnNearestPointToTarget));
            }
        }

        protected abstract IEnumerator FindPathCoroutine(Vector2 startPosition, Vector2 targetPosition, AgentAStar2D agent, bool returnNearestPointToTarget);
    }
}