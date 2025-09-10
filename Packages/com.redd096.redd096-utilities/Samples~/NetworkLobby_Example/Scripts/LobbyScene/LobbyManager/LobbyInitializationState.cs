using System.Collections;
using UnityEngine;
using redd096.Network;

namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// On load scene, start with a Loading to wait Managers to be initialized
    /// </summary>
    [System.Serializable]
    public class LobbyInitializationState : LobbyLoadingBaseState
    {
        private bool onCompleteMinimumTime;
        private Coroutine initializationCoroutine;

        public override void Enter()
        {
            base.Enter();

            //start coroutine
            onCompleteMinimumTime = false;
            initializationCoroutine = StateMachine.StartCoroutine(InitializationCoroutine());
        }

        public override void Exit()
        {
            base.Exit();

            //stop coroutine
            if (initializationCoroutine != null)
                StateMachine.StopCoroutine(initializationCoroutine);
        }

        /// <summary>
        /// After waiting minimum time, set boolean to true
        /// </summary>
        protected override void UpdateAfterMinimumTime()
        {
            onCompleteMinimumTime = true;
        }

        protected override void OnClickBackOnError()
        {
            //exit and re-enter in this state to restart Initialization
            StateMachine.SetState(StateMachine.InitializationState);
        }

        private IEnumerator InitializationCoroutine()
        {
            //initialize Network Manager (fishnet, mirror, etc...)
            //and be sure it's initialized before continue
            StateMachine.Blackboard.NetworkManager = INetworkManager.GenerateNetworkManager();
            yield return StateMachine.Blackboard.NetworkManager.Initialize(forceRetryOnError: false, x => SetErrorInitializationLabel(x));
            yield return new WaitUntil(() => StateMachine.Blackboard.NetworkManager != null && StateMachine.Blackboard.NetworkManager.IsInitialized());

            //generate Network Transport and initialize it too (e.g. Steam, EpicOnlineServices, etc...)
            StateMachine.Blackboard.NetTransport = StateMachine.Blackboard.NetworkManager.GenerateNetworkTransport();
            yield return StateMachine.Blackboard.NetTransport.Initialize(forceRetryOnError: false, x => SetErrorInitializationLabel(x));
            yield return new WaitUntil(() => StateMachine.Blackboard.NetTransport != null && StateMachine.Blackboard.NetTransport.IsInitialized());

            //on complete, be sure minimum time has passed
            yield return new WaitUntil(() => onCompleteMinimumTime);

            //then change state
            StateMachine.SetState(StateMachine.OfflineState);
        }

        private void SetErrorInitializationLabel(string error)
        {
            //set error label when initialization fail
            errorLabel.text = error;
            ShowObj(showLoading: false);
        }
    }
}