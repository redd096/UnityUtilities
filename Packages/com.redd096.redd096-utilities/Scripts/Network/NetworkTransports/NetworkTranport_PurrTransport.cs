using System.Collections;
using UnityEngine;

#if DISABLESTEAMWORKS == false && STEAMWORKS_NET
using Steamworks;
#endif

/*
    Links:
    Fishnet's Steam transport: https://github.com/FirstGearGames/FishySteamworks/
    Multipass (Fishnet multi-transport): https://fish-networking.gitbook.io/docs/fishnet-building-blocks/transports/multipass
    Steamworks.NET: https://github.com/rlabrecque/Steamworks.NET
    SteamWorks installation instructions (heathen has varios toolkits to help with Steam integration): https://kb.heathen.group/steam/register-with-valve
    Original SteamWorks instructions: https://steamworks.github.io/installation/
    SteamMatchMaking API: https://partner.steamgames.com/doc/features/multiplayer/matchmaking 


    1. To change Steam App ID: in the project folder (parent of Assets/ folder) there is a steam_appid.txt
    1.a. Steam App ID for development is: 480
    1.b. SteamWorks.NET must be installed as Embedded Package. It means the package is to put manually inside project Packages/ folder

    2. SteamManager can be a GameObject in scene or it's created when call SteamManager.Initialized
    2.a. SteamManager installer: in Assets/FishNet/Plugins/FishySteamworks/ install SteamManager.unitypackage

    3.To use Steam as Fishnet Transport attach Fishy Steamworks to your NetworkManager
    3.a. Either remove other transports or add TransportManager and specify which transport to use.
    3.b. If you need more transports (e.g. steam + webgl) use Multipass transport
*/

#if DISABLESTEAMWORKS == false && STEAMWORKS_NET
namespace redd096.Network
{
    ///TODO - FOR NOW IT'S JUST A COPY-PASTE OF STEAM TRANSPORT
    public class NetworkTransport_PurrTransport : INetworkTransport
    {
        //consts
        private const float DELAY_BETWEEN_RETRY_INITIALIZATION = 2f;
        private const ELobbyType CREATE_LOBBY_TYPE = ELobbyType.k_ELobbyTypeFriendsOnly;
        // private const string HOST_ADDRESS_KEY = "HostAddress";

        //steam events
        private Callback<LobbyCreated_t> onLobbyCreated;
        private Callback<GameLobbyJoinRequested_t> onUserClickJoinInSteamFriendsList;
        private Callback<LobbyEnter_t> onJoinedLobby;
        private Callback<LobbyMatchList_t> onFoundLobbiesList;

        //transport events
        public System.Action<bool, string, string> OnRoomCreated { get; set; }
        public System.Action<bool, string, string> OnJoinedRoom { get; set; }
        public System.Action<object> OnTryJoinFromExternalSource { get; set; }
        public System.Action<INetworkTransport.FRoomData[]> OnReceiveRooms { get; set; }
        public System.Action<bool, string, string> OnLeaveRoom { get; set; }

        //private
        private SteamManager steamManager;
        private bool isInitialized;

        public IEnumerator Initialize(bool forceRetryOnError, System.Action<string> onError)
        {
            GameObject steamManagerObj = null;

            while (true)
            {
                //be sure SteamManager is in scene
                steamManager = Object.FindFirstObjectByType<SteamManager>();
                if (steamManager == null)
                    steamManager = steamManagerObj ? steamManagerObj.AddComponent<SteamManager>() : new GameObject("SteamManager").AddComponent<SteamManager>();

                //wait initialization (when initialized, just break this whileLoop)
                steamManagerObj = steamManager.gameObject;
                yield return new WaitForSeconds(0.1f);
                if (SteamManager.Initialized)
                    break;

                //If there is an error, print it
                string error = "Error initialization: is Steam running? It is necessary for this application to works.";
                onError?.Invoke(error);

                //destroy SteamManager on error (so it will be recreated on retry)
                Object.Destroy(steamManager);

                //if force retry, just wait few seconds before retry. Else quit
                if (forceRetryOnError)
                    yield return new WaitForSeconds(DELAY_BETWEEN_RETRY_INITIALIZATION);
                else
                    yield break;
            }

            //register events
            onLobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            onUserClickJoinInSteamFriendsList = Callback<GameLobbyJoinRequested_t>.Create(OnUserClickJoinInSteamFriendsList);
            onJoinedLobby = Callback<LobbyEnter_t>.Create(OnJoinedLobby);
            onFoundLobbiesList = Callback<LobbyMatchList_t>.Create(OnFoundLobbiesList);

            isInitialized = true;
        }

        public void Deinitialize()
        {
            //unregister events
            onLobbyCreated.Dispose();
            onUserClickJoinInSteamFriendsList.Dispose();
            onJoinedLobby.Dispose();
            onFoundLobbiesList.Dispose();

            //destroy SteamManager (so it will be recreated on initialize)
            if (steamManager)
                Object.Destroy(steamManager);

            isInitialized = false;
        }

        public bool IsInitialized()
        {
            return steamManager && SteamManager.Initialized && isInitialized;
        }

        public void CreateRoom(int maxClients)
        {
            //create steam lobby
            SteamMatchmaking.CreateLobby(CREATE_LOBBY_TYPE, maxClients);
        }

        public void JoinRoom(string roomID)
        {
            //join steam lobby
            if (ulong.TryParse(roomID, out ulong result))
                SteamMatchmaking.JoinLobby(new CSteamID(result));
            else
                OnJoinedRoom?.Invoke(false, "Error: wrong Room ID", "");
        }

        public void JoinRoomFromExternalSource(object roomToJoin)
        {
            //join lobby selected from Steam Friends List
            if (roomToJoin is GameLobbyJoinRequested_t param)
                SteamMatchmaking.JoinLobby(param.m_steamIDLobby);
            else
                OnJoinedRoom?.Invoke(false, "Error: impossible to connect to friend's room", "");
        }

        public void RequestRoomsList()
        {
            // SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
            SteamMatchmaking.RequestLobbyList();
        }

        public void LeaveRoom(string roomID)
        {
            //leave steam lobby. There isn't an event for success or fail
            if (ulong.TryParse(roomID, out ulong result))
            {
                SteamMatchmaking.LeaveLobby(new CSteamID(result));
                OnLeaveRoom?.Invoke(true, "", roomID);
            }
            else
            {
                OnLeaveRoom?.Invoke(false, "Error: wrong Room ID to leave", roomID);
            }
        }

        #region steam events

        private void OnLobbyCreated(LobbyCreated_t param)
        {
            bool success = param.m_eResult == EResult.k_EResultOK;
            string error = success ? "" : $"Error: {param.m_eResult}";
            string roomID = success ? param.m_ulSteamIDLobby.ToString() : "";

            // //set lobby to use SteamID as key
            // if (success)
            //     SteamMatchmaking.SetLobbyData(new CSteamID(param.m_ulSteamIDLobby), HOST_ADDRESS_KEY, SteamUser.GetSteamID().ToString());

            //call event
            OnRoomCreated?.Invoke(success, error, roomID);
        }

        private void OnUserClickJoinInSteamFriendsList(GameLobbyJoinRequested_t param)
        {
            //call event
            OnTryJoinFromExternalSource?.Invoke(param);
        }

        private void OnJoinedLobby(LobbyEnter_t param)
        {
            bool success = param.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess;
            string error = success ? "" : $"Error: {(EChatRoomEnterResponse)param.m_EChatRoomEnterResponse}";
            string roomID = success ? param.m_ulSteamIDLobby.ToString() : "";
            // string roomID = success ? SteamMatchmaking.GetLobbyData(new CSteamID(param.m_ulSteamIDLobby), HOST_ADDRESS_KEY) : "";

            //call event
            OnJoinedRoom?.Invoke(success, error, roomID);
        }

        private void OnFoundLobbiesList(LobbyMatchList_t param)
        {
            // SteamMatchmaking.RequestLobbyData()
            //get every room data
            INetworkTransport.FRoomData[] roomDatas = new INetworkTransport.FRoomData[param.m_nLobbiesMatching];
            for (int i = 0; i < roomDatas.Length; i++)
            {
                CSteamID room = SteamMatchmaking.GetLobbyByIndex(i);
                roomDatas[i] = new INetworkTransport.FRoomData()
                {
                    currentPlayersCount = 0,
                    maxPlayersCount = 10,
                    roomName = "",
                    roomID = room.m_SteamID.ToString(),
                };
            }

            //call event
            OnReceiveRooms?.Invoke(roomDatas);
        }

        #endregion
    }
}
#endif