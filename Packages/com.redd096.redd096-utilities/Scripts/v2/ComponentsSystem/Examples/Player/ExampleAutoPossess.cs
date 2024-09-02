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
                        //pawns[i].GetComponent<IGameObjectRD>().GetComponentRD<InspectorStateMachineComponent>().ResetToFirstState();
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

    #region advanced example

    public class PossessPawnsClass
    {
        [SerializeField] SimplePlayerController playerControllerPrefabForEditorTest;
        [SerializeField] SimplePlayerPawn[] pawnsInOrderByPlayerIndex;

        GameObject gameObject;

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;

            var controllers = GetPlayerControllers();
            PossessPawns(controllers);
        }

        SimplePlayerController[] GetPlayerControllers()
        {
            //get every player controller in scene
            var controllers = Object.FindObjectsByType<SimplePlayerController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            //if there aren't, instantiate player controller (started directly from this scene in editor, or single player without a transition scene where instantiate player controller)
            if (controllers.Length == 0)
            {
                var controller = Object.Instantiate(playerControllerPrefabForEditorTest);
                controllers = new SimplePlayerController[] { controller };
            }

            return controllers;
        }

        void PossessPawns(SimplePlayerController[] controllers)
        {
            //be sure there are pawns to possess
            if (pawnsInOrderByPlayerIndex == null)
            {
                Debug.LogError($"Pawns are null in {gameObject.name} - we use FindObjectsOfType to get them", gameObject);
                pawnsInOrderByPlayerIndex = Object.FindObjectsByType<SimplePlayerPawn>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            }

            //foreach controller, possess a pawn
            foreach (var controller in controllers)
            {
#if ENABLE_INPUT_SYSTEM
                int index = controller.GetComponent<UnityEngine.InputSystem.PlayerInput>().playerIndex;
#else
                int index = 0;
#endif
                if (index >= 0 && index < pawnsInOrderByPlayerIndex.Length)
                {
                    SimplePlayerPawn pawn = pawnsInOrderByPlayerIndex[index];
                    if (pawn != null && pawn.CurrentController == null)
                    {
                        pawn.gameObject.SetActive(true);
                        pawn.Possess(controller);

                        //add to players list
                        //PlayersList.Add(index, pawn);
                    }
                    else
                    {
                        Debug.LogError($"Pawn for this index ({index}) is null or already possessed by another PlayerController", gameObject);
                    }
                }
                else
                {
                    Debug.LogError($"This PlayerController has a wrong index: {index}. Pawns list length is: {pawnsInOrderByPlayerIndex.Length}");
                }
            }
        }
    }

    #endregion
}