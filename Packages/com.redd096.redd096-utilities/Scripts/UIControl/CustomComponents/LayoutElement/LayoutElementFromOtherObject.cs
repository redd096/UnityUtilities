using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.UIControl
{
    /// <summary>
    /// Add this component to a GameObject to make it into a layout element or override values on an existing layout element. 
    /// Instead of set values, get them from another object. 
    /// NB if the toggle isn't enabled, they aren't set. If the toggle is enabled, we get value from otherObject
    /// </summary>
    [AddComponentMenu("redd096/UIControl/Custom Components/Layout Element/Layout Element From Other Object")]
    public class LayoutElementFromOtherObject : LayoutElement
    {
        //create a copy to edit in custom editor (-1 do not update, >= 0 copy from otherObject)
        [SerializeField] private float new_MinWidth = -1;
        [SerializeField] private float new_MinHeight = -1;
        [SerializeField] private float new_PreferredWidth = -1;
        [SerializeField] private float new_PreferredHeight = -1;
        [SerializeField] private float new_FlexibleWidth = -1;
        [SerializeField] private float new_FlexibleHeight = -1;
        [SerializeField] RectTransform otherObject;

        private void Update()
        {
            minWidth = new_MinWidth < 0 ? -1 : LayoutUtility.GetMinWidth(otherObject);
            minHeight = new_MinHeight < 0 ? -1 : LayoutUtility.GetMinHeight(otherObject);
            preferredWidth = new_PreferredWidth < 0 ? -1 : LayoutUtility.GetPreferredWidth(otherObject);
            preferredHeight = new_PreferredHeight < 0 ? -1 : LayoutUtility.GetPreferredHeight(otherObject);
            flexibleWidth = new_FlexibleWidth < 0 ? -1 : LayoutUtility.GetFlexibleWidth(otherObject);
            flexibleHeight = new_FlexibleHeight < 0 ? -1 : LayoutUtility.GetFlexibleHeight(otherObject);
        }
    }

    #region custom editor
#if UNITY_EDITOR

    //COPY-PASTE from LayoutElementEditor, but serialize new_value instead of m_value and without FloatField (value are set by otherObject). And serialize also otherObject
    [CustomEditor(typeof(LayoutElementFromOtherObject), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom editor for the LayoutElement component
    ///   Extend this class to write a custom editor for a component derived from LayoutElement.
    /// </summary>
    public class LayoutElementWithLimitsEditor : Editor
    {
        SerializedProperty m_IgnoreLayout;
        SerializedProperty new_MinWidth;
        SerializedProperty new_MinHeight;
        SerializedProperty new_PreferredWidth;
        SerializedProperty new_PreferredHeight;
        SerializedProperty new_FlexibleWidth;
        SerializedProperty new_FlexibleHeight;
        SerializedProperty m_LayoutPriority;
        SerializedProperty otherObject; //added

        protected virtual void OnEnable()
        {
            //base.OnEnable();
            m_IgnoreLayout = serializedObject.FindProperty("m_IgnoreLayout");
            new_MinWidth = serializedObject.FindProperty("new_MinWidth");
            new_MinHeight = serializedObject.FindProperty("new_MinHeight");
            new_PreferredWidth = serializedObject.FindProperty("new_PreferredWidth");
            new_PreferredHeight = serializedObject.FindProperty("new_PreferredHeight");
            new_FlexibleWidth = serializedObject.FindProperty("new_FlexibleWidth");
            new_FlexibleHeight = serializedObject.FindProperty("new_FlexibleHeight");
            m_LayoutPriority = serializedObject.FindProperty("m_LayoutPriority");
            otherObject = serializedObject.FindProperty("otherObject"); //added
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_IgnoreLayout);

            if (!m_IgnoreLayout.boolValue)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(otherObject); //added
                LayoutElementField(new_MinWidth, 0);
                LayoutElementField(new_MinHeight, 0);
                LayoutElementField(new_PreferredWidth, t => t.rect.width);
                LayoutElementField(new_PreferredHeight, t => t.rect.height);
                LayoutElementField(new_FlexibleWidth, 1);
                LayoutElementField(new_FlexibleHeight, 1);
            }

            EditorGUILayout.PropertyField(m_LayoutPriority);

            serializedObject.ApplyModifiedProperties();
        }

        //copy-paste from LayoutElementEditor
        void LayoutElementField(SerializedProperty property, float defaultValue)
        {
            LayoutElementField(property, _ => defaultValue);
        }

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
                //REMOVED FLOAT FIELD BECAUSE THIS COMPONENT TAKE VALUE FROM OtherObject ==================================
                string s = otherObject.objectReferenceValue != null ? otherObject.objectReferenceValue.name : "Null";
                floatFieldRect.x += 4;  //small space
                EditorGUI.LabelField(floatFieldRect, new GUIContent($"Get from [{s}]"));
                //// Float field
                //EditorGUIUtility.labelWidth = 4; // Small invisible label area for drag zone functionality
                //EditorGUI.BeginChangeCheck();
                //float newValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(" "), property.floatValue);
                //if (EditorGUI.EndChangeCheck())
                //{
                //    property.floatValue = Mathf.Max(0, newValue);
                //}
                //EditorGUIUtility.labelWidth = 0;
                //=========================================================================================================
            }

            EditorGUI.EndProperty();
        }
    }

#endif
    #endregion
}