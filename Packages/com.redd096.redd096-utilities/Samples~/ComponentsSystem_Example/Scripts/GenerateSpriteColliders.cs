using redd096.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// This is used to generate automatically colliders for the trains or the bridges
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/GenerateSpriteColliders")]
    public class GenerateSpriteColliders : MonoBehaviour
    {
        [SerializeField] Transform spriteToGetSize;
        [SerializeField] bool showDoors;
        [SerializeField] TrainDoor[] doors;

        //vars created by CreateColliders function
        private Transform collidersParent;
        private Vector2 trainHalfSize;
        private List<TrainDoor> doorsUp;
        private List<TrainDoor> doorsDown;
        private List<TrainDoor> doorsRight;
        private List<TrainDoor> doorsLeft;

        [Button("From DoorsArray -> create placeholders")]
        void DoorsArray_CreatePlaceholderInScene()
        {
            //create doors parent
            Transform doorsParent = CreateParent("Doors");

            //create doors
            for (int i = 0; i < doors.Length; i++)
            {
                GameObject doorObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                doorObj.name = "Door " + i;
                doorObj.transform.parent = doorsParent;
                doorObj.transform.SetLocalPositionAndRotation(doors[i].Position, Quaternion.identity);
                doorObj.transform.localScale = new Vector3(doors[i].Size.x, doors[i].Size.y, 1);
            }
        }

        [Button("From Placeholders -> set doors array")]
        void DoorsInScene_PopulateArray()
        {
            //find doors parent
            Transform doorsParent = transform.Find("Doors");
            if (doorsParent)
            {
                //for every child
                doors = new TrainDoor[doorsParent.childCount];
                for (int i = 0; i < doorsParent.childCount; i++)
                {
                    //create a door in the array
                    doors[i] = new TrainDoor()
                    {
                        Position = doorsParent.GetChild(i).localPosition,
                        Size = doorsParent.GetChild(i).localScale
                    };
                }
            }
        }

        [Button]
        void CreateColliders()
        {
            //create colliders parent
            collidersParent = CreateParent("Colliders");

            //calculate train size and get doors
            trainHalfSize = spriteToGetSize.localScale * 0.5f;
            GetDoors();

            //create colliders
            CreateCollidersUpOrDown(true);
            CreateCollidersUpOrDown(false);
            CreateCollidersRightOrLeft(true);
            CreateCollidersRightOrLeft(false);
        }

        Transform CreateParent(string parentName)
        {
            //create parent
            Transform t = transform.Find(parentName);
            if (t == null)
            {
                t = new GameObject(parentName).transform;
                t.parent = transform;
                t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                t.localScale = Vector3.one;
            }
            //or if already created, remove previous childs
            else
            {
                for (int i = t.childCount - 1; i >= 0; i--)
                {
                    if (Application.isPlaying == false)
                        DestroyImmediate(t.GetChild(i).gameObject);
                    else
                        Destroy(t.GetChild(i).gameObject);
                }
            }

            return t;
        }

        void GetDoors()
        {
            doorsUp = new List<TrainDoor>();
            doorsDown = new List<TrainDoor>();
            doorsRight = new List<TrainDoor>();
            doorsLeft = new List<TrainDoor>();

            foreach (var door in doors)
            {
                //use size to know if horizontal (then the door is up or down)
                if (door.Size.x > door.Size.y)
                {
                    if (door.Position.y > 0)
                        doorsUp.Add(door);
                    else
                        doorsDown.Add(door);
                }
                //or vertical door (then the door is right or left)
                else
                {
                    if (door.Position.x > 0)
                        doorsRight.Add(door);
                    else
                        doorsLeft.Add(door);
                }
            }

            //order doors
            doorsUp.OrderBy(door => door.Position.x).ToList();
            doorsDown.OrderBy(door => door.Position.x).ToList();
            doorsRight.OrderBy(door => door.Position.y).ToList();
            doorsLeft.OrderBy(door => door.Position.y).ToList();
        }

        void CreateCollidersRightOrLeft(bool isRight)
        {
            Vector2 colliderPosition = isRight ? Vector2.right : Vector2.left;
            string colliderName = isRight ? "ColliderRight" : "ColliderLeft";
            List<TrainDoor> doorsToUse = isRight ? doorsRight : doorsLeft;

            //start from down
            Vector2 pos = Vector2.down * trainHalfSize.y;
            for (int i = 0; i < doorsToUse.Count; i++)
            {
                //from position to down point
                if (Mathf.Approximately(pos.y, doorsToUse[i].PointDown.y) == false)
                {
                    EdgeCollider2D col = CreateCollider(colliderName + i, colliderPosition * trainHalfSize.x);
                    col.points = new Vector2[] { pos, doorsToUse[i].PointDown };
                }

                //set position as up point
                pos = doorsToUse[i].PointUp;
            }

            //last collider from position to up
            if (Mathf.Approximately(pos.y, trainHalfSize.y) == false)
            {
                EdgeCollider2D colliderRight = CreateCollider(colliderName, colliderPosition * trainHalfSize.x);
                colliderRight.points = new Vector2[] { pos, Vector2.up * trainHalfSize.y };
            }
        }

        void CreateCollidersUpOrDown(bool isUp)
        {
            Vector2 colliderPosition = isUp ? Vector2.up : Vector2.down;
            string colliderName = isUp ? "ColliderUp" : "ColliderDown";
            List<TrainDoor> doorsToUse = isUp ? doorsUp : doorsDown;

            //start from left
            Vector2 pos = Vector2.left * trainHalfSize.x;
            for (int i = 0; i < doorsToUse.Count; i++)
            {
                //from position to left point
                if (Mathf.Approximately(pos.x, doorsToUse[i].PointLeft.x) == false)
                {
                    EdgeCollider2D col = CreateCollider(colliderName + i, colliderPosition * trainHalfSize.y);
                    col.points = new Vector2[] { pos, doorsToUse[i].PointLeft };
                }

                //set position as right point
                pos = doorsToUse[i].PointRight;
            }

            //last collider from position to right
            if (Mathf.Approximately(pos.x, trainHalfSize.x) == false)
            {
                EdgeCollider2D colliderRight = CreateCollider(colliderName, colliderPosition * trainHalfSize.y);
                colliderRight.points = new Vector2[] { pos, Vector2.right * trainHalfSize.x };
            }
        }

        EdgeCollider2D CreateCollider(string colliderName, Vector2 localPosition)
        {
            //create collider and set parent and position
            Transform col = new GameObject(colliderName, typeof(EdgeCollider2D)).transform;
            col.parent = collidersParent.transform;
            col.localPosition = localPosition;
            return col.GetComponent<EdgeCollider2D>();
        }
    }

    /// <summary>
    /// This is used to calculate train colliders
    /// </summary>
    [System.Serializable]
    public class TrainDoor
    {
        public Vector2 Position;
        public Vector2 Size;

        public Vector2 PointUp => new Vector2(0, Position.y) + Vector2.up * Size * 0.5f;
        public Vector2 PointDown => new Vector2(0, Position.y) + Vector2.down * Size * 0.5f;
        public Vector2 PointRight => new Vector2(Position.x, 0) + Vector2.right * Size * 0.5f;
        public Vector2 PointLeft => new Vector2(Position.x, 0) + Vector2.left * Size * 0.5f;
    }
}