
namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// When user click to JoinRoom, wait and show errors if necessary
    /// </summary>
    [System.Serializable]
    public class LobbyJoiningRoomState : LobbyLoadingBaseState
    {
        private bool roomJoinedWithSuccess;

        public override void Enter()
        {
            //register events
            StateMachine.Blackboard.NetTransport.OnJoinedRoom += OnJoinedRoom;
            StateMachine.Blackboard.NetworkManager.OnLocalClientConnect += OnLocalClientConnect;
            StateMachine.Blackboard.NetworkManager.OnLocalClientDisconnect += OnLocalClientDisconnect;

            //start loading
            base.Enter();

            //join room (continue in OnJoinedRoom)
            roomJoinedWithSuccess = false;
            FJoinRoomData roomData = (FJoinRoomData)StateMachine.Blackboard.TempUserData;
            if (roomData.useRoomID)
            {
                //connect to a room by id
                StateMachine.Blackboard.NetTransport.JoinRoom(roomData.roomID);
            }
            else
            {
                //try connect to a room received from outside the game
                StateMachine.Blackboard.NetTransport.JoinRoomFromExternalSource(roomData.externalValue);
            }
        }

        public override void Exit()
        {
            //unregister events
            StateMachine.Blackboard.NetTransport.OnJoinedRoom -= OnJoinedRoom;
            StateMachine.Blackboard.NetworkManager.OnLocalClientConnect -= OnLocalClientConnect;
            StateMachine.Blackboard.NetworkManager.OnLocalClientDisconnect -= OnLocalClientDisconnect;

            //stop loading
            base.Exit();
        }

        private void OnJoinedRoom(bool success, string error, string roomID)
        {
            if (success)
            {
                //set success and save joined room id (continue in Update)
                roomJoinedWithSuccess = true;
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
        /// After waiting minimum time, check if room is joined with success
        /// </summary>
        protected override void UpdateAfterMinimumTime()
        {
            if (roomJoinedWithSuccess)
            {
                //client start connect to server (continue in OnLocalClientConnect or Disconnect)
                roomJoinedWithSuccess = false;  //reset to avoid call this function more times
                string roomID = StateMachine.Blackboard.CurrentRoomID;
                StateMachine.Blackboard.NetworkManager.StartClient(roomID);
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
            errorLabel.text = "Error: Impossible connect to Server";
            ShowObj(showLoading: false);
        }

        protected override void OnClickBackOnError()
        {
            //back to previous state
            StateMachine.SetState(StateMachine.OfflineState);
        }

        /// <summary>
        /// Temp variable to initialize JoinRoom state
        /// </summary>
        public struct FJoinRoomData
        {
            public bool useRoomID;
            public string roomID;
            public object externalValue;
        }
    }
}