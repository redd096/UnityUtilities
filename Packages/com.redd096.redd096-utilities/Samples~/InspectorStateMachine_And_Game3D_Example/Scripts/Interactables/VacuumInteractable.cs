using redd096.Game3D;
using redd096.InspectorStateMachine;
using System.Collections.Generic;
using UnityEngine;

namespace redd096.Examples.InspectorStateMachine_And_Game3D
{
    /// <summary>
    /// This interactable can be dragged and can be used also to vacuum rigidbodies
    /// </summary>
    [AddComponentMenu("redd096/Examples/InspectorStateMachine_And_Game3D/Interactables/Vacuum Interactable")]
    public class VacuumInteractable : DraggableInteractable
    {
        [Header("Vacuum")]
        [SerializeField] Transform contactPoint;
        [SerializeField] float coneRadius = 5;
        [SerializeField] float coneDistance = 10;
        [SerializeField] float coneAngle = 35;
        [SerializeField] float vacuumSpeed = 10;
        [SerializeField] LayerMask layerToVacuum = -1;

        StateMachine interactorStateMachine;
        float timeNextCast;
        bool isVacuumActive;
        List<Rigidbody> vacuumedRigidbodies = new List<Rigidbody>();

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            //draw cone
            Gizmos.color = Color.cyan;
            ConeCast.DrawConeGizmos(transform.position, coneRadius, transform.forward, coneDistance, coneAngle, Color.yellow);
            Gizmos.color = Color.white;
        }
#endif

        public override bool OnInteract(InteractComponent interactor, RaycastHit hit, params object[] args)
        {
            bool b = base.OnInteract(interactor, hit, args);

            //set vacuum in blackboard
            interactorStateMachine = interactor.GetComponent<StateMachine>();
            if (interactorStateMachine != null)
            {
                interactorStateMachine.SetBlackboardElement("Vacuum", this);    //set reference to this vacuum
            }

            return b;
        }

        public override bool OnDismiss(InteractComponent interactor, params object[] args)
        {
            bool b = base.OnDismiss(interactor, args);

            //reset blackboard
            if (interactorStateMachine != null)
            {
                interactorStateMachine.RemoveBlackboardElement("Vacuum");       //remove reference
            }

            //be sure to stop vacuum
            StopVacuum();

            return b;
        }

        public void Vacuum()
        {
            isVacuumActive = true;

            //check which objects are in the area 
            if (Time.time > timeNextCast)
            {
                timeNextCast = Time.time + 1;

                RaycastHit[] hits = ConeCast.ConeCastAll(transform.position, coneRadius, transform.forward, coneDistance, coneAngle, layerToVacuum);
                vacuumedRigidbodies.Clear();
                foreach (RaycastHit hit in hits)
                {
                    //and add to the list
                    Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
                    if (rb && !rb.isKinematic)
                    {
                        //be sure to not hit self
                        if (rb != Rb)
                            vacuumedRigidbodies.Add(rb);
                    }
                }
            }

            //move objects to the vacuum
            foreach (var rb in vacuumedRigidbodies)
            {
                if (rb != null)
                {
                    Vector3 direction = (contactPoint.position - rb.position).normalized;
                    rb.MovePosition(rb.position + direction * vacuumSpeed * Time.fixedDeltaTime);
                }
            }
        }

        public void StopVacuum()
        {
            if (isVacuumActive)
            {
                isVacuumActive = false;
                timeNextCast = 0;

                //clear vacuum list
                vacuumedRigidbodies.Clear();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //if hit vacuum AreaDestroyer
            if (isVacuumActive)
            {
                Rigidbody rb = other.GetComponentInParent<Rigidbody>();
                if (rb != null && vacuumedRigidbodies.Contains(rb))
                {
                    //remove elements from list and destroy
                    vacuumedRigidbodies.Remove(rb);
                    Destroy(rb.gameObject);
                }
            }
        }
    }
}