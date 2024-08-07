using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Just tell to PlayerController to possess Pawn on awake
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Player/Auto Possess")]
    public class AutoPossess : MonoBehaviour
    {
        private void Awake()
        {
            //get pawns in scene
            PlayerController playerController = GetComponent<PlayerController>();
            PlayerPawn[] pawns = FindObjectsOfType<PlayerPawn>();

            //find first pawn without player controller
            for (int i = 0; i < pawns.Length; i++)
            {
                if (pawns[i].CurrentController == null)
                {
                    pawns[i].Possess(playerController);
                }
            }
        }
    }
}