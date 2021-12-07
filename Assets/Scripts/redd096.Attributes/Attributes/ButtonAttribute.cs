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
                ButtonAttribute buttonAttribute = method.GetCustomAttribute<ButtonAttribute>(true);
                if (buttonAttribute != null)
                {
                    //if the user clicks the button, invoke the method (show button name or method name)
                    if (GUILayout.Button(string.IsNullOrEmpty(buttonAttribute.buttonName) ? method.Name : buttonAttribute.buttonName))
                    {
                        method.Invoke(target, null);

                        //repaint scene
                        SceneView.RepaintAll();
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
        public readonly string buttonName;

        /// <summary>
        /// Attribute to show button in inspector
        /// </summary>
        /// <param name="buttonName">Name of the button (default is method name)</param>
        public ButtonAttribute(string buttonName = "")
        {            
            this.buttonName = buttonName;
        }
    }
}