#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// Toolbar on top of the Graph, with various buttons and ui elements
    /// </summary>
    public class NodesGraphToolbar : VisualElement
    {
        //automatic save
        protected AutomaticSaveTask automaticSaveTask;
        protected virtual bool automaticSaveEnabled => true;
        protected virtual int automaticSaveTimer => 300;

        //init
        protected NodesGraphView graph;
        protected SaveLoadGraph saveLoad;
        protected Toolbar toolbar;

        //ui elements
        protected TextField fileNameTextfield;
        protected Button minimapButton;

        //file
        protected string fileName;
        protected string filePathInProject;

        public const string DEFAULT_FILE_NAME = "New File";
        public const string SAVE_FOLDER_NAME = "SavedGraphs";
        public static string SavedFolderPath => Path.Combine("Assets", SAVE_FOLDER_NAME);

        public NodesGraphToolbar(NodesGraphView graph, SaveLoadGraph saveLoad) : base()
        {
            this.graph = graph;
            this.saveLoad = saveLoad;

            //for now we use a VisualElement with inside a Toolbar, to not lose graphic styles. In the future I have to edit styles for Toolbar too
            toolbar = new Toolbar();
            Add(toolbar);

            //add label
            fileName = DEFAULT_FILE_NAME;
            fileNameTextfield = CreateElementsUtilities.CreateTextField("File Name:", fileName, callback => fileName = callback.newValue);

            //add buttons
            Button saveButton = CreateElementsUtilities.CreateButton("Save", ClickSave);
            Button loadButton = CreateElementsUtilities.CreateButton("Load", ClickLoad);
            Button clearButton = CreateElementsUtilities.CreateButton("Clear", ClickClear);
            minimapButton = CreateElementsUtilities.CreateButton("Minimap", ClickToggleMinimap);

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

        protected virtual void AddInToolbar(VisualElement element)
        {
            toolbar.Add(element);
        }

        #region Save API

        protected virtual void Save()
        {
            //if fileName and filePath are setted
            if (string.IsNullOrEmpty(fileName) == false && string.IsNullOrEmpty(filePathInProject) == false)
            {
                //save
                saveLoad.Save(graph, filePathInProject);

                //and start save automatically
                if (automaticSaveEnabled)
                {
                    if (automaticSaveTask != null)
                        automaticSaveTask.Stop();
                    automaticSaveTask = new AutomaticSaveTask(automaticSaveTimer, fileName, filePathInProject, AutomaticSave);
                }
            }
        }

        protected virtual void AutomaticSave(string prevFileName, string prevFilePathInProject)
        {
            //check if file name and path are still the same
            if (fileName == prevFileName && filePathInProject == prevFilePathInProject)
            {
                UnityEngine.Debug.Log("AUTOMATIC SAVE: " + System.DateTime.Now.ToString());

                //and save
                Save();
            }
        }

        #endregion

        #region on click buttons

        /// <summary>
        /// Save all assets
        /// </summary>
        protected virtual void ClickSave()
        {
            //be sure there is a file name
            if (string.IsNullOrEmpty(fileName))
            {
                EditorUtility.DisplayDialog(
                    "Invalid file name",
                    "Please ensure the file name you have typed is valid.",
                    "OK");

                return;
            }

            //save file
            filePathInProject = EditorUtility.SaveFilePanelInProject("Save file", fileName, "asset", "Select folder where save file");
            Save();
        }

        /// <summary>
        /// Load from asset
        /// </summary>
        protected virtual void ClickLoad()
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
                filePathInProject = filePath.Substring(filePath.LastIndexOf("Assets"));
                saveLoad.Load(graph, filePathInProject);

                //and start save automatically
                if (automaticSaveEnabled)
                {
                    if (automaticSaveTask != null)
                        automaticSaveTask.Stop();
                    automaticSaveTask = new AutomaticSaveTask(automaticSaveTimer, fileName, filePathInProject, AutomaticSave);
                }
            }
        }

        /// <summary>
        /// Clear graph view
        /// </summary>
        protected virtual void ClickClear()
        {
            graph.ClearGraph();
        }

        /// <summary>
        /// Toggle minimap
        /// </summary>
        protected virtual void ClickToggleMinimap()
        {
            graph.ToggleMinimap();

            //toggle minimap button styles
            StyleSheetUtilities.ToggleInClassesList(minimapButton, "ds-toolbar__button__selected");
        }

        #endregion
    }
}
#endif