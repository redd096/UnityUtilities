using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.UIControl
{
    /// <summary>
    /// Same as DynamicContentSizeFitter but with max size
    /// </summary>
    [AddComponentMenu("redd096/UIControl/Custom Components/Content Size Fitter/Dynamic Content Size Fitter With Limits")]
    public class DynamicContentSizeFitterWithLimits : DynamicContentSizeFitter
    {
        [Header("Content max limits")]
        [SerializeField] bool m_limitWidth;
        [SerializeField] float m_maxWidth = 100;
        [SerializeField] bool m_limitHeight;
        [SerializeField] float m_maxHeight = 100;

        /// <summary>
        /// Set max width for this content?
        /// </summary>
        public bool limitWidth { get { return m_limitWidth; } set { m_limitWidth = value; SetDirty(); } }

        /// <summary>
        /// Max width for this content
        /// </summary>
        public float maxWidth { get { return m_maxWidth; } set { m_maxWidth = value; SetDirty(); } }

        /// <summary>
        /// Set max height for this content?
        /// </summary>
        public bool limitHeight { get { return m_limitHeight; } set { m_limitHeight = value; SetDirty(); } }

        /// <summary>
        /// Max height for this content
        /// </summary>
        public float maxHeight { get { return m_maxHeight; } set { m_maxHeight = value; SetDirty(); } }

        protected override float DynamicSize(float size, int axis, Vector2 defaultSize)
        {
            //limit size
            if (axis == 0)
            {
                if (m_limitWidth && size > m_maxWidth)
                    size = m_maxWidth;
            }
            else
            {
                if (m_limitHeight && size > m_maxHeight)
                    size = m_maxHeight;
            }

            //do normally dynamic size checks
            return base.DynamicSize(size, axis, defaultSize);
        }
    }

    #region custom editor
#if UNITY_EDITOR

    [CustomEditor(typeof(DynamicContentSizeFitterWithLimits), true)]
    [CanEditMultipleObjects]
    public class DynamicContentSizeFitterWithLimitsEditor : DynamicContentSizeFitterEditor
    {
        SerializedProperty m_limitWidth;
        SerializedProperty m_maxWidth;
        SerializedProperty m_limitHeight;
        SerializedProperty m_maxHeight;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_limitWidth = serializedObject.FindProperty("m_limitWidth");
            m_maxWidth = serializedObject.FindProperty("m_maxWidth");
            m_limitHeight = serializedObject.FindProperty("m_limitHeight");
            m_maxHeight = serializedObject.FindProperty("m_maxHeight");
        }

        public override void OnInspectorGUI()
        {
            //show base class inspector
            base.OnInspectorGUI();
            //DrawDefaultInspector();

            //override content size fitter editor, to show also new vars
            ShowNewVars();
        }

        void ShowNewVars()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_limitWidth, true);
            EditorGUILayout.PropertyField(m_maxWidth, true);
            EditorGUILayout.PropertyField(m_limitHeight, true);
            EditorGUILayout.PropertyField(m_maxHeight, true);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
    #endregion
}