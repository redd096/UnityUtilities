#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// Group inside the Graph, where user can put nodes
    /// </summary>
    public class GraphGroup : Group
    {
        public string ID;

        public GraphGroup(string title, Vector2 position) : base()
        {
            //set default values
            this.title = title;
            ID = System.Guid.NewGuid().ToString();

            //set position
            SetPosition(new Rect(position, Vector2.zero));
        }
    }
}
#endif