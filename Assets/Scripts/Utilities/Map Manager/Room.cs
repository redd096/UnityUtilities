using System.Collections.Generic;
using UnityEngine;

public enum CardinalDirection
{
    up, down, left, right
}

[System.Serializable]
public struct DoorStruct
{
    public Transform door;
    public CardinalDirection direction;
}

[AddComponentMenu("Project Brain/Room")]
[SelectionBase]
public class Room : MonoBehaviour
{
    #region variables

    [Header("2D or 3D")]
    [SerializeField] bool is3D = true;

    [Header("Important")]
    [SerializeField] float tileSize = 1f;   //size of every tile which compose this room
    [SerializeField] int width = 1;         //int because the size will be exspressed in tiles
    [SerializeField] int height = 1;        //int because the size will be exspressed in tiles
    [SerializeField] List<DoorStruct> doors = new List<DoorStruct>();

    float HalfWidth => width * tileSize * 0.5f;
    float HalfHeight => height * tileSize * 0.5f;
    Vector2 UpRight => is3D ? new Vector3(transform.position.x + HalfWidth, transform.position.z + HalfHeight) : new Vector3(transform.position.x + HalfWidth, transform.position.y + HalfHeight);
    Vector2 DownLeft => is3D ? new Vector3(transform.position.x - HalfWidth, transform.position.z - HalfHeight) : new Vector3(transform.position.x - HalfWidth, transform.position.y - HalfHeight);

    [Header("DEBUG")]
    [SerializeField] TextMesh textID = default;
    [SerializeField] int id = 0;

    [Header("DEBUG adjancent room (necessary not public)")]
    [SerializeField] DoorStruct adjacentDoor = default;
    [SerializeField] Room adjacentRoom = default;
    [SerializeField] DoorStruct door = default;

    #endregion

    #region public API

    public void Init(int id, bool teleported)
    {
        this.id = id;

        //debug
        if (textID)
        {
            textID.text = teleported ? "tp: " + id.ToString() : id.ToString();
        }
        else
        {
            Debug.Log("La room " + name + " non ha un Text per mostrare il suo ID in scena");
        }

        //random color
        float h = Random.value;
        Color color = Color.HSVToRGB(h, 0.8f, 0.8f);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = color;
            color = Color.HSVToRGB(h, 1f, 1f);
        }
    }

    public void SetPosition(Vector3 position)
    {
        //set position
        transform.position = position;
    }

    public bool SetPosition(DoorStruct adjacentDoor, Room adjacentRoom, MapManager mapManager)
    {
        //set references
        this.adjacentDoor = adjacentDoor;
        this.adjacentRoom = adjacentRoom;

        if (adjacentRoom == null || adjacentDoor.door == null)
            Debug.Log("<color=red>Houston, abbiamo un problema</color>");

        //check this room has a door to adjacent room, and adjust position
        if (CheckDoors() == false)
            return false;

        //check this room is not inside another room and viceversa
        if (CheckOverlap(mapManager) == false)
            return false;

        return true;
    }

    public DoorStruct GetRandomDoor()
    {
        //return random door
        return doors[Random.Range(0, doors.Count)];
    }

    #endregion

    #region private API

    bool CheckDoors()
    {
        //if moving left, check doors on right, else check doors on left
        if (adjacentDoor.direction == CardinalDirection.left || adjacentDoor.direction == CardinalDirection.right)
        {
            door.direction = adjacentDoor.direction == CardinalDirection.left ? CardinalDirection.right : CardinalDirection.left;
        }
        //if moving up, check doors on bottom, else check doors on top
        else
        {
            door.direction = adjacentDoor.direction == CardinalDirection.up ? CardinalDirection.down : CardinalDirection.up;
        }

        List<DoorStruct> possibleDoors = new List<DoorStruct>();

        //add every possible door (on left or right side)
        foreach (DoorStruct d in doors)
        {
            if (d.direction == door.direction)
            {
                possibleDoors.Add(d);
            }
        }

        //if no possible doors, return
        if (possibleDoors.Count <= 0)
            return false;

        //else get a random door between possibles
        door.door = possibleDoors[Random.Range(0, possibleDoors.Count)].door;

        //calculate distance and move
        Vector3 fromDoorToAdjacentDoor = adjacentDoor.door.position - door.door.position;
        transform.position += fromDoorToAdjacentDoor;

        return true;
    }

    bool CheckOverlap(MapManager mapManager)
    {
        //check rooms of this map manager and also rooms of others map managers
        List<Room> roomsToCheck = new List<Room>(mapManager.Rooms);
        if (mapManager.RoomsEveryOtherMapManager != null)
            roomsToCheck.AddRange(mapManager.RoomsEveryOtherMapManager);

        //foreach room
        foreach (Room room in roomsToCheck)
        {
            //check if there is a room inside this one
            if (PointsInsideRoom(room))
                return false;

            //check if this one is inside a room
            if (room.PointsInsideRoom(this))
                return false;
        }

        return true;
    }

    bool PointsInsideRoom(Room roomToCheck)
    {
        //get every tile from downleft (<= so check also DownRight, UpLeft, UpRight)
        for (int x = 0; x <= roomToCheck.width; x++)
        {
            for (int y = 0; y <= roomToCheck.height; y++)
            {
                //direction right up, if reached limit go backward
                Vector2 directionGap = new Vector2(x >= roomToCheck.width ? -1 : 1, y >= roomToCheck.height ? -1 : 1);

                Vector2 point = roomToCheck.DownLeft + (Vector2.right * x * roomToCheck.tileSize) + (Vector2.up * y * roomToCheck.tileSize)     //down left of every tile
                    + (directionGap * roomToCheck.tileSize * 0.1f);                                                                             //little gap to no have half room inside another

                //check is inside this room
                if (point.x > DownLeft.x && point.x < UpRight.x)
                {
                    if (point.y > DownLeft.y && point.y < UpRight.y)
                    {
                        return true;
                    }
                }
            }
        }

        //check also center if inside this room
        Vector2 center = is3D ? new Vector2(roomToCheck.transform.position.x, roomToCheck.transform.position.y) : new Vector2(roomToCheck.transform.position.x, roomToCheck.transform.position.z);
        if (center.x > DownLeft.x && center.x < UpRight.x)
        {
            if (center.y > DownLeft.y && center.y < UpRight.y)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
