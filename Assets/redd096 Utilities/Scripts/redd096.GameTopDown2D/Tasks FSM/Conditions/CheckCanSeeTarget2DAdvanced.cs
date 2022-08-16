using System.Collections.Generic;
using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Tasks FSM/Conditions/Check Can See Target 2D Advanced")]
    public class CheckCanSeeTarget2DAdvanced : ConditionTask
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] AimComponent aimComponent = default;

        [Header("Can See Component 2D")]
        [SerializeField] VarOrBlackboard<LayerMask> targetLayer = default;
        [SerializeField] VarOrBlackboard<float> awarenessDistance = 1;
        [Tooltip("Check if there is a wall between this object and target")][SerializeField] bool checkViewClearForAwareness = true;

        [Header("Cone Vision")]
        [SerializeField] VarOrBlackboard<float> sightDistance = 5;
        [Tooltip("Look only left and right, or use aim direction")][SerializeField] bool useOnlyLeftAndRight = false;
        [Range(1, 180)] public float viewAngle = 70f;
        [Tooltip("Check if there is a wall between this object and target")][SerializeField] bool checkViewClear = true;
        [SerializeField] VarOrBlackboard<LayerMask> layerWalls = default;
        [ColorGUI(AttributesUtility.EColor.Yellow)][SerializeField] string saveTargetInBlackboardAs = "Target";

        [Header("DEBUG")]
        [SerializeField] ShowDebugRedd096 drawAwarenessArea = Color.red;
        [SerializeField] ShowDebugRedd096 drawConeVision = Color.cyan;

        List<Transform> possibleTargets = new List<Transform>();
        List<Transform> targetsInSight = new List<Transform>();
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
            //draw cone vision
            if (drawConeVision)
            {
                Gizmos.color = drawConeVision.ColorDebug;
                Vector2 direction = aimComponent ? (useOnlyLeftAndRight ? (aimComponent.IsLookingRight ? Vector2.right : Vector2.left) : aimComponent.AimDirectionInput) : Vector2.right;
                Gizmos.matrix = Matrix4x4.TRS(transformTask.position, Quaternion.LookRotation(direction), Vector3.one);

                Gizmos.DrawFrustum(Vector3.zero, viewAngle, GetValue(sightDistance), 0, 1f);

                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.color = Color.white;
            }
        }

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //get references
            if (aimComponent == null) aimComponent = GetStateMachineComponent<AimComponent>();
        }

        public override bool OnCheckTask()
        {
            //find every targets inside awareness and sight distance
            FindPossibleTargets();

            //check if view clear to awareness targets
            if (checkViewClearForAwareness)
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

            foreach (Transform t in targetsInSight)
            {
                //check if target is in cone vision (and can see them) and add to possible targets
                if (IsInsideConeVision(t) && (checkViewClear == false || IsViewClear(t)))
                {
                    possibleTargets.Add(t);
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
            //clear lists
            possibleTargets.Clear();
            targetsInSight.Clear();

            //find every element in awareness, using layer
            foreach (Collider2D col in Physics2D.OverlapCircleAll(transformTask.position, GetValue(awarenessDistance), GetValue(targetLayer)))
            {
                possibleTargets.Add(col.transform);
            }

            //find every element in sight distance, using layer
            foreach (Collider2D col in Physics2D.OverlapCircleAll(transformTask.position, GetValue(sightDistance), GetValue(targetLayer)))
            {
                targetsInSight.Add(col.transform);
            }
        }

        bool IsViewClear(Transform t)
        {
            //check there is nothing between
            return Physics2D.Linecast(transformTask.position, t.position, GetValue(layerWalls)) == false;
        }

        bool IsInsideConeVision(Transform t)
        {
            if (aimComponent == null)
                return false;

            //check is inside view angle (use IsLookingRight to check right or left, else use aim direction input)
            return Vector2.Angle(t.position - transformTask.position, useOnlyLeftAndRight ? (aimComponent.IsLookingRight ? Vector2.right : Vector2.left) : aimComponent.AimDirectionInput) < viewAngle;
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