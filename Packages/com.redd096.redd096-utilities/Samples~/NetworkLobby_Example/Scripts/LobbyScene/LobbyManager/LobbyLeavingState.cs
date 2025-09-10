
namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// When user was connected to a room and press Quit button
    /// </summary>
    [System.Serializable]
    public class LobbyLeavingState : LobbyLoadingBaseState
    {
        private bool roomLeavedWithSuccess;

        public override void Enter()
        {
            //register events
            StateMachine.Blackboard.NetTransport.OnLeaveRoom += OnLeaveRoom;

            //start loading
            base.Enter();

            //leave room
            string roomID = StateMachine.Blackboard.CurrentRoomID;
            StateMachine.Blackboard.NetTransport.LeaveRoom(roomID);
        }

        public override void Exit()
        {
            //unregister events
            StateMachine.Blackboard.NetTransport.OnLeaveRoom -= OnLeaveRoom;

            //stop loading
            base.Exit();
        }

        private void OnLeaveRoom(bool success, string error, string roomID)
        {
            if (success)
            {
                //set success and reset room id (continue in Update)
                roomLeavedWithSuccess = true;
                StateMachine.Blackboard.CurrentRoomID = "";
            }
            else
            {
                //show error
                errorLabel.text = error;
                ShowObj(showLoading: false);
            }
        }

        /// <summary>
        /// After waiting minimum time, check if every connection is stopped
        /// </summary>
        protected override void UpdateAfterMinimumTime()
        {
            if (roomLeavedWithSuccess)
            {
                //stop client and server connections
                roomLeavedWithSuccess = false;  //reset to avoid call this function more times
                StateMachine.Blackboard.NetworkManager.Shutdown();
            }

            //when Shutdown is completed, back to offline state
            if (StateMachine.Blackboard.NetworkManager.IsOffline)
            {
                StateMachine.SetState(StateMachine.OfflineState);
            }
        }

        protected override void OnClickBackOnError()
        {
            //back to previous state
            StateMachine.SetState(StateMachine.ConnectedState);
        }
    }
}