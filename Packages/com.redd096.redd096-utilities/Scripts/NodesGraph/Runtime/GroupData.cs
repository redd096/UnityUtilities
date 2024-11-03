using System.Collections.Generic;
using UnityEngine;

namespace redd096.NodesGraph.Runtime
{
    /// <summary>
    /// Data to save group
    /// </summary>
    [System.Serializable]
    public class GroupData
    {
        public string Title;
        public string ID;
        public Vector2 Position;
        public List<string> ContainedNodesID;

        /// <summary>
        /// Instead of create a GroupData for every new group, you can save values inside this
        /// </summary>
        public object UserData;
    }
}