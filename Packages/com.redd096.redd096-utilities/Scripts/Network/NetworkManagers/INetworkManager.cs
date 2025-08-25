using System.Collections;
using System.Collections.Generic;

namespace redd096.Network
{
    /// <summary>
    /// Base interface for NetworkManager (mirror, fishnet, photon, etc...)
    /// </summary>
    public interface INetworkManager
    {
        /// <summary>
        /// Return which NetworkManager to use for this project
        /// </summary>
        public static INetworkManager GenerateNetworkManager()
        {
#if FISHNET
            return new NetworkManager_Fishnet();
#elif PURRNET
            return new NetworkManager_Purrnet();
#else
            UnityEngine.Debug.LogError("Impossible create Network Manager for this device");
            return null;
#endif
        }

        #region events

        /// <summary>
        /// Called when the local server started
        /// </summary>
        System.Action OnLocalServerStarted { get; set; }
        /// <summary>
        /// Called when the local server stopped
        /// </summary>
        System.Action OnLocalServerStopped { get; set; }
        /// <summary>
        /// Called when the local client connect to a server
        /// </summary>
        System.Action OnLocalClientConnect { get; set; }
        /// <summary>
        /// Called when the local client disconnect from server
        /// </summary>
        System.Action OnLocalClientDisconnect { get; set; }
        /// <summary>
        /// Called on server, when a client connect to it. 
        /// Return connectionId and data of the client
        /// </summary>
        System.Action<int, RemoteClientData> OnServerClientConnect { get; set; }
        /// <summary>
        /// Called on server, when a client disconnect from it. 
        /// Return connectionId and data of the client
        /// </summary>
        System.Action<int, RemoteClientData> OnServerClientDisconnect { get; set; }

        #endregion


        #region properties

        /// <summary>
        /// True if server is started
        /// </summary>
        bool IsServer { get; }
        /// <summary>
        /// True if only the server is started
        /// </summary>
        bool IsServerOnly { get; }
        /// <summary>
        /// True if the client is authenticated
        /// </summary>
        bool IsClient { get; }
        /// <summary>
        /// True if only the client is authenticated
        /// </summary>
        bool IsClientOnly { get; }
        /// <summary>
        /// True if client is authenticated, and the server is started
        /// </summary>
        bool IsHost { get; }
        /// <summary>
        /// True if client nor server are started
        /// </summary>
        bool IsOffline { get; }

        /// <summary>
        /// Works only on server. Return the list of clients connected. 
        /// The Key is the connectionId and the Value is the data of the connected client
        /// </summary>
        Dictionary<int, RemoteClientData> Clients { get; }

        #endregion


        /// <summary>
        /// Data to rapresent a client connected to server
        /// </summary>
        public struct RemoteClientData
        {
            public ulong clientID;
            /// <summary>
            /// Client IP. Not all NetworkManagers return it
            /// </summary>
            public string connectionAddress;

            public override string ToString()
            {
                return $"Id [{clientID}] Address [{connectionAddress}]";
            }
        }


        /// <summary>
        /// Initialize managers and register to events
        /// </summary>
        /// <param name="forceRetryOnError">On error, after call event, try again to initialize</param>
        /// <param name="onError">Call event if there is an error in the initialization</param>
        IEnumerator Initialize(bool forceRetryOnError, System.Action<string> onError);
        /// <summary>
        /// Deinitialize managers and unregister from events
        /// </summary>
        void Deinitialize();
        /// <summary>
        /// Return if everything is initialized correctly
        /// </summary>
        bool IsInitialized();

        /// <summary>
        /// Return NetworkTransport to use, based on Transport set in NetworkManager
        /// </summary>
        INetworkTransport GenerateNetworkTransport();


        #region get and set

        /// <summary>
        /// Gets which port to use
        /// </summary>
        ushort GetPort();
        /// <summary>
        /// Sets which port to use
        /// </summary>
        void SetPort(ushort port);
        /// <summary>
        /// Returns which address the client will connect to
        /// </summary>
        string GetClientAddress();
        /// <summary>
        /// Sets which address the client will connect to
        /// </summary>
        void SetClientAddress(string address);
        /// <summary>
        /// Returns the maximum number of clients allowed to connect to the server. If the transport does not support this method the value -1 is returned.
        /// </summary>
        /// <returns>Maximum clients transport allows.</returns>
        int GetMaximumClients();
        /// <summary>
        /// Sets the maximum number of clients allowed to connect to the server. If applied at runtime and clients exceed this value existing clients will stay connected but new clients may not connect.
        /// </summary>
        /// <param name = "value">Maximum clients to allow.</param>
        void SetMaximumClients(int value);

        #endregion


        #region start

        /// <summary>
        /// Start server
        /// </summary>
        void StartServer();
        /// <summary>
        /// Set Port and Start server
        /// </summary>
        void StartServer(ushort port);
        /// <summary>
        /// Connect to a server
        /// </summary>
        void StartClient();
        /// <summary>
        /// Set ClientAddress and Connect to the server
        /// </summary>
        void StartClient(string address);
        /// <summary>
        /// Set ClientAddress and Port and Connect to the server
        /// </summary>
        void StartClient(string address, ushort port);
        /// <summary>
        /// Start both server and client and connect to self
        /// </summary>
        void StartHost();

        #endregion


        #region stop

        /// <summary>
        /// Stop the local server connection (close the server)
        /// </summary>
        void StopServer();
        /// <summary>
        /// Stop the local client connection (disconnect from server)
        /// </summary>
        void StopClient();
        /// <summary>
        /// Stop both client and server
        /// </summary>
        void Shutdown();

        #endregion
    }
}