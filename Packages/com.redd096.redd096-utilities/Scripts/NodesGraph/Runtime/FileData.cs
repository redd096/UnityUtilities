using System.Collections.Generic;
using UnityEngine;

namespace redd096.NodesGraph.Runtime
{
    /// <summary>
    /// This is the file to save. You can load it to recreate the same Graph
    /// </summary>
    public class FileData : ScriptableObject
    {
        public string FileName;
        [Space]
        public List<NodeData> Nodes;
        public List<GroupData> Groups;

        public void Initialize(string fileName, List<NodeData> nodes, List<GroupData> groups)
        {
            FileName = fileName;
            Nodes = nodes;
            Groups = groups;
        }
    }
}