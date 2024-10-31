#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// Toolbar on top of the Graph, with various buttons and ui elements
    /// </summary>
    public class NodesGraphToolbar : VisualElement
    {
        protected NodesGraphView graph;
        protected Toolbar toolbar;

        protected TextField fileNameTextfield;
        protected Button minimapButton;

        protected string fileName;

        public const string DEFAULT_FILE_NAME = "New File";
        public const string SAVE_FOLDER_NAME = "SavedGraphs";
        public static string SavedFolderPath => Path.Combine("Assets", SAVE_FOLDER_NAME);

        public NodesGraphToolbar(NodesGraphView graph) : base()
        {
            this.graph = graph;

            //for now we use a VisualElement with inside a Toolbar, to not lose graphic styles. In the future I have to edit styles for Toolbar too
            toolbar = new Toolbar();
            Add(toolbar);

            //add label
            fileName = DEFAULT_FILE_NAME;
            fileNameTextfield = CreateElementsUtilities.CreatetextField(fileName, "File Name:", callback => fileName = callback.newValue);

            //add buttons
            Button saveButton = CreateElementsUtilities.CreateButton("Save", () => ClickSave());
            Button loadButton = CreateElementsUtilities.CreateButton("Load", () => ClickLoad());
            Button clearButton = CreateElementsUtilities.CreateButton("Clear", () => ClickClear());
            minimapButton = CreateElementsUtilities.CreateButton("Minimap", () => ClickToggleMinimap());

            //check if minimapButton is toggled or not (minimap visible or not)
            if (this.graph.IsMinimapVisible())
                StyleSheetUtilities.ToggleInClassesList(minimapButton, "ds-toolbar__button__selected");

            //add elements to Toolbar
            AddInToolbar(fileNameTextfield);
            AddInToolbar(saveButton);
            AddInToolbar(loadButton);
            AddInToolbar(clearButton);
            AddInToolbar(minimapButton);
        }
        private void AddInToolbar(VisualElement element)
        {
            toolbar.Add(element);
        }

        #region on click buttons

        /// <summary>
        /// Save all assets
        /// </summary>
        void ClickSave()
        {
            //be sure there is a file name
            if (string.IsNullOrEmpty(fileName))
            {
                EditorUtility.DisplayDialog(
                    "Invalid file name",
                    "Please ensure the file name you have typed in is valid.",
                    "OK");

                return;
            }

            //save file
            string filePathInProject = EditorUtility.SaveFilePanelInProject("Save file", fileName, "asset", "Select folder where save file");
            if (string.IsNullOrEmpty(filePathInProject) == false)
                SaveUtilities.Save(graph, filePathInProject);
        }

        /// <summary>
        /// Load from asset
        /// </summary>
        void ClickLoad()
        {
            //find file in project
            string filePath = EditorUtility.OpenFilePanel("Load Graph", SavedFolderPath, "asset");

            if (string.IsNullOrEmpty(filePath) == false)
            {
                //clear graph view
                graph.ClearGraph();

                //get file name
                fileName = Path.GetFileNameWithoutExtension(filePath);

                //update label
                fileNameTextfield.SetValueWithoutNotify(fileName);

                //remove path until "Assets", because we need path relative to project folder
                string filePathInProject = filePath.Substring(filePath.LastIndexOf("Assets"));
                SaveUtilities.Load(graph, filePathInProject);
            }
        }

        /// <summary>
        /// Clear graph view
        /// </summary>
        void ClickClear()
        {
            graph.ClearGraph();
        }

        /// <summary>
        /// Toggle minimap
        /// </summary>
        void ClickToggleMinimap()
        {
            graph.ToggleMinimap();

            //toggle minimap button styles
            StyleSheetUtilities.ToggleInClassesList(minimapButton, "ds-toolbar__button__selected");
        }

        #endregion
    }
}
#endif