using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace redd096.UIControl
{
    /// <summary>
    /// Add this component to a GameObject to make it into a layout element or override values on an existing layout element. 
    /// This add MaxWidth and MaxHeight values.
    /// NB This affect PreferredWidth and PreferredHeight, they can't go over Max
    /// </summary>
    [AddComponentMenu("redd096/UIControl/Custom Components/Layout Element/Layout Element With Limits")]
    public class LayoutElementWithLimits : LayoutElement
    {
        [SerializeField] private float m_MaxWidth = -1;
        [SerializeField] private float m_MaxHeight = -1;

        private RectTransform m_RectTransform;
        private RectTransform rect { get { if (m_RectTransform == null) m_RectTransform = transform as RectTransform; return m_RectTransform; } }

        public virtual float maxWidth { get { return m_MaxWidth; } set { if (/*SetPropertyUtility.*/SetStruct(ref m_MaxWidth, value)) SetDirty(); } }
        public virtual float maxHeight { get { return m_MaxHeight; } set { if (/*SetPropertyUtility.*/SetStruct(ref m_MaxHeight, value)) SetDirty(); } }

        //copy-paste from SetPropertyUtility.SetStruct in UnityEngine.UI
        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;
            
            currentValue = newValue;
            return true;
        }

        public override float preferredWidth { get => GetMinValue(base.preferredWidth, maxWidth, axis: 0); set => base.preferredWidth = Mathf.Min(value, maxWidth); }

        public override float preferredHeight { get => GetMinValue(base.preferredHeight, maxHeight, axis: 1); set => base.preferredHeight = Mathf.Min(value, maxHeight); }

        #region fix MaxValue enabled and PreferredValue not

        private float GetMinValue(float preferredValue, float maxValue, int axis)
        {
            //if preferred isn't enabled but max is
            if (preferredValue < 0 && maxValue >= 0)
            {
                //calculate preferred without this layout element
                ILayoutElement dummy;
                if (axis == 0)
                    preferredValue = Mathf.Max(GetLayoutProperty(rect, e => e.minWidth, 0, out dummy), GetLayoutProperty(rect, e => e.preferredWidth, 0, out dummy));
                else
                    preferredValue = Mathf.Max(GetLayoutProperty(rect, e => e.minHeight, 0, out dummy), GetLayoutProperty(rect, e => e.preferredHeight, 0, out dummy));
            }

            return Mathf.Min(preferredValue, maxValue);
        }

        //copy-paste from LayoutUtility.GetLayoutProperty in UnityEngine.UI.
        //But in the for loop now there is a line to ignore this component, and the function isn't static
        public float GetLayoutProperty(RectTransform rect, System.Func<ILayoutElement, float> property, float defaultValue, out ILayoutElement source)
        {
            source = null;
            if (rect == null)
                return 0;
            float min = defaultValue;
            int maxPriority = System.Int32.MinValue;
            var components = ListPool<Component>.Get();
            rect.GetComponents(typeof(ILayoutElement), components);

            var componentsCount = components.Count;
            for (int i = 0; i < componentsCount; i++)
            {
                var layoutComp = components[i] as ILayoutElement;
                if (layoutComp is Behaviour && !((Behaviour)layoutComp).isActiveAndEnabled)
                    continue;

                //THIS - IGNORE THIS COMPONENT ================
                if (layoutComp is LayoutElementWithLimits compToIgnore && compToIgnore == this)
                    continue;
                //==============================================

                int priority = layoutComp.layoutPriority;
                // If this layout components has lower priority than a previously used, ignore it.
                if (priority < maxPriority)
                    continue;
                float prop = property(layoutComp);
                // If this layout property is set to a negative value, it means it should be ignored.
                if (prop < 0)
                    continue;

                // If this layout component has higher priority than all previous ones,
                // overwrite with this one's value.
                if (priority > maxPriority)
                {
                    min = prop;
                    maxPriority = priority;
                    source = layoutComp;
                }
                // If the layout component has the same priority as a previously used,
                // use the largest of the values with the same priority.
                else if (prop > min)
                {
                    min = prop;
                    source = layoutComp;
                }
            }

            ListPool<Component>.Release(components);
            return min;
        }

        #endregion
    }

    #region custom editor
#if UNITY_EDITOR

    [CustomEditor(typeof(LayoutElementWithLimits), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom editor for the LayoutElement component
    ///   Extend this class to write a custom editor for a component derived from LayoutElement.
    /// </summary>
    public class LayoutElementWithLimitsEditor : LayoutElementEditor
    {
        SerializedProperty m_IgnoreLayout;
        SerializedProperty m_MaxWidth;
        SerializedProperty m_MaxHeight;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_IgnoreLayout = serializedObject.FindProperty("m_IgnoreLayout");
            m_MaxWidth = serializedObject.FindProperty("m_MaxWidth");
            m_MaxHeight = serializedObject.FindProperty("m_MaxHeight");
        }

        public override void OnInspectorGUI()
        {
            //show base class inspector
            base.OnInspectorGUI();
            //DrawDefaultInspector();

            //override to show also new vars
            if (!m_IgnoreLayout.boolValue)
                ShowNewVars();
        }

        void ShowNewVars()
        {
            serializedObject.Update();
            LayoutElementField(m_MaxWidth, t => t.rect.width);
            LayoutElementField(m_MaxHeight, t => t.rect.height);

            serializedObject.ApplyModifiedProperties();
        }

        //copy-paste from LayoutElementEditor
        void LayoutElementField(SerializedProperty property, System.Func<RectTransform, float> defaultValue)
        {
            Rect position = EditorGUILayout.GetControlRect();

            // Label
            GUIContent label = EditorGUI.BeginProperty(position, null, property);

            // Rects
            Rect fieldPosition = EditorGUI.PrefixLabel(position, label);

            Rect toggleRect = fieldPosition;
            toggleRect.width = 16;

            Rect floatFieldRect = fieldPosition;
            floatFieldRect.xMin += 16;

            // Checkbox
            EditorGUI.BeginChangeCheck();
            bool enabled = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, property.floatValue >= 0);
            if (EditorGUI.EndChangeCheck())
            {
                // This could be made better to set all of the targets to their initial width, but mimizing code change for now
                property.floatValue = (enabled ? defaultValue((target as LayoutElement).transform as RectTransform) : -1);
            }

            if (!property.hasMultipleDifferentValues && property.floatValue >= 0)
            {
                // Float field
                EditorGUIUtility.labelWidth = 4; // Small invisible label area for drag zone functionality
                EditorGUI.BeginChangeCheck();
                float newValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(" "), property.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = Mathf.Max(0, newValue);
                }
                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.EndProperty();
        }
    }

#endif
    #endregion
}