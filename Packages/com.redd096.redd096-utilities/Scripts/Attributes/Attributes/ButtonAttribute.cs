using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using redd096.Attributes.AttributesEditorUtility;
using UnityEditor.SceneManagement;                //used for PrefabStage, PrefabStageUtility and EditorSceneManager
//using UnityEditor.Experimental.SceneManagement;   //old versions of unity
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Attribute to show button in inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public enum EEnableType { Always, Editor, PlayMode }
        public enum EButtonPosition { Bottom, Top }

        public readonly string buttonName;
        public readonly EEnableType enableType = EEnableType.Always;
        public EButtonPosition buttonPosition = EButtonPosition.Bottom;

        public ButtonAttribute(EEnableType enableType = EEnableType.Always)
        {
            this.enableType = enableType;
        }

        public ButtonAttribute(string buttonName, EEnableType enableType = EEnableType.Always)
        {
            this.buttonName = buttonName;
            this.enableType = enableType;
        }
    }

    #region editor

#if UNITY_EDITOR

    //[CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class ButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //get buttons list
            Dictionary<MethodInfo, ButtonAttribute> buttons = GetButtonsList();

            //show buttons on top
            foreach (var method in buttons.Keys)
            {
                if (buttons[method].buttonPosition == ButtonAttribute.EButtonPosition.Top)
                    DrawButton(buttons[method], method);
            }

            base.OnInspectorGUI();

            //show buttons on bottom
            foreach (var method in buttons.Keys)
            {
                if (buttons[method].buttonPosition == ButtonAttribute.EButtonPosition.Bottom)
                    DrawButton(buttons[method], method);
            }
        }

        Dictionary<MethodInfo, ButtonAttribute> GetButtonsList()
        {
            //get every method inside monobehaviour
            Dictionary<MethodInfo, ButtonAttribute> buttons = new Dictionary<MethodInfo, ButtonAttribute>();
            foreach (MethodInfo method in target.GetMethods())
            {
                //make sure it is decorated by our custom attribute
                ButtonAttribute buttonAttribute = method.GetCustomAttribute<ButtonAttribute>(true);
                if (buttonAttribute != null)
                    buttons[method] = buttonAttribute;
            }
            return buttons;
        }

        void DrawButton(ButtonAttribute buttonAttribute, MethodInfo method)
        {
            //can have only zero or optional parameters
            if (method.HasZeroParameterOrOnlyOptional())
            {
                //set if button is enabled or disabled
                EditorGUI.BeginDisabledGroup(
                    buttonAttribute.enableType == ButtonAttribute.EEnableType.Editor && Application.isPlaying                   //if Editor button, disable when in play mode
                    || buttonAttribute.enableType == ButtonAttribute.EEnableType.PlayMode && Application.isPlaying == false);   //if PlayMode button, disable when in editor

                //if the user clicks the button, invoke the method (show button name or method name)
                string buttonName = string.IsNullOrEmpty(buttonAttribute.buttonName) ? method.Name : buttonAttribute.buttonName;
                if (GUILayout.Button(buttonName))
                {
                    //in editor mode, create undo
                    if (Application.isPlaying == false)
                    {
                        Undo.RegisterCompleteObjectUndo(target, buttonName);
                    }

                    IEnumerator methodCoroutine = method.Invoke(target, method.GetDefaultParameters()) as IEnumerator;          //pass default values, if there are optional parameters

                    //in editor mode set target object and scene dirty to serialize changes to disk
                    if (Application.isPlaying == false)
                    {
                        //I'm not sure SetDirty and MarkSceneDirty are still necessary now that there is the undo
                        EditorUtility.SetDirty(target);

                        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (stage != null)
                            EditorSceneManager.MarkSceneDirty(stage.scene);                             //prefab mode
                        else
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());     //normal scene

                        //save prefab overrides if this is a prefab
                        if (Selection.activeGameObject)
                        {
                            foreach (var component in Selection.activeGameObject.GetComponentsInChildren<Component>())
                            {
                                PrefabUtility.RecordPrefabInstancePropertyModifications(component);
                            }
                        }

                        //repaint scene and inspector
                        SceneView.RepaintAll();
                        Repaint();
                    }
                    //in play mode can call also coroutines
                    else if (methodCoroutine != null && target is MonoBehaviour behaviour)
                    {
                        behaviour.StartCoroutine(methodCoroutine);
                    }
                }

                EditorGUI.EndDisabledGroup();
            }
            else
            {
                Debug.LogWarning(target.name + " can't invoke '" + method.Name + "'. It can invoke only methods with 0 or optional parameters", target);
            }
        }
    }

#endif

    #endregion
}