using System.Collections;
using UnityEngine;
using System.Collections.Generic;

#if FISHNET
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Connection;
using FishNet.Transporting.Tugboat;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Yak;
#endif

#if FISHNET
namespace redd096.Network
{
    /// <summary>
    /// NetworkManager for Fishnet unity's plugin
    /// </summary>
    public class NetworkManager_Fishnet : INetworkManager
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
        public bool IsServer => networkManager.IsServerStarted;
        public bool IsServerOnly => networkManager.IsServerOnlyStarted;
        public bool IsClient => networkManager.IsClientStarted;
        public bool IsClientOnly => networkManager.IsClientOnlyStarted;
        public bool IsHost => networkManager.IsHostStarted;
        public bool IsOffline => networkManager.IsOffline;
        public Dictionary<int, INetworkManager.RemoteClientData> Clients
        {
            get
            {
                var clients = networkManager.ServerManager.Clients;
                Dictionary<int, INetworkManager.RemoteClientData> result = new Dictionary<int, INetworkManager.RemoteClientData>();

                foreach (var keypair in clients)
                {
                    //create data wrapper
                    var clientData = new INetworkManager.RemoteClientData()
                    {
                        clientID = (ulong)keypair.Value.ClientId,
                        connectionAddress = networkManager.TransportManager.Transport.GetConnectionAddress(keypair.Value.ClientId),
                    };

                    result.Add(keypair.Key, clientData);
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
                var instances = NetworkManager.Instances;
                networkManager = instances != null && instances.Count > 0 ? instances[0] : null;
                if (networkManager != null && networkManager.Initialized)
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
            networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
            networkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;

            isInitialized = true;
        }

        public void Deinitialize()
        {
            //unregister events
            networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
            networkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;

            //remove networkManager
            networkManager = null;

            isInitialized = false;
        }

        public bool IsInitialized()
        {
            return networkManager && networkManager.Initialized && isInitialized;
        }

        #region get and set

        public INetworkTransport GenerateNetworkTransport()
        {
            switch (networkManager.TransportManager.Transport)
            {
#if DISABLESTEAMWORKS == false && STEAMWORKS_NET
                case FishySteamworks.FishySteamworks:
                    return new NetworkTransport_Steamworks();
#endif
                case Tugboat:
                case Yak:
                // case Bayou:
                // case FishyWebRTC:
                // case FishyUnityTransport:
                // case FishyEOS:
                // case FishyFacePunch:
                // case FishyRealTime:
                case Multipass:
                default:
                    Debug.LogError($"This Transport isn't supported by {typeof(NetworkManager_Fishnet).Name}");
                    return null;
            }
        }

        public ushort GetPort()
        {
            return networkManager.TransportManager.Transport.GetPort();
        }

        public void SetPort(ushort port)
        {
            networkManager.TransportManager.Transport.SetPort(port);
        }

        public string GetClientAddress()
        {
            return networkManager.TransportManager.Transport.GetClientAddress();
        }

        public void SetClientAddress(string address)
        {
            networkManager.TransportManager.Transport.SetClientAddress(address);
        }

        public int GetMaximumClients()
        {
            return networkManager.TransportManager.Transport.GetMaximumClients();
        }

        public void SetMaximumClients(int value)
        {
            networkManager.TransportManager.Transport.SetMaximumClients(value);
        }

        #endregion

        #region start

        public void StartServer()
        {
            networkManager.ServerManager.StartConnection();
        }

        public void StartServer(ushort port)
        {
            networkManager.ServerManager.StartConnection(port);
        }

        public void StartClient()
        {
            networkManager.ClientManager.StartConnection();
        }

        public void StartClient(string clientAddress)
        {
            networkManager.ClientManager.StartConnection(clientAddress);
        }

        public void StartClient(string clientAddress, ushort port)
        {
            networkManager.ClientManager.StartConnection(clientAddress, port);
        }

        public void StartHost()
        {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
        }

        #endregion

        #region stop

        public void StopServer()
        {
            networkManager.ServerManager.StopConnection(sendDisconnectMessage: true);
        }

        public void StopClient()
        {
            networkManager.ClientManager.StopConnection();
        }

        public void Shutdown()
        {
            // networkManager.ClientManager.StopConnection();
            // if (networkManager.IsServerStarted)
            //     networkManager.ServerManager.StopConnection(true);
            networkManager.TransportManager.Transport.Shutdown();
        }

        #endregion

        #region fishnet events

        private void OnServerConnectionState(ServerConnectionStateArgs args)
        {
            //check local server started or stopped
            if (args.ConnectionState == LocalConnectionState.Started)
                OnLocalServerStarted?.Invoke();
            else if (args.ConnectionState == LocalConnectionState.Stopped)
                OnLocalServerStopped?.Invoke();
        }

        private void OnClientConnectionState(ClientConnectionStateArgs args)
        {
            //check local client connect or disconnect
            if (args.ConnectionState == LocalConnectionState.Started)
                OnLocalClientConnect?.Invoke();
            else if (args.ConnectionState == LocalConnectionState.Stopped)
                OnLocalClientDisconnect?.Invoke();
        }

        private void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
        {
            int connectionId = args.ConnectionId;
            INetworkManager.RemoteClientData clientData = new INetworkManager.RemoteClientData()
            {
                clientID = (ulong)connection.ClientId,
                connectionAddress = networkManager.TransportManager.Transport.GetConnectionAddress(connection.ClientId),
            };
            
            //check server client connect or disconnect
            if (args.ConnectionState == RemoteConnectionState.Started)
                OnServerClientConnect?.Invoke(connectionId, clientData);
            else if (args.ConnectionState == RemoteConnectionState.Stopped)
                OnServerClientDisconnect?.Invoke(connectionId, clientData);
        }

        #endregion
    }
}
#endif