#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using redd096.Attributes.AttributesEditorUtility;
using UnityEditor;
using redd096.Attributes;

namespace redd096.Examples.ComponentsSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ExampleGenerateSpriteColliders))]
    public class GenerateSpriteCollidersEditor : ButtonEditor
    {
        private void OnSceneGUI()
        {
            //get vars
            bool showDoors = (bool)target.GetValue("showDoors");
            TrainDoor[] doors = (TrainDoor[])target.GetValue("doors");

            //draw every door
            if (showDoors && doors != null)
            {
                for (int i = 0; i < doors.Length; i++)
                {
                    //doors[i].pointA = DrawHandle(train.transform, doors[i].pointA, $"[{i}] A");
                    //doors[i].pointB = DrawHandle(train.transform, doors[i].pointB, $"[{i}] B");
                    DrawHandle(target.GetTransform(), ref doors[i].Position, ref doors[i].Size, $"[{i}]");
                }
            }

            //update properties 
            FieldInfo doorsProperty = target.GetField("doors");
            doorsProperty.SetValue(target, doors);
        }

        void DrawHandle(Transform transform, ref Vector2 point, ref Vector2 pointSize, string pointName)
        {
            //point to local position
            Vector3 pos = (Vector2)transform.position + point;
            Vector3 size = pointSize;

            //draw handle
            Handles.Label(pos + Vector3.down * 0.08f, pointName);
            Handles.DrawSolidRectangleWithOutline(new Rect(pos - size * 0.5f, size), Color.white, Color.black);
            Handles.TransformHandle(ref pos, Quaternion.identity, ref size);

            point = pos - transform.position;
            pointSize = size;
        }

        //Vector2 DrawHandle(Transform transform, Vector2 point, string pointName)
        //{
        //    //point to local position
        //    Vector2 pos = (Vector2)transform.position + point;

        //    //draw handle
        //    Handles.Label(pos + Vector2.down * 0.08f, pointName);
        //    pos = Handles.PositionHandle(pos, Quaternion.identity);

        //    return pos - (Vector2)transform.position;
        //}
    }
}
#endif