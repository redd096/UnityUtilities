#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// The GraphView we use to create nodes and link them
    /// </summary>
    public class NodesGraphView : GraphView
    {
        public GraphNode StartingNode;

        protected EditorWindow editorWindow;
        protected MiniMap minimap;

        public NodesGraphView(EditorWindow editorWindow) : base()
        {
            this.editorWindow = editorWindow;

            //override callbacks
            OnDeleteSelection();
            OnElementsAddedToGroup();
            OnElementsRemovedFromGroup();
            //OnGraphViewChanged();

            //add manipulators (to drag, zoom, select more nodes with a rectangle, etc...)
            AddManipulators();
            //AddContextualMenuManipulators();

            //add components
            AddMinimap();
            AddGridBackground();
            //AddSearchWindow();
        }

        #region override graph view

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //return base.GetCompatiblePorts(startPort, nodeAdapter);

            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                //not on same node
                if (startPort.node == port.node)
                    return;

                //not input with input, or output with output
                if (startPort.direction == port.direction)
                    return;

                Port inputPort = startPort.direction == Direction.Input ? startPort : port;
                Port outputPort = startPort.direction == Direction.Output ? startPort : port;

                //if output is same type or inherit from input, then can connect
                if (inputPort.portType.IsAssignableFrom(outputPort.portType))
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        //public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        //{
        //    //base.BuildContextualMenu(evt);

        //    //is possible to add elements in contextual menu, by overriding BuildContextualMenu or with manipulators
        //    //you can find an example with override in BuildContextualMenu()
        //    //and an example with manipulators in AddContextualMenuManipulators()

        //    //AddContextualMenuButtons(evt);
        //}

        protected virtual void AddContextualMenuButtons(ContextualMenuPopulateEvent evt)
        {
            //only when click on the graph (not on one node, group or other things)
            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Add Node (DynamicNode)", (actionEvent) =>                        //we take the menu already showed (menuEvent.menu) and add an action "Add Node"
                CreateNode("NodeName", typeof(Example.ExampleDynamicNode),                              //and Create Node will create a node to the mouse position,
                    GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)));                  //using this function to find the correct position even when the graph is zoomed
            }
            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Add Node (GraphNode)", (actionEvent) =>                          //we take the menu already showed (menuEvent.menu) and add an action "Add Node"
                CreateNode("NodeName", typeof(GraphNode),                                               //and Create Node will create a node to the mouse position,
                    GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)));                  //using this function to find the correct position even when the graph is zoomed
            }
            //only when click on the graph or node to add in the group (not on group or other things)
            if (evt.target is GraphView || evt.target is Node)
            {
                evt.menu.AppendAction("Add Group", (actionEvent) =>                                     //Add Group action
                CreateGroup("Group", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))); //call CreateGroup event
            }
        }

        #endregion

        #region override callbacks

        /// <summary>
        /// Override deleteSelection, the callback when delete an element from graph view.
        /// </summary>
        protected virtual void OnDeleteSelection()
        {
            //operationName is used by undo/redo functions, and askUser is to know if show popup to user before delete
            deleteSelection += (operationName, askUser) =>
            {
                foreach (GraphElement element in selection)
                {
                    //we have to override this: when delete a group, remove all its nodes to not delete nodes that aren't selected
                    if (element is Group group)
                    {
                        var containedElements = new List<GraphElement>(group.containedElements);
                        foreach (GraphElement containedElement in containedElements)
                        {
                            if (containedElement is Node)
                            {
                                group.RemoveElement(containedElement);
                            }
                        }

                        continue;
                    }
                }

                //in the end, call normal delete selection
                DeleteSelection();
            };
        }

        /// <summary>
        /// Override elementsAddedToGroup, the callback when something is added to a group
        /// </summary>
        protected virtual void OnElementsAddedToGroup()
        {
            elementsAddedToGroup += (group, elements) =>
            {
                //save group in node
                foreach (GraphElement element in elements)
                {
                    if (element is GraphNode node)
                        node.Group = group as GraphGroup;
                }
            };
        }

        /// <summary>
        /// Override elementsRemovedFromGroup, the callback when something is removed from a group
        /// </summary>
        protected virtual void OnElementsRemovedFromGroup()
        {
            elementsRemovedFromGroup += (group, elements) =>
            {
                //clear group in node
                foreach (GraphElement element in elements)
                {
                    if (element is GraphNode node)
                        node.Group = null;
                }
            };
        }

        ///// <summary>
        ///// Override graphViewChanged, the callback when something is changed in the graph view
        ///// </summary>
        //protected virtual void OnGraphViewChanged()
        //{
        //    graphViewChanged += (changes) =>
        //    {
        //        //when create connections between nodes
        //        if (changes.edgesToCreate != null)
        //        {
        //            foreach (Edge edge in changes.edgesToCreate)
        //            {
        //                //save on outputPort, the node it reach
        //                GraphNode connectedNode = edge.input.node as GraphNode;
        //                NodeOutputData data = edge.output.userData as NodeOutputData;
        //                if (data != null) data.ConnectedNodeID = connectedNode != null ? connectedNode.ID : "";
        //            }
        //        }

        //        //when delete elements
        //        if (changes.elementsToRemove != null)
        //        {
        //            foreach (GraphElement element in changes.elementsToRemove)
        //            {
        //                //if delete connections between nodes
        //                if (element is Edge edge)
        //                {
        //                    //remove node from port
        //                    NodeOutputData data = edge.output.userData as NodeOutputData;
        //                    if (data != null) data.ConnectedNodeID = "";
        //                }
        //            }
        //        }

        //        return changes;
        //    };
        //}

        #endregion

        #region add manipulators

        protected virtual void AddManipulators()
        {
            //add drag manipulator when click middle mouse button, and zoom when move the wheel
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            //another method to add content zoomer
            //SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            //drag selected nodes, and rectangle to select more nodes at time
            //for some reason, SelectionDragger must be added before RectangleSelector, otherwise the nodes selected with the rectangle can't be dragged
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        protected virtual void AddContextualMenuManipulators()
        {
            //is possible to add elements in contextual menu, by overriding BuildContextualMenu or with manipulators
            //you can find an example with override in BuildContextualMenu()
            //and an example with manipulators in AddContextualMenuManipulators()

            this.AddManipulator(CreateNodeContextualMenu("Add Node (DynamicNode)", typeof(Example.ExampleDynamicNode)));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (GraphNode)", typeof(GraphNode)));
            this.AddManipulator(CreateGroupContextualMenu());
        }

        protected virtual IManipulator CreateNodeContextualMenu(string actionTitle, System.Type nodeType)
        {
            //update the contextual menu to show when right click on the graph
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator((evt) =>    //evt is the event to trigger when the menu is showed
            {
                //only when click on the graph (not on one node, group or other things)
                if (evt.target is GraphView)
                {
                    evt.menu.AppendAction(actionTitle, (actionEvent) =>                                     //we take the menu already showed (menuEvent.menu) and add an action "Add Node"
                    CreateNode("NodeName", nodeType,                                                        //and Create Node will create a node to the mouse position,
                        GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)));                  //using this function to find the correct position even when the graph is zoomed
                }
            });

            return contextualMenuManipulator;
        }

        protected virtual IManipulator CreateGroupContextualMenu()
        {
            //update the contextual menu to show when right click on the graph
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator((evt) =>
            {
                //only when click on the graph or node to add in the group (not on group or other things)
                if (evt.target is GraphView || evt.target is Node)
                {
                    evt.menu.AppendAction("Add Group", (actionEvent) =>                                     //Add Group action
                    CreateGroup("Group", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))); //call CreateGroup event
                }
            });

            return contextualMenuManipulator;
        }

        #endregion

        #region add components

        protected virtual void AddMinimap()
        {
            //create minimap and set position
            minimap = new MiniMap()
            {
                //anchored or can user drag it? - with right click on the minimap is possible to toggle this option
                anchored = true,
            };
            minimap.SetPosition(new Rect(15, 50, 200, 180));

            //add instead of AddElement, because AddElement is used when create something on the Graph like nodes, that could also increase its size
            Add(minimap);

            //by default is not visible. We'll add a button in toolbar to show or hide minimap
            minimap.visible = false;
        }

        protected virtual void AddGridBackground()
        {
            //create a GridBackground and set same size as the graph view
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();

            //add to graphView, with index 0 to be sure is behind everything
            Insert(0, gridBackground);
        }

        protected virtual void AddSearchWindow()
        {
            //create Search Window scriptable object instance
            Example.ExampleSearchWindow searchWindow = ScriptableObject.CreateInstance<Example.ExampleSearchWindow>();
            searchWindow.Initialize(this);

            //node creation request is called when press Space
            //when press Space, call Unity SearchWindow.Open passing the mouse position to know where open it, and our scriptable object to know what visualize
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
            //we don't need to call GetLocalMousePosition, because we don't need to instantiate on the graph, but just to show a window on the screen

            //NB probably you want to override BuildContextualMenu, because unity add a "Create Node" option when nodeCreationRequest isn't null
        }

        #endregion

        #region public API

        /// <summary>
        /// Create a node inside this GraphView
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <param name="shouldDraw">Call node.DrawNode()? By default is true, but you could set it false for example when load a node to set values before call Draw</param>
        /// <returns></returns>
        public virtual GraphNode CreateNode(string nodeName, System.Type type, Vector2 position, bool shouldDraw = true)
        {
            //create node and use AddElement instead of Add, to add to the GraphView
            GraphNode node = System.Activator.CreateInstance(type) as GraphNode;
            node.Initialize(nodeName, this, position);

            //draw
            if (shouldDraw)
                node.DrawNode();

            //add to graph view
            AddElement(node);

            //if first node in the graph, set as Starting Node
            if (StartingNode == null)
                SetStartingNode(node);

            return node;
        }

        /// <summary>
        /// Create a group inside this Grap
        /// </summary>
        /// <param name="title"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual GraphGroup CreateGroup(string title, Vector2 position)
        {
            //add group to drag more nodes together.
            //Shift + drag to remove node from the group
            GraphGroup group = new GraphGroup(title, position);

            //add to graph view
            AddElement(group);

            //if selecting nodes when create group, add them to it
            //this automatically call elementsAddedToGroup callback
            foreach (GraphElement element in selection)
            {
                if (element is Node node)
                    group.AddElement(node);
            }

            //if add a node already inside a menu, by default the node is removed from previous group and added to new one

            return group;
        }

        /// <summary>
        /// Mouse position: from screen position to graph local position. 
        /// So it works also if we zoom or move the graph view
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="isScreenPosition">True is screenPosition. False is local in editor position</param>
        /// <returns></returns>
        public virtual Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isScreenPosition = false)
        {
            //if screen position instead of editor position, remove the window editor position
            if (isScreenPosition)
                mousePosition -= editorWindow.position.position;

            return contentViewContainer.WorldToLocal(mousePosition);
        }

        /// <summary>
        /// Set starting node for this graph. Update also graphic
        /// </summary>
        /// <param name="node"></param>
        public virtual void SetStartingNode(GraphNode node)
        {
            if (node == null || StartingNode == node)
                return;

            //remove color from previous starting node
            if (StartingNode != null)
                StyleSheetUtilities.RemoveFromClassesList(StartingNode.mainContainer, "ds-node__main-container__starting-button");

            //and add to new start node
            StartingNode = node;
            StyleSheetUtilities.AddToClassesList(StartingNode.mainContainer, "ds-node__main-container__starting-button");
        }

        /// <summary>
        /// Remove every node, group, etc.. in the graph
        /// </summary>
        public virtual void ClearGraph()
        {
            DeleteElements(graphElements);
            //graphElements.ForEach(graphElement => RemoveElement(graphElement));

            StartingNode = null;
        }

        /// <summary>
        /// Show or hide minimap
        /// </summary>
        public virtual void ToggleMinimap()
        {
            minimap.visible = !minimap.visible;
        }

        /// <summary>
        /// Return true if the minimap is instantiated and visible, else return false
        /// </summary>
        /// <returns></returns>
        public virtual bool IsMinimapVisible()
        {
            return minimap != null && minimap.visible;
        }

        #endregion
    }
}
#endif