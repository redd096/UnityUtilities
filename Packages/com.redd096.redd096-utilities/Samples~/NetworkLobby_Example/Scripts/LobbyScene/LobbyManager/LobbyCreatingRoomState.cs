
namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// When user click to CreateRoom, wait and show errors if necessary
    /// </summary>
    [System.Serializable]
    public class LobbyCreatingRoomState : LobbyLoadingBaseState
    {
        private bool roomCreatedWithSuccess;

        public override void Enter()
        {
            //register events
            StateMachine.Blackboard.NetTransport.OnRoomCreated += OnRoomCreated;
            StateMachine.Blackboard.NetworkManager.OnLocalClientConnect += OnLocalClientConnect;
            StateMachine.Blackboard.NetworkManager.OnLocalClientDisconnect += OnLocalClientDisconnect;

            //start loading
            base.Enter();

            //create room
            roomCreatedWithSuccess = false;
            int maxClients = StateMachine.Blackboard.NetworkManager.GetMaximumClients();
            StateMachine.Blackboard.NetTransport.CreateRoom(maxClients);
        }

        public override void Exit()
        {
            //unregister events
            StateMachine.Blackboard.NetTransport.OnRoomCreated -= OnRoomCreated;
            StateMachine.Blackboard.NetworkManager.OnLocalClientConnect -= OnLocalClientConnect;
            StateMachine.Blackboard.NetworkManager.OnLocalClientDisconnect -= OnLocalClientDisconnect;

            //stop loading
            base.Exit();
        }

        private void OnRoomCreated(bool success, string error, string roomID)
        {
            if (success)
            {
                //set success and save created room id (continue in Update)
                roomCreatedWithSuccess = true;
                StateMachine.Blackboard.CurrentRoomID = roomID;
            }
            else
            {
                //show error
                errorLabel.text = error;
                ShowObj(showLoading: false);
            }
        }

        /// <summary>
        /// After waiting minimum time, check if room is created with success
        /// </summary>
        protected override void UpdateAfterMinimumTime()
        {
            if (roomCreatedWithSuccess)
            {
                //start host (continue in OnLocalClientConnect or Disconnect)
                roomCreatedWithSuccess = false; //reset to avoid call this function more times
                StateMachine.Blackboard.NetworkManager.StartHost();
            }
        }

        private void OnLocalClientConnect()
        {
            //on success connection, change state
            StateMachine.SetState(StateMachine.ConnectedState);
        }

        private void OnLocalClientDisconnect()
        {
            //show error
            errorLabel.text = "Error: Impossible create Server";
            ShowObj(showLoading: false);
        }

        protected override void OnClickBackOnError()
        {
            //back to previous state
            StateMachine.SetState(StateMachine.OfflineState);
        }
    }
}