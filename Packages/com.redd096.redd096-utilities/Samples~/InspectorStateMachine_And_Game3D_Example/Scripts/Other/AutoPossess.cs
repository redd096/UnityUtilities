using redd096.v1.Game3D;
using UnityEngine;

namespace redd096.Examples.InspectorStateMachine_And_Game3D
{
    /// <summary>
    /// Just tell to PlayerController to possess Pawn on awake
    /// </summary>
    [AddComponentMenu("redd096/Examples/InspectorStateMachine_And_Game3D/Other/Auto Possess")]
    public class AutoPossess : MonoBehaviour
    {
        [SerializeField] PlayerController controller;
        [SerializeField] PlayerPawn pawn;

        private void Awake()
        {
            controller.Possess(pawn);
        }
    }
}
