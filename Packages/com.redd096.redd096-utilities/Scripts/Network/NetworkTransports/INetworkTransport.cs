using System.Collections;

namespace redd096.Network
{
    /// <summary>
    /// Base interface for NetworkTransport (steam, epic online services, etc...)
    /// </summary>
    public interface INetworkTransport
    {
        /// <summary>
        /// Return success or fail. Then return a string for the error if fail, and a string for the roomID if success
        /// </summary>
        System.Action<bool, string, string> OnRoomCreated { get; set; }
        /// <summary>
        /// Return success or fail. Then return a string for the error if fail, and a string for the roomID if success
        /// </summary>
        System.Action<bool, string, string> OnJoinedRoom { get; set; }
        /// <summary>
        /// With some transports is possible to receive join request from outside your game (e.g. Steam by clicking on Friends List). 
        /// When receive this event, you can call <see cref="JoinRoomFromExternalSource"/> to join that room.
        /// </summary>
        System.Action<object> OnTryJoinFromExternalSource { get; set; }
        /// <summary>
        /// Return a list of rooms
        /// </summary>
        System.Action<FRoomData[]> OnReceiveRooms { get; set; }
        /// <summary>
        /// Return success or fail. Then return a string for the error if fail, and a string for the roomID left
        /// </summary>
        System.Action<bool, string, string> OnLeaveRoom { get; set; }
        

        /// <summary>
        /// Data received in OnReceiveRooms event
        /// </summary>
        public struct FRoomData
        {
            public int currentPlayersCount;
            public int maxPlayersCount;
            public string roomName;
            public string roomID;
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
        /// Request to create a room. Register to <see cref="OnRoomCreated"/> to read the result
        /// </summary>
        /// <param name="maxClients">Limit of players for this room</param>
        void CreateRoom(int maxClients);
        /// <summary>
        /// Request to join a room. Register to <see cref="OnJoinedRoom"/> to read the result
        /// </summary>
        /// <param name="roomID">The room to join</param>
        void JoinRoom(string roomID);
        /// <summary>
        /// With some transports is possible to receive join request from outside your game (e.g. Steam by clicking on Friends List). 
        /// This will trigger <see cref="OnTryJoinFromExternalSource"/> event. You can call this function to do the same as JoinRoom but with received parameter
        /// </summary>
        /// <param name="roomToJoin">The same param received by the event</param>
        void JoinRoomFromExternalSource(object roomToJoin);
        /// <summary>
        /// Request the list of rooms. Register to <see cref="OnReceiveRooms"/> to read the result
        /// </summary>
        void RequestRoomsList();
        /// <summary>
        /// Leave room. Register to <see cref="OnLeaveRoom"/> to read the result
        /// </summary>
        void LeaveRoom(string roomID);
    }
}