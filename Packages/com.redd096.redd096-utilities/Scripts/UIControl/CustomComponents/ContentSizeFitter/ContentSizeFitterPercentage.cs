using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace redd096.UIControl
{
    /// <summary>
    /// Content Size Fitter, but instead of resize based on child preferred or min size, set Width and Height as a Percentage based on parent
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("redd096/UIControl/Custom Components/Content Size Fitter/Content Size Fitter Percentage")]
    public class ContentSizeFitterPercentage : UIBehaviour, ILayoutSelfController
    {
        #region vars

        [Header("Percentage (from 0 to 1)")]
        [SerializeField] bool m_usePercentageWidth;
        [SerializeField] float m_widthPercentage = 1;
        [SerializeField] bool m_usePercentageHeight;
        [SerializeField] float m_heightPercentage = 1;

        /// <summary>
        /// Set width as percentage for this content?
        /// </summary>
        public bool usePercentageWidth { get { return m_usePercentageWidth; } set { m_usePercentageWidth = value; SetDirty(); } }
        /// <summary>
        /// Width in percentage for this content
        /// </summary>
        public float widthPercentage { get { return m_widthPercentage; } set { m_widthPercentage = value; SetDirty(); } }
        /// <summary>
        /// Set height as percentage for this content?
        /// </summary>
        public bool usePercentageHeight { get { return m_usePercentageHeight; } set { m_usePercentageHeight = value; SetDirty(); } }
        /// <summary>
        /// Height in percentage for this content
        /// </summary>
        public float heightPercentage { get { return m_heightPercentage; } set { m_heightPercentage = value; SetDirty(); } }

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

        //parent rect transform
        [System.NonSerialized] private RectTransform m_parentRect;
        private RectTransform parentRectTransform
        {
            get
            {
                if (m_parentRect == null && transform.parent)
                    m_parentRect = transform.parent.GetComponent<RectTransform>();
                return m_parentRect;
            }
        }

        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;   //this is used to set not editable Width and Height in inspector for this RectTransform
#pragma warning restore 649

        #endregion

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            SetDirty();
        }

#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        protected ContentSizeFitterPercentage()
        { }

        /// <summary>
        /// Calculate and apply the horizontal component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        /// <summary>
        /// Calculate and apply the vertical component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            if (axis == 0)
            {
                //width percentage
                if (m_usePercentageWidth)
                {
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);

                    Rect parentRect = parentRectTransform ? parentRectTransform.rect : Rect.zero;
                    float size = parentRect.width * m_widthPercentage;
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, size);
                    return;
                }
            }
            else
            {
                //height percentage
                if (m_usePercentageHeight)
                {
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);

                    Rect parentRect = parentRectTransform ? parentRectTransform.rect : Rect.zero;
                    float size = parentRect.height * m_heightPercentage;
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, size);
                    return;
                }
            }

            //if don't control this size, keep a reference to the tracked transform, but don't control its properties:
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.None);
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }
    }

    #region custom editor
#if UNITY_EDITOR

    /// <summary>
    /// Custom Editor for the ContentSizeFitterPercentage Component.
    /// Extend this class to write a custom editor for a component derived from ContentSizeFitterPercentage.
    /// </summary>
    [CustomEditor(typeof(ContentSizeFitterPercentage), true)]
    [CanEditMultipleObjects]
    public class ContentSizeFitterPercentageEditor : SelfControllerEditor
    {
        SerializedProperty m_usePercentageWidth;
        SerializedProperty m_widthPercentage;
        SerializedProperty m_usePercentageHeight;
        SerializedProperty m_heightPercentage;

        protected virtual void OnEnable()
        {
            m_usePercentageWidth = serializedObject.FindProperty("m_usePercentageWidth");
            m_widthPercentage = serializedObject.FindProperty("m_widthPercentage");
            m_usePercentageHeight = serializedObject.FindProperty("m_usePercentageHeight");
            m_heightPercentage = serializedObject.FindProperty("m_heightPercentage");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_usePercentageWidth, true);
            EditorGUILayout.PropertyField(m_widthPercentage, true);
            EditorGUILayout.PropertyField(m_usePercentageHeight, true);
            EditorGUILayout.PropertyField(m_heightPercentage, true);
            serializedObject.ApplyModifiedProperties();

            //show base class inspector
            base.OnInspectorGUI();
        }
    }

#endif
    #endregion
}
