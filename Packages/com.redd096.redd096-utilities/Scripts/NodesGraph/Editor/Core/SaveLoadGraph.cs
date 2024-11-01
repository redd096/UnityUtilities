#if UNITY_EDITOR
namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// This is the base class used for save and load the graph. Inherit from it and manage the files as you want
    /// </summary>
    public class SaveLoadGraph
    {
        /// <summary>
        /// Save a scriptable object with a reference to every node and group in the graph
        /// </summary>
        /// <param name="assetPathRelativeToProject">The path to the file, but the path must starts with Assets</param>
        public virtual void Save(NodesGraphView graph, string assetPathRelativeToProject)
        {
            SaveUtilities.Save(graph, assetPathRelativeToProject);
        }

        /// <summary>
        /// Load file data and recreate a graph with it
        /// </summary>
        /// <param name="assetPathRelativeToProject">The path to the file, but the path must starts with Assets</param>
        public virtual void Load(NodesGraphView graph, string assetPathRelativeToProject)
        {
            SaveUtilities.Load(graph, assetPathRelativeToProject);
        }
    }
}
#endif