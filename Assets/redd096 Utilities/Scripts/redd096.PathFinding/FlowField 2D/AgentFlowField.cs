using UnityEngine;

namespace redd096.PathFinding.FlowField2D
{
    /// <summary>
    /// Used to find path using a PathFindingFlowField and to know if still processing it
    /// </summary>
    [AddComponentMenu("redd096/.PathFinding/FlowField 2D/Agent FlowField 2D")]
    public class AgentFlowField : MonoBehaviour
    {
        bool isWaitingPath;
        PathRequest lastPathRequest;

        Vector2 previousPosition;
        Node previousNode;

        public Node currentNode { get { UpdateCurrentNode(); return previousNode; } }
        public Node nextNode => PathFindingFlowField.instance.Grid.GetNodeByCoordinates(
                    currentNode.gridPosition.x + currentNode.bestDirection.x, currentNode.gridPosition.y + currentNode.bestDirection.y);
        public Vector2 nextPosition => nextNode.worldPosition;
        public Vector2 nextDirection => (nextPosition - (Vector2)transform.position).normalized;

        #region private API

        void UpdateCurrentNode()
        {
            //get current node
            if ((Vector2)transform.position != previousPosition || previousNode == null)
            {
                previousPosition = transform.position;
                previousNode = PathFindingFlowField.instance.Grid.GetNodeFromWorldPosition(previousPosition);
            }
        }

        #endregion

        #region find path

        /// <summary>
        /// Calculate path. If called before finish processing path, will stop previous request. Call IsDone() to check when has finished
        /// </summary>
        /// <param name="targetRequests"></param>
        public void FindPath(TargetRequest[] targetRequests)
        {
            //call find path on Path Finding
            if (PathFindingFlowField.instance)
            {
                //if still waiting previous path, stop that request
                if (isWaitingPath)
                {
                    PathFindingFlowField.instance.CancelRequest(lastPathRequest);
                }

                isWaitingPath = true;                                               //set is waiting path
                lastPathRequest = new PathRequest(targetRequests, this);            //save last path request
                PathFindingFlowField.instance.FindPath(lastPathRequest);
            }
        }

        /// <summary>
        /// Calculate path. If called before finish processing path, will stop previous request. Call IsDone() to check when has finished
        /// </summary>
        /// <param name="targets"></param>
        public void FindPath(Transform[] targets)
        {
            TargetRequest[] targetRequests = new TargetRequest[targets.Length];
            for (int i = 0; i < targets.Length; i++)
                targetRequests[i] = new TargetRequest(targets[i]);

            FindPath(targetRequests);
        }

        /// <summary>
        /// Calculate path. If called before finish processing path, will stop previous request. Call IsDone() to check when has finished
        /// </summary>
        /// <param name="positions"></param>
        public void FindPath(Vector2[] positions)
        {
            TargetRequest[] targetRequests = new TargetRequest[positions.Length];
            for (int i = 0; i < positions.Length; i++)
                targetRequests[i] = new TargetRequest(positions[i]);

            FindPath(targetRequests);
        }

        /// <summary>
        /// Calculate path. If called before finish processing path, will stop previous request. Call IsDone() to check when has finished
        /// </summary>
        /// <param name="targetRequest"></param>
        public void FindPath(TargetRequest targetRequest)
        {
            FindPath(new TargetRequest[1] { targetRequest });
        }

        /// <summary>
        /// Calculate path. If called before finish processing path, will stop previous request. Call IsDone() to check when has finished
        /// </summary>
        /// <param name="target"></param>
        public void FindPath(Transform target)
        {
            FindPath(new TargetRequest(target));
        }

        /// <summary>
        /// Calculate path. If called before finish processing path, will stop previous request. Call IsDone() to check when has finished
        /// </summary>
        /// <param name="position"></param>
        public void FindPath(Vector2 position)
        {
            FindPath(new TargetRequest(position));
        }

        #endregion

        /// <summary>
        /// Remove last path request from queue. If already processing, do nothing
        /// </summary>
        public void CancelLastPathRequest()
        {
            //stop request
            if (PathFindingFlowField.instance)
            {
                //if succeeded, set is not waiting path
                if (PathFindingFlowField.instance.CancelRequest(lastPathRequest))
                    isWaitingPath = false;
            }
        }

        /// <summary>
        /// Called from pathfinding, when finish processing path
        /// </summary>
        public void OnFinishProcessingPath(PathRequest pathRequest)
        {
            //if finish processing last request (if finish another request but not last, can't set isWaiting at false)
            if (pathRequest == lastPathRequest)
            {
                //set has finished to wait path
                isWaitingPath = false;
            }
        }

        /// <summary>
        /// Is not waiting path (already received or not requested)
        /// </summary>
        /// <returns></returns>
        public bool IsDone()
        {
            return !isWaitingPath;
        }
    }
}