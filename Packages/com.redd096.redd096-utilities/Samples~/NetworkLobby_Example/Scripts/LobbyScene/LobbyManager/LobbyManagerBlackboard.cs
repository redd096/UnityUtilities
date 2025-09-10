using redd096.Network;

namespace redd096.Examples.NetworkLobby
{
    public class LobbyManagerBlackboard
    {
        public INetworkManager NetworkManager { get; set; }
        public INetworkTransport NetTransport { get; set; }

        /// <summary>
        /// Created or Joined room
        /// </summary>
        public string CurrentRoomID;

        /// <summary>
        /// Used to pass temporary variables to next state
        /// </summary>
        public object TempUserData;
    }
}