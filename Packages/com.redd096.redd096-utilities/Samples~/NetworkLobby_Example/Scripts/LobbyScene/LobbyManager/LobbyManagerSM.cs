using redd096.StateMachine;
using UnityEngine;

namespace redd096.Examples.NetworkLobby
{
    [AddComponentMenu("redd096/Examples/NetworkLobby/Lobby Manager StateMachine")]
    public class LobbyManagerSM : StateMachine<LobbyManagerSM>
    {
        //states
        public LobbyInitializationState InitializationState;
        public LobbyOfflineState OfflineState;
        public LobbyCreatingRoomState CreatingRoomState;
        public LobbyJoiningRoomState JoiningRoomState;
        public LobbyConnectedState ConnectedState;
        public LobbyLeavingState LeavingState;

        //blackboard
        public LobbyManagerBlackboard Blackboard = new LobbyManagerBlackboard();

        protected override void Awake()
        {
            base.Awake();

            //set default state
            SetState(InitializationState);
        }
    }
}