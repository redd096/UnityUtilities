namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR

    using UnityEditor;

    [CustomEditor(typeof(ProceduralMapManager))]
    public class ProceduralMapManagerEditor : Editor
    {
        private ProceduralMapManager mapManager;

        private void OnEnable()
        {
            mapManager = target as ProceduralMapManager;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Regen Map - OLD"))
            {
                mapManager.DestroyMap();
                mapManager.CreateEditorMap();

                //set undo
                Undo.RegisterFullObjectHierarchyUndo(target, "Regen World");
            }

            if (GUILayout.Button("Destroy Map"))
            {
                mapManager.DestroyMap();

                //set undo
                Undo.RegisterFullObjectHierarchyUndo(target, "Regen World");
            }
        }
    }

#endif

    //scriptable object class
    public abstract class ProceduralMapManagerCheck : ScriptableObject
    {
        public abstract bool IsValid(Room roomToPlace, ProceduralMapManager mapManager);
    }

    [System.Serializable]
    public class StructFixedRooms
    {
        [Min(0)] public int minID;
        [Min(0)] public int maxID;
        public Room roomPrefab;
    }

    [AddComponentMenu("redd096/Procedural Map/Procedural Map Manager")]
    public class ProceduralMapManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] bool regenOnPlay = true;
        [Min(1)] [SerializeField] int numberRooms = 12;
        [SerializeField] ProceduralMapManagerCheck[] checks = default;

        [Header("Attempts")]
        [Min(1)] [SerializeField] int maxAttempts = 5;
        [Min(1)] [SerializeField] int roomsPerAttempt = 5;
        [Min(1)] [SerializeField] int doorsPerAttempt = 2;

        [Header("Prefabs")]
        [SerializeField] Room[] roomPrefabs = default;

        [Header("Fixed Rooms")]
        [SerializeField] StructFixedRooms[] fixedRooms = default;

        //rooms
        List<Room> rooms = new List<Room>();
        private int roomID;
        private Room lastRoom;
        private bool succeded;
        private bool teleported;

        //for fixed rooms
        List<StructFixedRooms> fixedRoomsAlreadyChecked = new List<StructFixedRooms>();
        StructFixedRooms roomTryingToPut;
        List<StructFixedRooms> fixedRoomsAlreadyPut = new List<StructFixedRooms>();

        //to use more map managers
        public List<Room> Rooms => rooms;
        public List<Room> RoomsEveryOtherMapManager { get; private set; } = new List<Room>();

        void Start()
        {
            //regen map at start
            if (regenOnPlay)
            {
                DestroyMap();
                StartCoroutine(CreateMap());
            }
        }

        #region publi API

        public void DestroyMap()
        {
            //clear rooms
            rooms.Clear();
            roomID = 0;
            lastRoom = null;

            //clear fixed rooms
            fixedRoomsAlreadyChecked.Clear();
            roomTryingToPut = null;
            fixedRoomsAlreadyPut.Clear();

            //remove every child
            foreach (Transform child in transform)
            {
#if UNITY_EDITOR
                EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }

        public IEnumerator CreateMap(List<Room> roomsEveryOtherMapManager = null)
        {
            //save others map manager rooms
            if (roomsEveryOtherMapManager != null)
            {
                this.RoomsEveryOtherMapManager = roomsEveryOtherMapManager;
                //roomID = roomsEveryOtherMapManager.Count;
            }

            int attempts = 0;

            while (rooms.Count < numberRooms && attempts < maxAttempts)
            {
                //get room prefab, if can't, destroy dungeon and recreate (failed to put some room)
                Room newRoomPrefab = GetRoomPrefab();
                if (newRoomPrefab == null)
                    break;

                //generate room
                Room newRoom = Instantiate(newRoomPrefab, transform);

                //try to positionate
                yield return PositionRoom(newRoom);

                //if succeded, register it
                if (succeded)
                {
                    RegisterRoom(newRoom);
                    attempts = 0;
                }
                //else, destroy and try new one
                else
                {
                    Destroy(newRoom.gameObject);
                    Debug.Log("<color=orange>Shitty room, make another one</color>");
                    attempts++;
                }
            }

            //if not reach number rooms, regen
            if (rooms.Count < numberRooms)
            {
                //destroy old and create new one
                Debug.Log("<color=red>Shitty map, cry and restart</color>");
                DestroyMap();
                yield return CreateMap(roomsEveryOtherMapManager);
            }
            else
            {
                //if unique map manager, end generation
                if (regenOnPlay)
                    yield return EndGeneration();

                Debug.Log("<color=cyan>Mission complete!</color>");
            }
        }

        public IEnumerator EndGeneration()
        {
            //foreach room created, call function
            foreach (Room room in rooms)
            {
                room.EndRoom();

                yield return null;
            }
        }

        #endregion

        #region private API

        Room GetRoomPrefab()
        {
            //quando istanzi, controlla se ci sono stanze fisse
            //se non si attacca, vedere se ce ne sono altre
            //se una stanza non viene creata nel suo range, distruggere e ricreare il dungeon

            //quando istanzi, controlla se ci sono stanze fisse
            //aggiungere alla lista delle già checkate, così se non si attacca ne viene provata un'altra
            //quando viene posizionata una stanza pulisci le già checkate per riprovarle tutte, ma se quella creata era una di quelle fisse aggiungila alla lista già piazzate per non riprovarla
            //se la stanza fissa non viene posizionata ma ha il range più alto, si può riprovare al posizionamento successivo, ma assicurarsi di resettare la variabile se si posiziona una stanza non fissa
            //se una stanza non viene creata nel suo range, distruggere e ricreare il dungeon

            //if a room failed at its maxID, recreate dungeon
            if (roomTryingToPut != null)
            {
                if (roomID >= roomTryingToPut.maxID)
                {
                    return null;
                }
            }

            //check fixed rooms
            foreach (StructFixedRooms checkRoom in fixedRooms)
            {
                //be sure is not already checked
                if (fixedRoomsAlreadyChecked.Contains(checkRoom) || fixedRoomsAlreadyPut.Contains(checkRoom))
                {
                    continue;
                }

                //if there is one who need this ID
                if (roomID >= checkRoom.minID && roomID <= checkRoom.maxID)
                {
                    //add to list already check
                    fixedRoomsAlreadyChecked.Add(checkRoom);

                    //return it
                    roomTryingToPut = checkRoom;
                    return checkRoom.roomPrefab;
                }
            }

            //else return random room
            roomTryingToPut = null;
            return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        }

        private IEnumerator PositionRoom(Room newRoom)
        {
            succeded = false;
            teleported = false;

            if (rooms.Count == 0)
            {
                //first room
                Debug.Log("<color=lime>positioned first room</color>");
                newRoom.SetPosition(Vector3.zero);
                succeded = true;
                yield break;
            }
            else
            {
                //attach to first room of another map manager
                if (rooms.Count <= 0)
                    lastRoom = RoomsEveryOtherMapManager[0];
                //else try attach to last room
                else
                    lastRoom = rooms[rooms.Count - 1];

                for (int roomLoop = 0; roomLoop < roomsPerAttempt; roomLoop++)
                {
                    for (int doorLoop = 0; doorLoop < doorsPerAttempt; doorLoop++)
                    {
                        //try attach to door
                        if (newRoom.SetPosition(lastRoom.GetRandomDoor(), lastRoom, this) && IsValidPlace(newRoom))
                        {
                            Debug.Log("<color=lime>positioned room " + roomID + "</color>");
                            succeded = true;
                            yield break;
                        }

                        //else try another door
                        yield return null;
                    }

                    //else try another room
                    Debug.Log("<color=yellow>changed room</color>");
                    teleported = true;                                      //set teleported true
                    lastRoom = rooms.Count > 0 ? rooms[Random.Range(0, rooms.Count - 1)] : RoomsEveryOtherMapManager[0];   //if rooms at 0, try again first room of another map manager
                }
            }
        }

        bool IsValidPlace(Room roomToPlace)
        {
            //foreach check, be sure is valid
            foreach (ProceduralMapManagerCheck check in checks)
            {
                if (!check.IsValid(roomToPlace, this))
                    return false;
            }

            return true;
        }

        private void RegisterRoom(Room newRoom)
        {
            //add to list and update ID
            rooms.Add(newRoom);
            newRoom.Register(roomID, teleported);
            roomID++;

            //update fixed rooms
            UpdateFixedRooms();
        }

        void UpdateFixedRooms()
        {
            //clear rooms already checked 
            fixedRoomsAlreadyChecked.Clear();

            //and add to list already put
            if (roomTryingToPut != null)
            {
                fixedRoomsAlreadyPut.Add(roomTryingToPut);
                roomTryingToPut = null;
            }
        }

        #endregion

        #region editor but old

        public void CreateEditorMap()
        {
            int roomID = 0;
            Room currentRoom = null;
            Room lastRoom = null;

            int loopCount = 0;

            while (rooms.Count < numberRooms)
            {
                //generate first room
                if (rooms.Count <= 0)
                {
                    GenerateFirstRoom(ref currentRoom, ref roomID, ref lastRoom);
                    currentRoom = null;
                }
                //generate other rooms
                else
                {
                    //if generate room, be sure to have loop count at 0
                    if (GenerateOtherRooms(ref currentRoom, ref roomID, ref lastRoom, loopCount > 20))
                    {
                        currentRoom = null;
                        loopCount = 0;
                    }
                    //if no generate, increase loopCount
                    else
                    {
                        loopCount++;
                    }
                }

                //if we are in endless loop (no space for a room)
                if (loopCount > 20)
                {
                    //try to put room adjacent to another room in the list (instead of last one created)
                    lastRoom = rooms[Random.Range(0, rooms.Count)];
                }

                //if continue loop, maybe this room can't attach to others, destroy and try new one
                if (loopCount > 50)
                {
#if UNITY_EDITOR
                    if (EditorApplication.isPlaying)
                        Destroy(currentRoom.gameObject);
                    else
                        DestroyImmediate(currentRoom.gameObject);
#else
                Destroy(currentRoom.gameObject);
#endif
                }

                //if loop count is too big, break while
                if (loopCount > 100)
                {
                    Debug.Log("<color=yellow>Stopped an endless loop</color>");
                    break;
                }
            }

            if (rooms.Count >= numberRooms)
            {
                //end generation
                EndGeneration();

                Debug.Log("<color=cyan>Mission complete!</color>");
            }
        }

        void GenerateFirstRoom(ref Room currentRoom, ref int roomID, ref Room lastRoom)
        {
            //instantiate room (child of this transform) and initialize
            currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], transform);
            currentRoom.Register(roomID, false);
            currentRoom.SetPosition(Vector3.zero);

            //add to list and update ID and last room
            rooms.Add(currentRoom);
            lastRoom = currentRoom;
            roomID++;
        }

        bool GenerateOtherRooms(ref Room currentRoom, ref int roomID, ref Room lastRoom, bool teleported)
        {
            //get random direction by last room
            DoorStruct door = lastRoom.GetRandomDoor();

            //instantiate room (only if != null, cause can be just a teleport of current room)
            if (currentRoom == null)
            {
                currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], transform);
            }

            //initialize and set position
            currentRoom.Register(roomID, teleported);
            if (currentRoom.SetPosition(door, lastRoom, this))    //if can't set position is because there are not doors which attach - EDIT now check also if there is space for this room
            {
                //add to list and update ID and last room
                rooms.Add(currentRoom);
                lastRoom = currentRoom;
                roomID++;
                return true;
            }

            return false;
        }

        #endregion
    }
}