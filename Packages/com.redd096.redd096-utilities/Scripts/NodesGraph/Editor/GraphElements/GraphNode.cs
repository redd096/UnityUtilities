#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// Basic node for the Graph
    /// </summary>
    public abstract class GraphNode : Node
    {
        public string NodeName;
        public string ID;
        public GraphGroup Group;

        protected NodesGraphView graph;

        //graphic
        protected TextField nameTextField;

        /// <summary>
        /// Initialize node when created
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="position"></param>
        public virtual void Initialize(string nodeName, NodesGraphView graph, Vector2 position)
        {
            this.graph = graph;

            //set default values
            NodeName = nodeName;
            ID = System.Guid.NewGuid().ToString();

            //set position
            SetPosition(new Rect(position, Vector2.zero));
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //when right click on a node, add these buttons
            if (evt.target is Node)
            {
                evt.menu.AppendAction("Set as Starting Node", dropdownAction => graph.SetStartingNode(this));
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Disconnect inputs", dropdownAction => DisconnectPorts(inputContainer), status: IsOnePortConnected(inputContainer) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);      //enable only if at least one port is connected
                evt.menu.AppendAction("Disconnect outputs", dropdownAction => DisconnectPorts(outputContainer), status: IsOnePortConnected(outputContainer) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);   //enable only if at least one port is connected
                //disconnect all is already showed by base class
            }

            base.BuildContextualMenu(evt);
        }

        /// <summary>
        /// Call every Draw function of the node
        /// </summary>
        public virtual void DrawNode()
        {
            DrawBaseNode();
            DrawInputPorts();
            DrawOutputPorts();
            DrawContent();
            FinishDrawContent();
        }

        #region draw API

        /// <summary>
        /// Draw base node: a Textfield for the name
        /// </summary>
        protected virtual void DrawBaseNode()
        {
            //MainContainer contains everything
            //TitleContainer and TitleButtonContainer will be on top, also when collapsed                                               (index 0 inside main container)
            //on the top, under the title container: InputContainer on the left, TopContainer at center, OutputContainer on the right   (index 1 inside main container)
            //the body of the node is in ExtensionContainer (it wants RefreshExpandedState() to show everything you added inside)       (index 2 inside main container)
            
            //set name textfield, and add to title container
            nameTextField = CreateElementsUtilities.CreateTextField(null, NodeName, callback => NodeName = callback.newValue);
            titleContainer.Insert(0, nameTextField);
        }

        /// <summary>
        /// Draw input ports
        /// </summary>
        protected abstract void DrawInputPorts();

        /// <summary>
        /// Draw output ports
        /// </summary>
        protected abstract void DrawOutputPorts();

        /// <summary>
        /// Draw the node content (e.g. text for a DebugLog Node, or object variable for AudioClip node)
        /// </summary>
        protected abstract void DrawContent();

        /// <summary>
        /// Complete draw (call RefreshExpandedState of the GraphView Node, add style to every component, etc...)
        /// </summary>
        protected virtual void FinishDrawContent()
        {
            //this is necessary if add something inside ExtensionContainer
            RefreshExpandedState();

            //add styles
            AddStyles();
        }

        #endregion

        #region ports and styles

        protected virtual bool IsOnePortConnected(VisualElement container)
        {
            //check if at least one port is connected
            List<Port> list = container.Query<Port>().ToList();
            foreach (Port port in list)
            {
                if (port.connected)
                    return true;
            }

            return false;

            //foreach (VisualElement element in container.Children())
            //{
            //    if (element is Port port)
            //    {
            //        if (port.connected)
            //            return true;
            //    }
            //}
            //return false;
        }

        protected virtual void DisconnectPorts(VisualElement container)
        {
            //foreach port, remove every connection
            List<GraphElement> toDeleteList = new List<GraphElement>();
            container.Query<Port>().ForEach(port =>
            {
                //if not connected, ignore it
                if (port.connected)
                {
                    //I don't know what is this capabilities check, I copied it from Node.AddConnectionsToDeleteSet() 
                    foreach (Edge connection in port.connections)
                    {
                        if ((connection.capabilities & Capabilities.Deletable) != 0)
                            toDeleteList.Add(connection);
                    }
                }
            });
            graph.DeleteElements(toDeleteList);

            //foreach (VisualElement element in container.Children())
            //{
            //    if (element is Port port)
            //    {
            //        if (port.connected)
            //            graph.DeleteElements(port.connections);
            //    }
            //}
        }

        protected virtual void AddStyles()
        {
            //add styles to containers
            StyleSheetUtilities.AddToClassesList(mainContainer, "ds-node__main-container");
            StyleSheetUtilities.AddToClassesList(extensionContainer, "ds-node__extension-container");

            //add styles to elements
            StyleSheetUtilities.AddToClassesList(nameTextField, "ds-node__textfield", "ds-node__filename-textfield", "ds-node__textfield__hidden");
        }

        #endregion
    }
}
#endif