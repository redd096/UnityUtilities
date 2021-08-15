namespace redd096
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class VisualStateMachine : MonoBehaviour
    {
#if UNITY_EDITOR
        //window used to visualize this state machine
        public WindowStateMachine CurrentWindow { get; set; }
#endif

        protected VisualState state;

        /// <summary>
        /// Call it to change state
        /// </summary>
        public void SetState(VisualState stateToSet)
        {
            VisualState previousState = state;

            //set new one
            state = stateToSet;

            //exit from previous
            if (previousState != null)
                previousState.Exit();

            //enter in new one
            if (state != null)
            {
                state.AwakeState(this);
                state.Enter();
            }
        }

        protected virtual void Update()
        {
            state?.Update();
        }

        protected virtual void FixedUpdate()
        {
            state?.FixedUpdate();
        }
    }

#if UNITY_EDITOR
    #region window editor

    public class WindowStateMachine : EditorWindow
    {
        Vector2 scrollPosition = Vector2.zero;
        VisualStateMachine visualStateMachine;

        /// <summary>
        /// Set owner of this window
        /// </summary>
        /// <param name="visualStateMachine"></param>
        public void Init(VisualStateMachine visualStateMachine)
        {
            this.visualStateMachine = visualStateMachine;
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.Space(15);

            //show menu on click
            MenuOnClick();

            EditorGUILayout.Space(15);
            EditorGUILayout.EndScrollView();
        }

        void MenuOnClick()
        {
            //if right click on this window
            if (mouseOverWindow == this && Event.current.type == EventType.ContextClick)
            {
                GenericMenu menu = new GenericMenu();

                menu.AddDisabledItem(new GUIContent($"I clicked on {visualStateMachine.name}"));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Add State"), false, () => Debug.Log("daje"));
                menu.ShowAsContext();

                Event.current.Use();
            }
        }
    }

    #endregion

    #region custom editor

    [CustomEditor(typeof(VisualStateMachine), true)]
    [CanEditMultipleObjects]
    public class VisualStateMachineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VisualStateMachine visualStateMachine = target as VisualStateMachine;
            if (visualStateMachine == null)
                return;

            //if click button
            if (GUILayout.Button("Open StateMachine"))
            {
                //show window and focus on it
                if (visualStateMachine.CurrentWindow != null)
                {
                    visualStateMachine.CurrentWindow.Show();
                    visualStateMachine.CurrentWindow.Focus();
                    return;
                }

                //else create it and initialize
                visualStateMachine.CurrentWindow = EditorWindow.CreateWindow<WindowStateMachine>(visualStateMachine.name);
                visualStateMachine.CurrentWindow.Init(visualStateMachine);
            }

            base.OnInspectorGUI();
        }
    }

    #endregion
#endif
}