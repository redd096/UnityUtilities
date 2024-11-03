#if UNITY_EDITOR
using redd096.NodesGraph.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// This is the base class used for save and load the graph. Inherit from it and manage the files as you want
    /// </summary>
    public class SaveLoadGraph
    {
        protected NodesGraphView graph;
        protected string assetPathRelativeToProject;
        protected string directoryPath;
        protected string directoryPathRelativeToProject;
        protected string fileName;

        protected List<NodeData> nodes;
        protected List<GroupData> groups;

        //used on load
        protected Dictionary<string, GraphNode> loadedNodes;
        protected Dictionary<string, GraphGroup> loadedGroups;

        /// <summary>
        /// Save a scriptable object with a reference to every node and group in the graph
        /// </summary>
        /// <param name="assetPathRelativeToProject">The path to the file, but the path must starts with Assets</param>
        public virtual void Save(NodesGraphView graph, string assetPathRelativeToProject)
        {
            Initialize(graph, assetPathRelativeToProject);

            //get elements in graph and save file
            GetElementsFromGraphView();
            SaveAsset();
        }

        /// <summary>
        /// Load file data and recreate a graph with it
        /// </summary>
        /// <param name="assetPathRelativeToProject">The path to the file, but the path must starts with Assets</param>
        public virtual void Load(NodesGraphView graph, string assetPathRelativeToProject)
        {
            Initialize(graph, assetPathRelativeToProject);

            //load file and set elements in graph
            if (TryLoadFile())
            {
                SetElementsInGraphView();
            }
        }

        protected virtual void Initialize(NodesGraphView graph, string assetPathRelativeToProject)
        {
            //save refs
            this.graph = graph;
            this.assetPathRelativeToProject = assetPathRelativeToProject;
            fileName = Path.GetFileNameWithoutExtension(assetPathRelativeToProject);

            string pathToProject = Application.dataPath.Replace("Assets", string.Empty);    //remove "Assets" because it should already be in assetPath
            string projectDirectories = Path.GetDirectoryName(assetPathRelativeToProject);  //remove file from path and keep only directories

            //calculate full directory path, and directory path starting from Assets/
            directoryPath = Path.Combine(pathToProject, projectDirectories);
            directoryPathRelativeToProject = assetPathRelativeToProject.Replace(Path.GetFileName(assetPathRelativeToProject), "");

            //initialize vars
            nodes = new List<NodeData>();
            groups = new List<GroupData>();
            loadedNodes = new Dictionary<string, GraphNode>();
            loadedGroups = new Dictionary<string, GraphGroup>();
        }

        #region save API - get elements in graph

        protected virtual void GetElementsFromGraphView()
        {
            //add starting node as first node
            BeforeAddGraphElements();

            graph.graphElements.ForEach(graphElement =>
            {
                //add every other node
                if (AddNode(graphElement))
                    return;

                //add groups
                if (AddGroup(graphElement))
                    return;

                //add other elements
                if (AddOtherGraphElements(graphElement))
                    return;
            });
        }

        protected virtual void BeforeAddGraphElements()
        {
            //add starting node as first node
            if (graph.StartingNode != null)
                nodes.Add(CreateNodeData(graph.StartingNode));
        }

        protected virtual bool AddNode(GraphElement graphElement)
        {
            //add every node
            if (graphElement is GraphNode node)
            {
                //not Starting Node, because we already added it as first node
                if (node != graph.StartingNode)
                    nodes.Add(CreateNodeData(node));

                return true;
            }

            return false;
        }

        protected virtual bool AddGroup(GraphElement graphElement)
        {
            //add groups
            if (graphElement is GraphGroup group)
            {
                groups.Add(CreateGroupData(group));
                return true;
            }

            return false;
        }

        protected virtual bool AddOtherGraphElements(GraphElement graphElement)
        {
            return false;
        }

        protected virtual NodeData CreateNodeData(GraphNode node)
        {
            //create data from GraphNode
            NodeData data = new NodeData();
            SetNodeDataValues(node, data);
            return data;
        }

        protected virtual void SetNodeDataValues(GraphNode node, NodeData data)
        {
            //set default data values
            data.NodeName = node.NodeName;
            data.ID = node.ID;
            data.NodeType = node.GetType().FullName;
            data.OutputsData = node.outputContainer.Query<Port>().ToList().Select(port => CreateOutputData(port)).ToList();   //foreach output port, create output data
            data.GroupID = node.Group != null ? node.Group.ID : "";
            data.Position = node.GetPosition().position;
        }

        protected virtual NodeOutputData CreateOutputData(Port port)
        {
            //create data from Port
            NodeOutputData data = new NodeOutputData();
            SetOutputDataValues(port, data);
            return data;
        }

        protected virtual void SetOutputDataValues(Port port, NodeOutputData data)
        {
            //save type
            data.OutputType = port.portType.FullName;

            //and connected node
            if (port.connected)
            {
                foreach (var edge in port.connections)
                {
                    if (edge.input != null && edge.input.node is GraphNode connectedNode)
                    {
                        data.ConnectedNodeID = connectedNode.ID;

                        //find input door connected with this output door and save its index
                        data.ConnectedPortIndex = edge.input.node.inputContainer.Query<Port>().ToList().FindIndex(inputPort => inputPort.connections.Any(inputEdge => inputEdge.output != null && inputEdge.output == port));
                        break;
                    }
                }
            }
        }

        protected virtual GroupData CreateGroupData(GraphGroup group)
        {
            //create data from GraphGroup
            GroupData data = new GroupData();
            SetGroupDataValues(group, data);
            return data;
        }

        protected virtual void SetGroupDataValues(GraphGroup group, GroupData data)
        {
            data.Title = group.title;
            data.ID = group.ID;
            data.Position = group.GetPosition().position;
            data.ContainedNodesID = group.containedElements.Where(x => x is GraphNode).Select(x => (x as GraphNode).ID).ToList();    //for every contained node, save ID
        }

        #endregion

        #region save API - save file

        protected virtual void SaveAsset()
        {
            //create folder if not exists
            CreateSaveFolder();

            //save file
            SetValuesAndSaveFile();
            RefreshEditor();
        }

        protected virtual void CreateSaveFolder()
        {
            //create folder if not exists
            if (Directory.Exists(directoryPath) == false)
                Directory.CreateDirectory(directoryPath);
        }

        protected virtual void SetValuesAndSaveFile()
        {
            //try load file
            FileData asset = AssetDatabase.LoadAssetAtPath<FileData>(assetPathRelativeToProject);
            if (asset == null)
            {
                //else create it
                asset = ScriptableObject.CreateInstance<FileData>();
                AssetDatabase.CreateAsset(asset, assetPathRelativeToProject);
            }

            //set values
            asset.Initialize(fileName, nodes, groups);

            //set dirty in editor
            EditorUtility.SetDirty(asset);
        }

        protected virtual void RefreshEditor()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion

        #region load API - load file

        protected virtual bool TryLoadFile()
        {
            //try load
            FileData asset = AssetDatabase.LoadAssetAtPath<FileData>(assetPathRelativeToProject);

            //error if not found
            if (asset == null)
            {
                EditorUtility.DisplayDialog(
                    "Couldn't load the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"{assetPathRelativeToProject}\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above",
                    "OK!");

                return false;
            }

            //get elements from asset
            nodes = asset.Nodes;
            groups = asset.Groups;

            return true;
        }

        #endregion

        #region load API - set elements in graph

        protected virtual void SetElementsInGraphView()
        {
            LoadGroups();
            LoadNodes();
            LoadNodesConnections();
        }

        protected virtual void LoadGroups()
        {
            if (groups == null)
                return;

            //instantiate every group and set values
            foreach (GroupData data in groups)
            {
                GraphGroup group = graph.CreateGroup(data.Title, data.Position);
                SetGroupValues(group, data);

                //add to dictionary
                loadedGroups.Add(group.ID, group);
            }
        }

        protected virtual void SetGroupValues(GraphGroup group, GroupData data)
        {
            group.ID = data.ID;
        }

        protected virtual void LoadNodes()
        {
            if (nodes == null)
                return;

            //instantiate every node and set values
            foreach (NodeData data in nodes)
            {
                System.Type nodeType = System.Type.GetType(data.NodeType);
                GraphNode node = graph.CreateNode(data.NodeName, nodeType, data.Position, false);
                SetNodeValues(node, data);
                node.DrawNode();
                SetNodeGroup(node, data);

                //add to dictionary
                loadedNodes.Add(node.ID, node);
            }
        }

        protected virtual void SetNodeValues(GraphNode node, NodeData data)
        {
            node.ID = data.ID;
        }

        protected virtual void SetNodeGroup(GraphNode node, NodeData data)
        {
            //if the node is inside a group, find the group in the dictionary and add to it
            if (string.IsNullOrEmpty(data.GroupID) == false)
            {
                if (loadedGroups.ContainsKey(data.GroupID))
                {
                    GraphGroup group = loadedGroups[data.GroupID];
                    node.Group = group;

                    group.AddElement(node);
                }
                else
                {
                    Debug.LogError($"Error Node with id: {data.ID}. Should be inside a group, but it's impossible to find group with id: {data.GroupID}");
                }
            }
        }

        protected virtual void LoadNodesConnections()
        {
            if (nodes == null || loadedNodes == null)
                return;

            //foreach node
            foreach (NodeData data in nodes)
            {
                //check if we created this node
                if (loadedNodes.ContainsKey(data.ID) == false)
                {
                    Debug.LogError($"Impossible to find node with ID: {data.ID}");
                    continue;
                }

                //check if there are output ports
                GraphNode node = loadedNodes[data.ID];
                List<Port> list = node.outputContainer.Query<Port>().ToList();
                if (list.Count != data.OutputsData.Count)
                {
                    Debug.LogError($"Error: Node has {list.Count} output doors, but in data are saved {data.OutputsData.Count} outputs");
                    continue;
                }

                //foreach output
                for (int i = 0; i < data.OutputsData.Count; i++)
                {
                    Port outputPort = list[i];
                    //outputPort.portType = System.Type.GetType(data.OutputsData[i].OutputType);
                    string connectedNodeID = data.OutputsData[i].ConnectedNodeID;
                    int connectedPortIndex = data.OutputsData[i].ConnectedPortIndex;

                    //connect to other node
                    if (string.IsNullOrEmpty(connectedNodeID) == false)
                    {
                        //check if we created a node with this ID
                        if (loadedNodes.ContainsKey(connectedNodeID) == false)
                        {
                            Debug.LogError($"Error: Node with ID {data.ID} should be connected to a node with ID {connectedNodeID}, but there isn't a node with ID {connectedNodeID}");
                            continue;
                        }

                        SetNodeConnection(node, outputPort, connectedNodeID, connectedPortIndex);
                    }
                }
            }
        }

        protected virtual void SetNodeConnection(GraphNode node, Port outputPort, string connectedNodeID, int connectedPortIndex)
        {
            GraphNode connectedNode = loadedNodes[connectedNodeID];
            List<Port> inputPorts = connectedNode.inputContainer.Query<Port>().ToList();

            //be sure port index is correct and the port is correct type
            Port connectedPort = inputPorts.Count > connectedPortIndex 
                && inputPorts[connectedPortIndex].portType.IsAssignableFrom(outputPort.portType) ? inputPorts[connectedPortIndex] : null;
            
            //else, try find first input port still not connected to other nodes and with correct Port Type
            if (connectedPort == null)
            {
                Debug.LogError($"Error connecting node with ID {node.ID} with node with ID {connectedNodeID}. " +
                    $"The second node doesn't have a correct input port at index {connectedPortIndex}. " +
                    $"Trying to connect to another door");
                connectedPort = inputPorts.Find(port => port.connected == false && port.portType.IsAssignableFrom(outputPort.portType));

                if (connectedPort == null)
                {
                    Debug.LogError($"Error connecting node with ID {node.ID} with node with ID {connectedNodeID}. Impossible to find a correct input door");
                    return;
                }
            }

            //connect ports
            Edge edge = outputPort.ConnectTo(connectedPort);
            graph.AddElement(edge);
            node.RefreshPorts();
        }

        #endregion
    }
}
#endif