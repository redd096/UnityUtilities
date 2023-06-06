using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace redd096
{
    [AddComponentMenu("redd096/UI Control/Dynamic Content Size Fitter")]
    public class DynamicContentSizeFitter : ContentSizeFitter
    {
        #region vars
        /// <summary>
        /// The dynamic size fit modes avaliable to use.
        /// </summary>
        public enum EDynamicFitMode
        {
            /// <summary>
            /// Resize to the rect, always.
            /// </summary>
            FollowAlwaysRect,
            /// <summary>
            /// Resize to the rect, only when it exceed content size.
            /// </summary>
            ExceedSize,
            /// <summary>
            /// Resize to the rect, only when it's smaller than content size.
            /// </summary>
            SmallerSize,
            /// <summary>
            /// Keep content size as minimum, increase it when rect is increased
            /// </summary>
            IncreaseWithRect,
        }

        /// <summary>
        /// How is calculated the content default size
        /// </summary>
        public enum EDefaultSizeMode
        {
            /// <summary>
            /// Use SizeDelta and Parent.rect
            /// </summary>
            DeltaAndParentRect,
            /// <summary>
            /// Use only SizeDelta
            /// </summary>
            Delta
        }

        [Header("Content size based on rect to check")]
        [SerializeField] EDynamicFitMode m_dynamicFitMode = EDynamicFitMode.ExceedSize;
        [SerializeField] EDefaultSizeMode m_defaultSizeMode = EDefaultSizeMode.DeltaAndParentRect;
        [SerializeField] Vector2 m_sizeDelta = Vector2.one * 100;
        [SerializeField] RectTransform m_rectToCheck = default;

        [Header("Default rect to check size")]
        [SerializeField] Vector2 m_rectToCheckDefaultSize = Vector2.one * 100;  //IncreaseWithRect - if rectToCheck exceed default size by 10, increase content size by 10

        /// <summary>
        /// The dynamic fit mode to use to determine the size.
        /// </summary>
        public EDynamicFitMode dynamicFitMode { get { return m_dynamicFitMode; } set { m_dynamicFitMode = value; SetDirty(); } }
        /// <summary>
        /// How to calculate the content default size
        /// </summary>
        public EDefaultSizeMode defaultSizeMode { get { return m_defaultSizeMode; } set { m_defaultSizeMode = value; SetDirty(); } }
        /// <summary>
        /// The normal size delta to use when we aren't using rect size
        /// </summary>
        public Vector2 sizeDelta { get { return m_sizeDelta; } set { m_sizeDelta = value; SetDirty(); } }
        /// <summary>
        /// The rect we are checking to set the content size
        /// </summary>
        public RectTransform rectToCheck { get { return m_rectToCheck; } set { m_rectToCheck = value; SetDirty(); } }
        /// <summary>
        /// Default size of the rect to check
        /// </summary>
        public Vector2 rectToCheckDefaultSize { get { return m_rectToCheckDefaultSize; } set { m_rectToCheckDefaultSize = value; SetDirty(); } }

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

        //parent rect transform, used to calculate sizeDelta
        [System.NonSerialized] private RectTransform m_parentRect;
        private RectTransform parentRectTransform
        {
            get
            {
                //do not use Canvas as parent (for example in prefabs)
                if (m_parentRect == null && transform.parent && transform.parent.GetComponent<Canvas>() == null)
                    m_parentRect = transform.parent.GetComponent<RectTransform>();
                return m_parentRect;
            }
        }
        #endregion

        public override void SetLayoutHorizontal()
        {
            //base.SetLayoutHorizontal();
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
                return;
            }

            Vector2 previousSize = rectTransform.sizeDelta;

            //float foundSize = (fitting == FitMode.MinSize) ? 
            //    DynamicSize(LayoutUtility.GetMinSize(m_rectToCheck, axis), axis) : 
            //    DynamicSize(LayoutUtility.GetPreferredSize(m_rectToCheck, axis), axis);
            //
            //Debug.Log($"<color=green>BEFORE - delta: {sizeDelta} - parent rect width: {parentRectTransform.rect.width} - otherRect width: {rectToCheck.rect.width} - " +
            //    $"current rect width: {rectTransform.rect.width} -  found size: {foundSize} - current sizeDelta: {rectTransform.sizeDelta}</color>");
            //
            //rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, foundSize);

            // Set size to min or preferred size
            if (fitting == FitMode.MinSize)
                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, DynamicSize(LayoutUtility.GetMinSize(m_rectToCheck, axis), axis));
            else
                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, DynamicSize(LayoutUtility.GetPreferredSize(m_rectToCheck, axis), axis));

            //Debug.Log($"<color=green>AFTER - delta: {sizeDelta} - parent rect width: {parentRectTransform.rect.width} - otherRect width: {rectToCheck.rect.width} - " +
            //    $"current rect width: {rectTransform.rect.width} -  found size: {foundSize} - current sizeDelta: {rectTransform.sizeDelta}</color>");

#if UNITY_EDITOR
            if (Application.isPlaying == false && previousSize != rectTransform.sizeDelta)
                EditorUtility.SetDirty(gameObject);
#endif
        }

        float DynamicSize(float size, int axis)
        {
            //if follow always rect, return its size
            if (dynamicFitMode == EDynamicFitMode.FollowAlwaysRect)
                return size;

            //else calculate normal content size
            float contentSize = axis == 0 ? sizeDelta.x : sizeDelta.y;
            if (defaultSizeMode == EDefaultSizeMode.DeltaAndParentRect)
            {
                //sizeDelta = rectTransform.rect.size - parentRectTransform.rect.size
                //so rectTransform.rect.size = parentRectTransform.rect.size + sizeDelta
                Rect parentRect = parentRectTransform ? parentRectTransform.rect : Rect.zero;   //if there is no parent, contentSize will be same as sizeDelta
                contentSize = axis == 0 ? parentRect.width + sizeDelta.x : parentRect.height + sizeDelta.y;
            }

            //check if exceed max or min size
            if ((dynamicFitMode == EDynamicFitMode.ExceedSize && size > contentSize) ||         //if rect exceed content size
                (dynamicFitMode == EDynamicFitMode.SmallerSize && size < contentSize))          //or rect is smaller than content size
            {
                return size;
            }

            //check if need to increase with rect
            if (dynamicFitMode == EDynamicFitMode.IncreaseWithRect)
            {
                float rectDefaultSize = axis == 0 ? m_rectToCheckDefaultSize.x : m_rectToCheckDefaultSize.y;
                //Debug.Log($"axis {axis} - contentSize {contentSize} - defaultRect {rectDefaultSize} - currentRect {size} - result {contentSize + size - rectDefaultSize}");
                if (size > rectDefaultSize)
                {
                    //increase by the same amount as rect to check
                    contentSize += size - rectDefaultSize;
                }
            }

            return contentSize;
        }
    }

    #region custom editor
#if UNITY_EDITOR

    [CustomEditor(typeof(DynamicContentSizeFitter), true)]
    [CanEditMultipleObjects]
    public class DynamicContentSizeFitterEditor : ContentSizeFitterEditor
    {
        SerializedProperty m_dynamicFitMode;
        SerializedProperty m_defaultSizeMode;
        SerializedProperty m_sizeDelta;
        SerializedProperty m_rectToCheck;
        SerializedProperty m_rectToCheckDefaultSize;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_dynamicFitMode = serializedObject.FindProperty("m_dynamicFitMode");
            m_defaultSizeMode = serializedObject.FindProperty("m_defaultSizeMode");
            m_sizeDelta = serializedObject.FindProperty("m_sizeDelta");
            m_rectToCheck = serializedObject.FindProperty("m_rectToCheck");
            m_rectToCheckDefaultSize = serializedObject.FindProperty("m_rectToCheckDefaultSize");
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
            EditorGUILayout.PropertyField(m_dynamicFitMode, true);
            EditorGUILayout.PropertyField(m_defaultSizeMode, true);
            EditorGUILayout.PropertyField(m_sizeDelta, true);
            EditorGUILayout.PropertyField(m_rectToCheck, true);

            //show this only with specifics dynamic fit mode
            if ((DynamicContentSizeFitter.EDynamicFitMode)m_dynamicFitMode.enumValueIndex == DynamicContentSizeFitter.EDynamicFitMode.IncreaseWithRect)
                EditorGUILayout.PropertyField(m_rectToCheckDefaultSize, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    #endregion
}