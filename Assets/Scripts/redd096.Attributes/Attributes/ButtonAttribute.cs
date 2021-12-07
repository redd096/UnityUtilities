using System;
using System.Reflection;
using UnityEngine;

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //get the type of monobehaviour
            var type = target.GetType();

            //get every method inside this
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                //make sure it is decorated by our custom attribute
                if (method.GetCustomAttribute<ButtonAttribute>(true) != null)
                {
                    //if the user clicks the button, invoke the method
                    if (GUILayout.Button(method.Name))
                    {
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }

#endif

    #endregion

    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {

    }
}