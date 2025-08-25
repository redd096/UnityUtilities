using System.Collections;
using UnityEngine;
using System.Collections.Generic;

#if PURRNET
using PurrNet;
using PurrNet.Transports;
using PurrNet.Steam;
#endif

#if PURRNET
namespace redd096.Network
{
    /// <summary>
    /// NetworkManager for Purrnet unity's plugin
    /// </summary>
    public class NetworkManager_Purrnet : INetworkManager
    {
        //consts
        private const float TIMEOUT_INITIALIZATION = 2f;
        private const float DELAY_BETWEEN_RETRY_INITIALIZATION = 2f;

        //manager events
        public System.Action OnLocalServerStarted { get; set; }
        public System.Action OnLocalServerStopped { get; set; }
        public System.Action OnLocalClientConnect { get; set; }
        public System.Action OnLocalClientDisconnect { get; set; }
        public System.Action<int, INetworkManager.RemoteClientData> OnServerClientConnect { get; set; }
        public System.Action<int, INetworkManager.RemoteClientData> OnServerClientDisconnect { get; set; }

        //manager properties
        public bool IsServer => networkManager.isServer;
        public bool IsServerOnly => networkManager.isServerOnly;
        public bool IsClient => networkManager.isClient;
        public bool IsClientOnly => networkManager.isClientOnly;
        public bool IsHost => networkManager.isHost;
        public bool IsOffline => networkManager.isOffline;
        public Dictionary<int, INetworkManager.RemoteClientData> Clients
        {
            get
            {
                //get clients
                var clientsIds = networkManager.players;
                Dictionary<Connection, PlayerID> clients = new Dictionary<Connection, PlayerID>();
                foreach (var id in clientsIds)
                {
                    if (networkManager.playerModule.TryGetConnection(id, out Connection conn))
                        clients.Add(conn, id);
                }

                Dictionary<int, INetworkManager.RemoteClientData> result = new Dictionary<int, INetworkManager.RemoteClientData>();
                foreach (var keypair in clients)
                {
                    //create data wrapper
                    var clientData = new INetworkManager.RemoteClientData()
                    {
                        clientID = keypair.Value.id.value,
                    };

                    result.Add(keypair.Key.connectionId, clientData);
                }

                return result;
            }
        }

        //private
        private NetworkManager networkManager;
        private bool isInitialized;

        public IEnumerator Initialize(bool forceRetryOnError, System.Action<string> onError)
        {
            float timer = Time.time + TIMEOUT_INITIALIZATION;

            while (true)
            {
                //find NetworkManager in scene
                networkManager = NetworkManager.main;
                if (networkManager != null)
                    break;

                //if timeout, send error and stop coroutine
                if (Time.time > timer)
                {
                    string error = $"Error: Impossible to find NetworkManager in scene. Be sure to put the prefab of NetworkManager in your main scene";
                    onError?.Invoke(error);

                    //if force retry, just wait few seconds before retry. Else quit
                    if (forceRetryOnError)
                        yield return new WaitForSeconds(DELAY_BETWEEN_RETRY_INITIALIZATION);
                    else
                        yield break;
                }
            }

            //register events
            networkManager.onServerConnectionState += OnServerConnectionState;
            networkManager.onClientConnectionState += OnClientConnectionState;
            networkManager.onPlayerJoined += OnPlayerJoined;
            networkManager.onPlayerLeft += OnPlayerLeft;

            isInitialized = true;
        }

        public void Deinitialize()
        {
            //unregister events
            networkManager.onServerConnectionState -= OnServerConnectionState;
            networkManager.onClientConnectionState -= OnClientConnectionState;
            networkManager.onPlayerJoined -= OnPlayerJoined;
            networkManager.onPlayerLeft -= OnPlayerLeft;

            //remove networkManager
            networkManager = null;

            isInitialized = false;
        }

        public bool IsInitialized()
        {
            return networkManager && isInitialized;
        }

        public INetworkTransport GenerateNetworkTransport()
        {
            switch (networkManager.transport)
            {
#if DISABLESTEAMWORKS == false && STEAMWORKS_NET
                case SteamTransport:
                    return new NetworkTransport_Steamworks();
#endif
                case PurrTransport:
                    return new NetworkTransport_PurrTransport();
                case UDPTransport:
                case WebTransport webTransport:
                case LocalTransport:
                // case CompositeTransport:
                case GenericTransport:
                default:
                    Debug.LogError($"This Transport isn't supported by {typeof(NetworkManager_Purrnet).Name}");
                    return null;
            }
        }

        #region get and set

        public ushort GetPort()
        {
            try
            {
                //try with reflection
                var property = networkManager.transport.GetType().GetProperty("serverPort");
                if (property?.PropertyType == typeof(ushort))
                    return (ushort)property.GetValue(networkManager.transport);
                return 0;
            }
            catch
            {
                //else, try in bad way
                switch (networkManager.transport)
                {
                    case UDPTransport udpTransport:
                        return udpTransport.serverPort;
                    case WebTransport webTransport:
                        return webTransport.serverPort;
                    case SteamTransport steamTransport:
                        return steamTransport.serverPort;
                    case PurrTransport:
                    case LocalTransport:
                    case GenericTransport:
                        // case CompositeTransport:
                        return 0;
                    default:
                        Debug.LogError($"This Transport isn't supported by {typeof(NetworkManager_Purrnet).Name}");
                        return 0;
                }
            }
        }

        public void SetPort(ushort port)
        {
            try
            {
                //try with reflection
                var property = networkManager.transport.GetType().GetProperty("serverPort");
                if (property?.PropertyType == typeof(ushort))
                    property.SetValue(networkManager.transport, port);
            }
            catch
            {
                //else, try in bad way
                switch (networkManager.transport)
                {
                    case UDPTransport udpTransport:
                        udpTransport.serverPort = port;
                        break;
                    case WebTransport webTransport:
                        webTransport.serverPort = port;
                        break;
                    case SteamTransport steamTransport:
                        steamTransport.serverPort = port;
                        break;
                    case PurrTransport:
                    case LocalTransport:
                    case GenericTransport:
                        //case CompositeTransport:
                        break;
                    default:
                        Debug.LogError($"This Transport isn't supported by {typeof(NetworkManager_Purrnet).Name}");
                        break;
                }
            }
        }

        public string GetClientAddress()
        {
            try
            {
                //try with reflection
                var property = networkManager.transport.GetType().GetProperty("address");
                if (property?.PropertyType == typeof(string))
                    return (string)property.GetValue(networkManager.transport);
                return "";
            }
            catch
            {
                //else, try in bad way
                switch (networkManager.transport)
                {
                    case UDPTransport udpTransport:
                        return udpTransport.address;
                    case WebTransport webTransport:
                        return webTransport.address;
                    case SteamTransport steamTransport:
                        return steamTransport.address;
                    case PurrTransport:
                    case LocalTransport:
                    case GenericTransport:
                        // case CompositeTransport:
                        return "";
                    default:
                        Debug.LogError($"This Transport isn't supported by {typeof(NetworkManager_Purrnet).Name}");
                        return "";
                }
            }
        }

        public void SetClientAddress(string address)
        {
            try
            {
                //try with reflection
                var property = networkManager.transport.GetType().GetProperty("address");
                if (property?.PropertyType == typeof(string))
                    property.SetValue(networkManager.transport, address);
            }
            catch
            {
                //else, try in bad way
                switch (networkManager.transport)
                {
                    case UDPTransport udpTransport:
                        udpTransport.address = address;
                        break;
                    case WebTransport webTransport:
                        webTransport.address = address;
                        break;
                    case SteamTransport steamTransport:
                        steamTransport.address = address;
                        break;
                    case PurrTransport:
                    case LocalTransport:
                    case GenericTransport:
                        //case CompositeTransport:
                        break;
                    default:
                        Debug.LogError($"This Transport isn't supported by {typeof(NetworkManager_Purrnet).Name}");
                        break;
                }
            }
        }

        public int GetMaximumClients()
        {
            try
            {
                //try with reflection
                var property = networkManager.transport.GetType().GetProperty("maxConnections");
                if (property?.PropertyType == typeof(int))
                    return (int)property.GetValue(networkManager.transport);
                return 0;
            }
            catch
            {
                //else, try in bad way
                switch (networkManager.transport)
                {
                    case UDPTransport udpTransport:
                        return udpTransport.maxConnections;
                    case WebTransport webTransport:
                        return webTransport.maxConnections;
                    case SteamTransport:
                    case PurrTransport:
                    case LocalTransport:
                    case GenericTransport:
                        // case CompositeTransport:
                        return 0;
                    default:
                        Debug.LogError($"This Transport isn't supported by {typeof(NetworkManager_Purrnet).Name}");
                        return 0;
                }
            }
        }

        public void SetMaximumClients(int value)
        {
            try
            {
                //try with reflection
                var property = networkManager.transport.GetType().GetProperty("maxConnections");
                if (property?.PropertyType == typeof(int))
                    property.SetValue(networkManager.transport, value);
            }
            catch
            {
                //else, try in bad way
                switch (networkManager.transport)
                {
                    case UDPTransport udpTransport:
                        udpTransport.maxConnections = value;
                        break;
                    case WebTransport webTransport:
                        webTransport.maxConnections = value;
                        break;
                    case SteamTransport:
                    case PurrTransport:
                    case LocalTransport:
                    case GenericTransport:
                        //case CompositeTransport:
                        break;
                    default:
                        Debug.LogError($"This Transport isn't supported by {typeof(NetworkManager_Purrnet).Name}");
                        break;
                }
            }
        }

        #endregion

        #region start

        public void StartServer()
        {
            networkManager.StartServer();
        }

        public void StartServer(ushort port)
        {
            // networkManager.rawTransport.Listen(port);
            SetPort(port);
            networkManager.StartServer();
        }

        public void StartClient()
        {
            networkManager.StartClient();
        }

        public void StartClient(string clientAddress)
        {
            SetClientAddress(clientAddress);
            networkManager.StartClient();
        }

        public void StartClient(string clientAddress, ushort port)
        {
            // networkManager.rawTransport.Connect(clientAddress, port);
            SetClientAddress(clientAddress);
            SetPort(port);
            networkManager.StartClient();
        }

        public void StartHost()
        {
            networkManager.StartHost();
        }

        #endregion

        #region stop

        public void StopServer()
        {
            networkManager.StopServer();
        }

        public void StopClient()
        {
            networkManager.StopClient();
        }

        public void Shutdown()
        {
            networkManager.StopServer();
            networkManager.StopClient();
        }

        #endregion

        #region purrnet events

        private void OnServerConnectionState(ConnectionState state)
        {
            //check local server started or stopped
            if (state == ConnectionState.Connected)
                OnLocalServerStarted?.Invoke();
            else if (state == ConnectionState.Disconnected)
                OnLocalServerStopped?.Invoke();
        }

        private void OnClientConnectionState(ConnectionState state)
        {
            //check local client connect or disconnect
            if (state == ConnectionState.Connected)
                OnLocalClientConnect?.Invoke();
            else if (state == ConnectionState.Disconnected)
                OnLocalClientDisconnect?.Invoke();
        }

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            int connectionId = 0;
            if (networkManager.playerModule.TryGetConnection(player, out Connection conn))
                connectionId = conn.connectionId;

            INetworkManager.RemoteClientData clientData = new INetworkManager.RemoteClientData()
            {
                clientID = player.id.value,
            };

            //on server client connect
            OnServerClientConnect?.Invoke(connectionId, clientData);
        }

        private void OnPlayerLeft(PlayerID player, bool asServer)
        {
            int connectionId = 0;
            if (networkManager.playerModule.TryGetConnection(player, out Connection conn))
                connectionId = conn.connectionId;

            INetworkManager.RemoteClientData clientData = new INetworkManager.RemoteClientData()
            {
                clientID = player.id.value,
            };

            //on server client disconnect
            OnServerClientDisconnect?.Invoke(connectionId, clientData);
        }

        #endregion
    }
}
#endif