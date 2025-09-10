using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using redd096.StateMachine;
using redd096.Network;

namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// When user is in LobbyScene and connected to a room. 
    /// Show players and buttons to start game
    /// </summary>
    [System.Serializable]
    public class LobbyConnectedState : IState<LobbyManagerSM>
    {
        [SerializeField] GameObject panel;
        [SerializeField] TMP_Text roomIdLabel;
        [SerializeField] Button leaveRoomButton;
        [Space]
        [SerializeField] Transform playersContainer;
        [SerializeField] LobbyPlayerUI lobbyPlayerPrefab;

        public LobbyManagerSM StateMachine { get; set; }

        public void Enter()
        {
            //update values
            roomIdLabel.text = $"Room ID: {StateMachine.Blackboard.CurrentRoomID}";
            UpdatePlayersList();

            //register events
            leaveRoomButton.onClick.AddListener(OnClickLeaveRoom);
            StateMachine.Blackboard.NetworkManager.OnServerClientConnect += OnServerClientConnect;
            StateMachine.Blackboard.NetworkManager.OnServerClientDisconnect += OnServerClientDisconnect;

            //show panel
            panel.SetActive(true);
        }

        public void UpdateState()
        {
        }

        public void Exit()
        {
            //unregister events
            leaveRoomButton.onClick.RemoveListener(OnClickLeaveRoom);
            StateMachine.Blackboard.NetworkManager.OnServerClientConnect -= OnServerClientConnect;
            StateMachine.Blackboard.NetworkManager.OnServerClientDisconnect -= OnServerClientDisconnect;

            //hide panel
            panel.SetActive(false);
        }

        private void OnClickLeaveRoom()
        {
            //go to leave state
            StateMachine.SetState(StateMachine.LeavingState);
        }

        private void OnServerClientConnect(int clientConnectionId, INetworkManager.RemoteClientData clientData)
        {
            //when a user connect, update list
            UpdatePlayersList();
        }

        private void OnServerClientDisconnect(int clientConnectionId, INetworkManager.RemoteClientData clientData)
        {
            //when a user leave, update list
            UpdatePlayersList();
        }

        private void UpdatePlayersList()
        {
            //remove previous
            for (int i = playersContainer.childCount - 1; i >= 0; i--)
                Object.Destroy(playersContainer.GetChild(i).gameObject);

            //add new ones
            Dictionary<int, INetworkManager.RemoteClientData> players = StateMachine.Blackboard.NetworkManager.Clients;
            foreach (var keypair in players)
            {
                LobbyPlayerUI player = Object.Instantiate(lobbyPlayerPrefab, playersContainer);
                player.Init(Color.red, $"ConnectionID: [{keypair.Key}] {keypair.Value}");
            }
        }
    }
}