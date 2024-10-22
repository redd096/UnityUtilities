using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace redd096.UIControl
{
    /// <summary>
    /// Content Size Fitter, but with max size
    /// </summary>
    [AddComponentMenu("redd096/UIControl/Custom Components/Content Size Fitter/Content Size Fitter With Limits")]
    public class ContentSizeFitterWithLimits : ContentSizeFitter
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

        //this object's rect transform (ContentSizeFitter copy-paste)
        [System.NonSerialized] private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;   //this is used to set not editable Width and Height in inspector for this RectTransform
#pragma warning restore 649

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        public override void SetLayoutHorizontal()
        {
            //base.SetLayoutHorizontal();
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            //base.SetLayoutVertical();
            HandleSelfFittingAlongAxis(1);
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            //Unconstrained, do nothing
            FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
            if (fitting == FitMode.Unconstrained)
            {
                // Keep a reference to the tracked transform, but don't control its properties:
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.None);
                return;
            }

            m_Tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

            // Set size to min or preferred size
            float size;
            if (fitting == FitMode.MinSize)
                size = LayoutUtility.GetMinSize(m_Rect, axis);
            else
                size = LayoutUtility.GetPreferredSize(m_Rect, axis);

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

            rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, size);
        }
    }

    #region custom editor
#if UNITY_EDITOR

    [CustomEditor(typeof(ContentSizeFitterWithLimits), true)]
    [CanEditMultipleObjects]
    public class ContentSizeFitterWithLimitsEditor : ContentSizeFitterEditor
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