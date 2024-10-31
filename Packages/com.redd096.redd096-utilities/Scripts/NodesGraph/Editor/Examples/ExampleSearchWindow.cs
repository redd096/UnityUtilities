#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace redd096.NodesGraph.Editor.Example
{
    public class ExampleSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private NodesGraphView graph;

        /// <summary>
        /// Initialize to set graph reference. Look in NodesGraphView.AddSearchWindow()
        /// </summary>
        /// <param name="graph"></param>
        public virtual void Initialize(NodesGraphView graph)
        {
            this.graph = graph;
        }

        public virtual List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            //create the menu to show when press Space
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>()
            {
                //title of the window menu at level 0: Create
                new SearchTreeGroupEntry(new GUIContent("Create"), level: 0),

                //inside the menu there are two groups at level 1: Create Node and Create Group
                new SearchTreeGroupEntry(new GUIContent("Create Node"), level: 1),

                //inside CreateNode, there are two buttons at level 2. UserData is used by OnSelectEntry to understand which button was clicked
                //NB now is SearchTreeEntry and not SearchTreeGroupEntry
                new SearchTreeEntry(new GUIContent("DynamicNode"))
                {
                    level = 2,
                    userData = "DynamicNode"
                },
                new SearchTreeEntry(new GUIContent("GraphNode"))
                {
                    level = 2,
                    userData = "GraphNode"
                },

                new SearchTreeGroupEntry(new GUIContent("Create Group"), level: 1),

                //inside CreateGroup, there is only one button at level 2
                new SearchTreeEntry(new GUIContent("Group"))
                {
                    level = 2,
                    userData = "Group"
                }
            };

            return entries;
        }

        public virtual bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //from screen to graph position, to know where instantiate node or group inside it
            Vector2 localMousePosition = graph.GetLocalMousePosition(context.screenMousePosition, true);

            //find event to call
            switch (SearchTreeEntry.userData)
            {
                case "DynamicNode":
                    {
                        graph.CreateNode("NodeName", typeof(ExampleDynamicNode), localMousePosition);
                        return true;
                    }
                case "GraphNode":
                    {
                        graph.CreateNode("NodeName", typeof(GraphNode), localMousePosition);
                        return true;
                    }
                case "Group":
                    {
                        graph.CreateGroup("Group", localMousePosition);
                        return true;
                    }
            }

            //if click other buttons, return false to keep the menu open
            return false;
        }
    }
}
#endif