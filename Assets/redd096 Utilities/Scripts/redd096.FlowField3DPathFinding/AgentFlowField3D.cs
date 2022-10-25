using UnityEngine;

namespace redd096.FlowField3DPathFinding
{
    /// <summary>
    /// Used to find path using a PathFindingFlowField and to know if still processing it
    /// </summary>
    [AddComponentMenu("redd096/.FlowField3DPathFinding/Agent FlowField 3D")]
    public class AgentFlowField3D : MonoBehaviour
    {
        bool isWaitingPath;
        PathRequest lastPathRequest;

        #region public API

        /// <summary>
        /// Calculate path, then call function passing the path as parameter. If called before receive path, will stop previous request. Call IsDone() to check when has finished
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="func">function to call when finish processing path. Will pass the path as parameter</param>
        public void FindPath(Vector3 startPosition, Vector3 targetPosition, System.Action<Path> func)
        {
            //call find path on Path Finding
            if (PathFindingFlowField3D.instance)
            {
                //if still waiting previous path, stop that request
                if (isWaitingPath)
                {
                    PathFindingFlowField3D.instance.CancelRequest(lastPathRequest);
                }

                isWaitingPath = true;                                                           //set is waiting path
                lastPathRequest = new PathRequest(startPosition, targetPosition, func, this);   //save last path request
                PathFindingFlowField3D.instance.FindPath(lastPathRequest);
            }
        }

        /// <summary>
        /// Remove last path request from queue. If already processing, do nothing
        /// </summary>
        public void CancelLastPathRequest()
        {
            //stop request
            if (PathFindingFlowField3D.instance)
            {
                if (PathFindingFlowField3D.instance.CancelRequest(lastPathRequest))
                    isWaitingPath = false;                                                              //if succeeded, set is not waiting path
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

        #endregion
    }
}