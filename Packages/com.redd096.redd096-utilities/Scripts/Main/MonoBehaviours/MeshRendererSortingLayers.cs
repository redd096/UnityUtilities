using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace redd096
{
    [AddComponentMenu("redd096/Main/MonoBehaviours/Mesh Renderer Sorting Layers")]
    public class MeshRendererSortingLayers : MonoBehaviour
    {
    }

#if UNITY_EDITOR

    //show sorting layer and order in layer on 3D mesh (to show above or bottom sprite renderers)
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(MeshRendererSortingLayers))]
    public class MeshRendererSortingLayersEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //get every mesh renderer
            List<Object> objs = new List<Object>();
            foreach (Object obj in serializedObject.targetObjects)
            {
                GameObject go = GetGameObject(obj);
                if (go)
                {
                    MeshRenderer mr = go.GetComponent<MeshRenderer>();
                    if (mr)
                        objs.Add(mr);
                }
            }

            if (objs == null || objs.Count <= 0)
                return;

            //create serialized object
            SerializedObject so = new SerializedObject(objs.ToArray());

            //get sorting layer and order in layer properties
            SerializedProperty sortingLayerID = so.FindProperty(propertyPath: "m_SortingLayerID");
            SerializedProperty sortingOrder = so.FindProperty("m_SortingOrder");

            //sorting layer
            Rect rectSortingLayer = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginProperty(rectSortingLayer, GUIContent.none, sortingLayerID); //used only to show changes to save
            string[] layerNames = GetSortingLayerNames();                               //get layer names to show
            int[] layerIDs = GetSortingLayerUniqueIDs();                                //get layer ids
            int selected = GetSelectedSortingLayer(sortingLayerID, layerIDs);
            selected = EditorGUILayout.Popup("Sorting Layer", selected, layerNames);    //show dropdown
            sortingLayerID.intValue = layerIDs[selected];                               //set property value
            EditorGUI.EndProperty();
            EditorGUILayout.EndHorizontal();

            //order in layer
            EditorGUILayout.PropertyField(sortingOrder, new GUIContent("Order in Layer"));

            //apply modified properties
            so.ApplyModifiedProperties();
        }

        private GameObject GetGameObject(Object obj)
        {
            //return GameObject (cast obj as GameObject or Component)
            if (obj is GameObject)
                return obj as GameObject;
            else
                return (obj as Component).gameObject;
        }

        string[] GetSortingLayerNames()
        {
            //get sorting layer names
            System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        int[] GetSortingLayerUniqueIDs()
        {
            //get sorting layer IDs
            System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
        }

        int GetSelectedSortingLayer(SerializedProperty sortingLayerID, int[] layerIDs)
        {
            int selected = -1;
            int sID = sortingLayerID.intValue;

            //find sorting layer ID
            for (int i = 0; i < layerIDs.Length; i++)
                if (sID == layerIDs[i])
                    selected = i;

            //if not found, get layer ID 0
            if (selected == -1)
                for (int i = 0; i < layerIDs.Length; i++)
                    if (layerIDs[i] == 0)
                        selected = i;

            return selected;
        }
    }

#endif
}