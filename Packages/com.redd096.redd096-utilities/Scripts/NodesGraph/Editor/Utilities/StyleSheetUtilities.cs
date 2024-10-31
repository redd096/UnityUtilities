#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// StyleSheet to set graphic in the editor window
    /// </summary>
    public static class StyleSheetUtilities
    {
        public const string VARIABLES_STYLES_PATH = "Packages/com.redd096.redd096-utilities/Scripts/NodesGraph/Editor/Styles/Variables.uss";
        public const string GRAPH_VIEW_STYLES_PATH = "Packages/com.redd096.redd096-utilities/Scripts/NodesGraph/Editor/Styles/GraphViewStyles.uss";
        public const string NODE_STYLES_PATH = "Packages/com.redd096.redd096-utilities/Scripts/NodesGraph/Editor/Styles/NodeStyles.uss";
        public const string MINIMAP_STYLES_PATH = "Packages/com.redd096.redd096-utilities/Scripts/NodesGraph/Editor/Styles/MinimapStyles.uss";
        public const string TOOLBAR_STYLES_PATH = "Packages/com.redd096.redd096-utilities/Scripts/NodesGraph/Editor/Styles/ToolbarStyles.uss";

        /// <summary>
        /// If styleSheet is null load it with LoadAssetAtPath(styleSheetPath), then call VisualElement.styleSheets.Add(styleSheet)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="styleSheet"></param>
        /// <param name="styleSheetPath"></param>
        public static void AddStyleSheets(VisualElement element, StyleSheet styleSheet, string styleSheetPath)
        {
            if (styleSheet == null)
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);

            element.styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// Call VisualElement.AddToClassList(className)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="classNames"></param>
        public static void AddToClassesList(VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }
        }

        /// <summary>
        /// Call VisualElement.AddToClassList(className)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="classNames"></param>
        public static void RemoveFromClassesList(VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.RemoveFromClassList(className);
            }
        }

        /// <summary>
        /// Call VisualElement.ToggleInClassList(className)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="classNames"></param>
        public static void ToggleInClassesList(VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.ToggleInClassList(className);
            }
        }
    }
}
#endif