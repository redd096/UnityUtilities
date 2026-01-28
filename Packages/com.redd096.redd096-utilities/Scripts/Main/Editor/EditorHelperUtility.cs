#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace redd096
{
    public static class EditorHelperUtility
    {
        public static void ShowScript(Object target)
        {
            //show script on top
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript) /*serializedObject.GetType()*/, false);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif