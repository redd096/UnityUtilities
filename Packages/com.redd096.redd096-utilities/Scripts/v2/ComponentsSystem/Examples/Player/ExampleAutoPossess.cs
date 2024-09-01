using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// Just tell to every PlayerController in scene to possess a Pawn on awake
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Examples/Example Auto Possess")]
    public class ExampleAutoPossess : MonoBehaviour
    {
        private void Awake()
        {
            //get player controllers and pawns in scene
            SimplePlayerController[] playerControllers = FindObjectsByType<SimplePlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            SimplePlayerPawn[] pawns = FindObjectsByType<SimplePlayerPawn>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            //foreach player controller
            foreach (var playerController in playerControllers)
            {
                //find first pawn without player controller and possess
                for (int i = 0; i < pawns.Length; i++)
                {
                    if (pawns[i].CurrentController == null)
                    {
                        pawns[i].Possess(playerController);
                        //pawns[i].GetComponent<ICharacter>().GetCharacterComponent<InspectorStateMachineComponent>().ResetToFirstState();
                        break;
                    }
                }

                if (playerController.CurrentPawn == null)
                {
                    Debug.LogError($"No more pawns to possess. {playerController.name} is still without a pawn", gameObject);
                }
            }
        }
    }
}