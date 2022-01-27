#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    public static class AttributeUtility
    {
#if UNITY_EDITOR

        /// <summary>
        /// Find property also if inside an array
        /// </summary>
        /// <param name="property"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SerializedProperty FindCorrectProperty(this SerializedProperty property, string path)
        {
            //if in array, get parent and find property in it
            SerializedProperty parent = GetParent(property);
            if (parent != null)
            {
                return parent.FindPropertyRelative(path);
            }

            //else find property normally
            return property.serializedObject.FindProperty(path);
        }

        /// <summary>
        /// Return parent inside array. From this can return its properties calling FindPropertyRelative(path)
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static SerializedProperty GetParent(this SerializedProperty property)
        {
            //if in array, path will be propertyName.Array.data[index].VariableName (where VariableName is the name of the var you are looking for)
            string path = property.propertyPath;
            int i = path.LastIndexOf('.');

            //if there is no point, is not an array
            if (i < 0)
                return null;

            //if finish with "Array" without data[index], then this property is already an array, find a property inside this
            path = path.Substring(0, i);
            if (path.Substring(i - 5, 5) == "Array")
                return property;

            //else find the property containing variableName
            return property.serializedObject.FindProperty(path);
        }

#endif
    }
}