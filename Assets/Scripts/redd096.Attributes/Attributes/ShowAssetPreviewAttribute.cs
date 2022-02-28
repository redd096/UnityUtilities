using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ShowAssetPreviewAttribute))]
    public class ShowAssetPreviewDrawer : PropertyDrawer
    {
        ShowAssetPreviewAttribute at;
        Texture textureToShow;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //if there is texture, add height and 10 (a bit of space between vars)
            return EditorGUI.GetPropertyHeight(property, label, true) + (at != null && textureToShow ? at.height + 10 : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //draw property
            EditorGUI.PropertyField(position, property, label, true);

            //get attribute
            at = attribute as ShowAssetPreviewAttribute;

            //get texture or sprite
            textureToShow = property.objectReferenceValue as Texture;
            if (textureToShow == null && property.objectReferenceValue is Sprite)
                textureToShow = ((Sprite)property.objectReferenceValue).texture;

            //show texture preview
            if (at != null && textureToShow)
            {
                EditorGUI.DrawPreviewTexture(new Rect(position.x, position.y + position.height - at.height - 5, at.width, at.height), textureToShow);
            }
        }
    }

#endif

    #endregion

    /// <summary>
    /// Show sprite or texture preview
    /// </summary>
    public class ShowAssetPreviewAttribute : PropertyAttribute
    {
        public readonly int width;
        public readonly int height;

        public ShowAssetPreviewAttribute(int width = 60, int height = 60)
        {
            this.width = width;
            this.height = height;
        }
    }
}