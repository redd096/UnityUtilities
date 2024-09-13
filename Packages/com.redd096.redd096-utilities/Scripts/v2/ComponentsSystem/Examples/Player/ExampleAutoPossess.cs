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

    [System.Serializable]
    public class PossessPawnsClass
    {
        [Tooltip("When there aren't players in scene (we started from gameplay scene in editor), instantiate a prefab")][SerializeField] bool autoInstantiateIfThereArentPlayers;
        [Tooltip("Prefab to instantiate when there aren't players in scene")][SerializeField] SimplePlayerController playerControllerPrefabForEditorTest;
        [Tooltip("Set pawns in order to possess correct pawn by every player controller")][SerializeField] SimplePlayerPawn[] pawnsInOrderByPlayerIndex;

        GameObject gameObject;

        /// <summary>
        /// Normally I call this function from a LevelManager with [DefaultExecutionOrder(-10)], so I'm sure it's called before everything in game but not before InputSystem that is at -100
        /// </summary>
        /// <param name="gameObject"></param>
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
            if (controllers.Length == 0 && autoInstantiateIfThereArentPlayers)
            {
                var controller = Object.Instantiate(playerControllerPrefabForEditorTest);
                controllers = new SimplePlayerController[] { controller };
            }

            return controllers;
        }

        void PossessPawns(SimplePlayerController[] controllers)
        {
            //be sure there are pawns to possess
            if (pawnsInOrderByPlayerIndex == null || pawnsInOrderByPlayerIndex.Length == 0)
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
                for (int i = 0; i < pawnsInOrderByPlayerIndex.Length; i++)
                {
                    if (pawnsInOrderByPlayerIndex[i].CurrentController == null)
                    {
                        index = i;
                        break;
                    }

                    if (controller.CurrentPawn == null)
                    {
                        Debug.LogError($"No more pawns to possess. {controller.name} is still without a pawn", gameObject);
                        continue;
                    }
                }
#endif
                if (index >= 0 && index < pawnsInOrderByPlayerIndex.Length)
                {
                    var pawn = pawnsInOrderByPlayerIndex[index];
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