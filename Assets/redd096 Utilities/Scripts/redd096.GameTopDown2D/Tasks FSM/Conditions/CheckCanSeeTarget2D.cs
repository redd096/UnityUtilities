using System.Collections.Generic;
using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Tasks FSM/Conditions/Check Can See Target 2D")]
    public class CheckCanSeeTarget2D : ConditionTask
    {
        [Header("Can See Component 2D")]
        [SerializeField] VarOrBlackboard<LayerMask> targetLayer = default;
        [SerializeField] VarOrBlackboard<float> awarenessDistance = 5;
        [Tooltip("Check if there is a wall between this object and target")][SerializeField] bool checkViewClear = true;
        [SerializeField] VarOrBlackboard<LayerMask> layerWalls = default;
        [ColorGUI(AttributesUtility.EColor.Yellow)][SerializeField] string saveTargetInBlackboardAs = "Target";

        [Header("DEBUG")]
        [SerializeField] ShowDebugRedd096 drawAwarenessArea = Color.cyan;

        List<Transform> possibleTargets = new List<Transform>();
        Transform target;
        float distance;

        void OnDrawGizmos()
        {
            //draw awareness
            if (drawAwarenessArea)
            {
                Gizmos.color = drawAwarenessArea.ColorDebug;
                Gizmos.DrawWireSphere(transformTask.position, GetValue(awarenessDistance));
                Gizmos.color = Color.white;
            }
        }

        public override bool OnCheckTask()
        {
            //find every targets inside max distance
            FindPossibleTargets();

            //check if view clear to awareness targets
            if (checkViewClear)
            {
                foreach (Transform t in new List<Transform>(possibleTargets))
                {
                    //remove targets if can't see them
                    if (IsViewClear(t) == false)
                    {
                        possibleTargets.Remove(t);
                    }
                }
            }

            if (possibleTargets.Count <= 0)
                return false;

            //if found targets, save nearest in the blackboard
            GetNearest();
            stateMachine.SetBlackboardElement(saveTargetInBlackboardAs, target);

            return true;
        }

        #region private API

        void FindPossibleTargets()
        {
            //clear list
            possibleTargets.Clear();

            //find every element in distance, using layer
            foreach (Collider2D col in Physics2D.OverlapCircleAll(transformTask.position, GetValue(awarenessDistance), GetValue(targetLayer)))
            {
                possibleTargets.Add(col.transform);
            }
        }

        bool IsViewClear(Transform t)
        {
            //check there is nothing between
            return Physics2D.Linecast(transformTask.position, t.position, GetValue(layerWalls)) == false;
        }

        void GetNearest()
        {
            distance = Mathf.Infinity;
            target = null;

            //find nearest
            foreach (Transform t in possibleTargets)
            {
                if (Vector2.Distance(transformTask.position, t.position) < distance)
                {
                    distance = Vector2.Distance(transformTask.position, t.position);
                    target = t;
                }
            }
        }

        #endregion
    }
}