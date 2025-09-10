using TMPro;
using UnityEngine;
using UnityEngine.UI;
using redd096.StateMachine;
using redd096.Network;

namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// When user is in LobbyScene but still not connected. 
    /// Show rooms and buttons to connect
    /// </summary>
    [System.Serializable]
    public class LobbyOfflineState : IState<LobbyManagerSM>
    {
        [SerializeField] GameObject panel;
        [SerializeField] TMP_InputField usernameInputField;
        [SerializeField] TMP_Text errorLabel;
        [Space]
        [SerializeField] Button createRoomButton;
        [SerializeField] Button joinRoomButton;
        [SerializeField] TMP_InputField portInputField;
        [SerializeField] TMP_InputField roomIdInputField;
        [Space]
        [SerializeField] Transform roomsContainer;
        [SerializeField] LobbyRoomUI lobbyRoomPrefab;

        public LobbyManagerSM StateMachine { get; set; }

        private const float DELAY_SCAN_LOBBIES = 5f;
        private float scanTimer;

        public void Enter()
        {
            //update values
            usernameInputField.text = RandomizeString.GetRandomFantasyName();
            errorLabel.text = "";
            portInputField.text = StateMachine.Blackboard.NetworkManager.GetPort().ToString();
            roomIdInputField.text = StateMachine.Blackboard.NetworkManager.GetClientAddress();

            //register events
            createRoomButton.onClick.AddListener(OnClickCreateRoom);
            joinRoomButton.onClick.AddListener(OnClickJoin);
            portInputField.onValueChanged.AddListener(OnPortValueChanged);
            roomIdInputField.onValueChanged.AddListener(OnRoomIdValueChanged);

            StateMachine.Blackboard.NetTransport.OnTryJoinFromExternalSource += OnTryJoinFromExternalSource;
            StateMachine.Blackboard.NetTransport.OnReceiveRooms += OnReceiveRooms;

            //show panel
            panel.SetActive(true);
        }

        public void UpdateState()
        {
            //scan rooms every few seconds
            if (Time.time > scanTimer)
            {
                scanTimer = Time.time + DELAY_SCAN_LOBBIES;
                StateMachine.Blackboard.NetTransport.RequestRoomsList();
            }
        }

        public void Exit()
        {
            //unregister events
            createRoomButton.onClick.RemoveListener(OnClickCreateRoom);
            joinRoomButton.onClick.RemoveListener(OnClickJoin);
            portInputField.onValueChanged.RemoveListener(OnPortValueChanged);
            roomIdInputField.onValueChanged.RemoveListener(OnRoomIdValueChanged);

            StateMachine.Blackboard.NetTransport.OnTryJoinFromExternalSource -= OnTryJoinFromExternalSource;
            StateMachine.Blackboard.NetTransport.OnReceiveRooms -= OnReceiveRooms;

            //hide panel
            panel.SetActive(false);
        }

        #region ui events

        private void OnClickCreateRoom()
        {
            //change state (create room)
            StateMachine.SetState(StateMachine.CreatingRoomState);
        }

        private void OnClickJoin()
        {
            //change state (join room by id)
            StateMachine.Blackboard.TempUserData = new LobbyJoiningRoomState.FJoinRoomData()
            {
                useRoomID = true,
                roomID = roomIdInputField.text,
            };
            StateMachine.SetState(StateMachine.JoiningRoomState);
        }

        private void OnPortValueChanged(string value)
        {
            //change port
            if (ushort.TryParse(value, out ushort result))
                StateMachine.Blackboard.NetworkManager.SetPort(result);
            else
                errorLabel.text = "Wrong Port";
        }

        private void OnRoomIdValueChanged(string value)
        {
            //change roomID
            StateMachine.Blackboard.NetworkManager.SetClientAddress(value);
        }

        #endregion

        #region transport events

        private void OnTryJoinFromExternalSource(object value)
        {
            //change state (join room by external source)
            StateMachine.Blackboard.TempUserData = new LobbyJoiningRoomState.FJoinRoomData()
            {
                useRoomID = false,
                externalValue = value,
            };
            StateMachine.SetState(StateMachine.JoiningRoomState);
        }

        private void OnReceiveRooms(INetworkTransport.FRoomData[] roomDatas)
        {
            //remove previous
            for (int i = roomsContainer.childCount - 1; i >= 0; i--)
                Object.Destroy(roomsContainer.GetChild(i).gameObject);

            //add new ones
            for (int i = 0; i < roomDatas.Length; i++)
            {
                var roomData = roomDatas[i];
                LobbyRoomUI room = Object.Instantiate(lobbyRoomPrefab, roomsContainer);
                room.Init(roomData.currentPlayersCount, roomData.maxPlayersCount, roomData.roomName, roomData.roomID, id =>
                {
                    //change state (join room by id)
                    StateMachine.Blackboard.TempUserData = new LobbyJoiningRoomState.FJoinRoomData()
                    {
                        useRoomID = true,
                        roomID = id,
                    };
                    StateMachine.SetState(StateMachine.JoiningRoomState);
                });
            }
        }

        #endregion
    }
}