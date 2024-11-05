using System.Collections.Generic;
using UnityEngine;

namespace redd096.NodesGraph.Runtime
{
    /// <summary>
    /// This is the file to save. You can load it to recreate the same Graph
    /// </summary>
    public class FileData : ScriptableObject
    {
        public List<NodeData> Nodes;
        public List<GroupData> Groups;

        public void Initialize(List<NodeData> nodes, List<GroupData> groups)
        {
            Nodes = nodes;
            Groups = groups;
        }
    }
}