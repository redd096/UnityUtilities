using redd096.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace redd096.PathFinding.FlowField2D
{
    public abstract class PathRequestManager : MonoBehaviour
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

                //save transform positions, because can't use transform.position in async functions, and ProcessPath will be async
                for (int i = 0; i < currentPathRequest.targetRequests.Length; i++)
                    currentPathRequest.targetRequests[i].SavePosition();

                ProcessPath(currentPathRequest);
            }
        }

        protected abstract void ProcessPath(PathRequest pathRequest);
    }

    #region classes

    public class PathRequest
    {
        public TargetRequest[] targetRequests;
        public AgentFlowField agent;

        /// <summary>
        /// Used to request a path to PathFinding
        /// </summary>
        /// <param name="targetRequests">Targets and/or positions to reach</param>
        /// <param name="agent"></param>
        public PathRequest(TargetRequest[] targetRequests, AgentFlowField agent = null)
        {
            this.targetRequests = targetRequests;
            this.agent = agent;
        }

        /// <summary>
        /// Used to request a path to PathFinding
        /// </summary>
        /// <param name="targets">Targets to reach</param>
        /// <param name="agent"></param>
        public PathRequest(Transform[] targets, AgentFlowField agent = null)
        {
            TargetRequest[] targetRequests = new TargetRequest[targets.Length];
            for (int i = 0; i < targets.Length; i++)
                targetRequests[i] = new TargetRequest(targets[i]);

            new PathRequest(targetRequests, agent);
        }

        /// <summary>
        /// Used to request a path to PathFinding
        /// </summary>
        /// <param name="positions">Positions to reach</param>
        /// <param name="agent"></param>
        public PathRequest(Vector2[] positions, AgentFlowField agent = null)
        {
            TargetRequest[] targetRequests = new TargetRequest[positions.Length];
            for (int i = 0; i < positions.Length; i++)
                targetRequests[i] = new TargetRequest(positions[i]);

            new PathRequest(targetRequests, agent);
        }

        /// <summary>
        /// Used to request a path to PathFinding
        /// </summary>
        /// <param name="target">Target to reach</param>
        /// <param name="agent"></param>
        public PathRequest(TargetRequest targetRequest, AgentFlowField agent = null)
        {
            new PathRequest(new TargetRequest[1] { targetRequest }, agent);
        }

        /// <summary>
        /// Used to request a path to PathFinding
        /// </summary>
        /// <param name="target">Target to reach</param>
        /// <param name="agent"></param>
        public PathRequest(Transform target, AgentFlowField agent = null)
        {
            new PathRequest(new TargetRequest(target), agent);
        }

        /// <summary>
        /// Used to request a path to PathFinding
        /// </summary>
        /// <param name="position">Position to reach</param>
        /// <param name="agent"></param>
        public PathRequest(Vector2 position, AgentFlowField agent = null)
        {
            new PathRequest(new TargetRequest(position), agent);
        }
    }

    [System.Serializable]
    public struct TargetRequest
    {
        [SerializeField] Transform target;
        [SerializeField][EnableIf("target", null, EnableIfAttribute.EComparisonType.isEqual)] Vector2 position;

        public short weight;
        public Vector2 targetPosition => target != null ? target.position : position;

        //Transform position, saved before processing path, to use in async functions
        public Vector2 savedPosition { get; private set; }
        public void SavePosition() => savedPosition = targetPosition;

        /// <summary>
        /// Set a target and its weight. More weight is equivalent to more importance
        /// </summary>
        /// <param name="target"></param>
        /// <param name="weight"></param>
        public TargetRequest(Transform target, short weight = 0)
        {
            this.target = target;
            position = default;
            this.weight = weight;
            savedPosition = target ? target.position : default;
        }

        /// <summary>
        /// Set a target and its weight. More weight is equivalent to more importance
        /// </summary>
        /// <param name="position"></param>
        /// <param name="weight"></param>
        public TargetRequest(Vector2 position, short weight = 0)
        {
            target = null;
            this.position = position;
            this.weight = weight;
            savedPosition = position;
        }
    }

    #endregion
}