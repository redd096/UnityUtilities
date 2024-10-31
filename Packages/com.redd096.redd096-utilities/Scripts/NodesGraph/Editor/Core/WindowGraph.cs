#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// Window in editor to visualize Graph
    /// </summary>
    public class WindowGraph : EditorWindow
    {
        //styles are here instead of StyleSheetUtilities, just to set them in inspector -> click on the script in the project and you can set these variables
        [SerializeField] protected StyleSheet variablesStyles;
        [SerializeField] protected StyleSheet graphViewStyles;
        [SerializeField] protected StyleSheet nodeStyles;
        [SerializeField] protected StyleSheet minimapStyles;
        [SerializeField] protected StyleSheet toolbarStyles;

        protected VisualElement graph;
        protected VisualElement toolbar;

        [MenuItem("Tools/redd096/Nodes Graph")]
        public static void ShowNodesGraphWindow()
        {
            //open window (and set title)
            GetWindow<WindowGraph>("Nodes Graph");
        }

        protected virtual void CreateGUI()
        {
            //create elements
            CreateGraph();
            CreateToolbar();
            AddElementsToRoot();

            //and add styles
            AddStyles();
        }

        /// <summary>
        /// Create graph
        /// </summary>
        protected virtual void CreateGraph()
        {
            graph = new NodesGraphView(this);
            graph.StretchToParentSize();
        }

        /// <summary>
        /// Create toolbar
        /// </summary>
        protected virtual void CreateToolbar()
        {
            toolbar = new NodesGraphToolbar(graph as NodesGraphView);
        }

        /// <summary>
        /// Add graph and toolbar to the root, to visualize them
        /// </summary>
        protected virtual void AddElementsToRoot()
        {
            rootVisualElement.Add(graph);
            rootVisualElement.Add(toolbar);
        }

        /// <summary>
        /// Add styles to visual elements
        /// </summary>
        protected virtual void AddStyles()
        {
            StyleSheetUtilities.AddStyleSheets(rootVisualElement, variablesStyles, StyleSheetUtilities.VARIABLES_STYLES_PATH);
            StyleSheetUtilities.AddStyleSheets(graph, graphViewStyles, StyleSheetUtilities.GRAPH_VIEW_STYLES_PATH);
            StyleSheetUtilities.AddStyleSheets(graph, nodeStyles, StyleSheetUtilities.NODE_STYLES_PATH);
            StyleSheetUtilities.AddStyleSheets(graph, minimapStyles, StyleSheetUtilities.MINIMAP_STYLES_PATH);
            StyleSheetUtilities.AddStyleSheets(toolbar, toolbarStyles, StyleSheetUtilities.TOOLBAR_STYLES_PATH);
        }
    }
}
#endif