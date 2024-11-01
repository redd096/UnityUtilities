#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using redd096.NodesGraph.Runtime;
using UnityEditor;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// Example of utilities to save graph in a scriptable object and load from scriptable object to graph
    /// </summary>
    public static class SaveUtilities
    {
        private static NodesGraphView graph;

        private static List<NodeData> nodes;
        private static List<GroupData> groups;

        //used on load
        private static Dictionary<string, GraphGroup> loadedGroups;
        private static Dictionary<string, GraphNode> loadedNodes;

        /// <summary>
        /// Save a scriptable object with a reference to every node and group in the graph
        /// </summary>
        /// <param name="assetPathRelativeToProject">The path to the file, but the path must starts with Assets</param>
        public static void Save(NodesGraphView graph, string assetPathRelativeToProject)
        {
            SaveUtilities.graph = graph;

            //initialize lists
            nodes = new List<NodeData>();
            groups = new List<GroupData>();

            GetElementsFromGraphView();
            SaveAsset(assetPathRelativeToProject);
        }

        /// <summary>
        /// Load file data and recreate a graph with it
        /// </summary>
        /// <param name="assetPathRelativeToProject">The path to the file, but the path must starts with Assets</param>
        public static void Load(NodesGraphView graph, string assetPathRelativeToProject)
        {
            SaveUtilities.graph = graph;

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

                return;
            }

            //get elements from asset
            string fileName = asset.FileName;
            nodes = asset.Nodes;
            groups = asset.Groups;

            //initialize dictionaries
            loadedGroups = new Dictionary<string, GraphGroup>();
            loadedNodes = new Dictionary<string, GraphNode>();

            LoadGroups();
            LoadNodes();
            LoadNodesConnections();
        }

        private static void SaveAsset(string assetPathRelativeToProject)
        {
            //create folder if not exists
            CreateSaveFolder(assetPathRelativeToProject);

            //try load file
            FileData asset = AssetDatabase.LoadAssetAtPath<FileData>(assetPathRelativeToProject);
            if (asset == null)
            {
                //else create it
                asset = ScriptableObject.CreateInstance<FileData>();
                AssetDatabase.CreateAsset(asset, assetPathRelativeToProject);
            }

            //set values
            string fileName = Path.GetFileNameWithoutExtension(assetPathRelativeToProject);
            asset.Initialize(fileName, nodes, groups);

            //save asset
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region save API

        private static void CreateSaveFolder(string assetPathRelativeToProject)
        {
            string pathToProject = Application.dataPath.Replace("Assets", string.Empty);    //remove "Assets" because it should already be in assetPath
            string projectDirectories = Path.GetDirectoryName(assetPathRelativeToProject);  //remove file from path and keep only directories

            //create folder if not exists
            string path = Path.Combine(pathToProject, projectDirectories);
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }

        private static void GetElementsFromGraphView()
        {
            //add starting node as first node
            if (graph.StartingNode != null)
                nodes.Add(CreateNodeData(graph.StartingNode));

            graph.graphElements.ForEach(graphElement =>
            {
                //add every other node
                if (graphElement is GraphNode node)
                {
                    if (node != graph.StartingNode)
                        nodes.Add(CreateNodeData(node));

                    return;
                }

                //add groups
                if (graphElement is GraphGroup group)
                {
                    groups.Add(CreateGroupData(group));
                    return;
                }
            });
        }

        private static NodeData CreateNodeData(GraphNode node)
        {
            //create data from GraphNode
            NodeData data = new NodeData()
            {
                NodeName = node.NodeName,
                ID = node.ID,
                NodeType = node.GetType().FullName,
                OutputsData = CreateOutputData(node),
                GroupID = node.Group != null ? node.Group.ID : "",
                Position = node.GetPosition().position,
            };

            return data;
        }

        private static List<NodeOutputData> CreateOutputData(GraphNode node)
        {
            List<NodeOutputData> datas = new List<NodeOutputData>();

            //foreach output port
            List<Port> list = node.outputContainer.Query<Port>().ToList();
            foreach (Port port in list)
            {
                //save type
                NodeOutputData data = new NodeOutputData();
                data.OutputType = port.portType.Name;
                
                //and connected node
                if (port.connected)
                {
                    foreach (var edge in port.connections)
                    {
                        if (edge.input != null && edge.input.node is GraphNode connectedNode)
                        {
                            data.ConnectedNodeID = connectedNode.ID;
                            break;
                        }
                    }
                }

                //and add to list
                datas.Add(data);
            }

            return datas;
        }

        private static GroupData CreateGroupData(GraphGroup group)
        {
            //create data from GraphGroup
            GroupData data = new GroupData()
            {
                Title = group.title,
                ID = group.ID,
                Position = group.GetPosition().position,
                ContainedNodesID = group.containedElements.Where(x => x is GraphNode).Select(x => (x as GraphNode).ID).ToList(),    //for every contained node, save ID
            };

            return data;
        }

        #endregion

        #region load API

        private static void LoadGroups()
        {
            if (groups == null)
                return;

            //instantiate every group and set values
            foreach (GroupData data in groups)
            {
                GraphGroup group = graph.CreateGroup(data.Title, data.Position);
                group.ID = data.ID;

                //add to dictionary
                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes()
        {
            if (nodes == null)
                return;

            //instantiate every node and set values
            foreach (NodeData data in nodes)
            {
                System.Type nodeType = System.Type.GetType(data.NodeType);
                GraphNode node = graph.CreateNode(data.NodeName, nodeType, data.Position);
                node.ID = data.ID;

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
                        Debug.LogError($"Node with id: {data.ID}. Impossible to find group with id: {data.GroupID}");
                    }
                }

                //add to dictionary
                loadedNodes.Add(node.ID, node);
            }
        }

        private static void LoadNodesConnections()
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
                    string connectedNodeID = data.OutputsData[i].ConnectedNodeID;

                    //connect to other node
                    if (string.IsNullOrEmpty(connectedNodeID) == false)
                    {
                        //check if we created a node with this ID
                        if (loadedNodes.ContainsKey(connectedNodeID) == false)
                        {
                            Debug.LogError($"Error: Node with ID {data.ID} should be connected to a node with ID {connectedNodeID}, but there isn't a node with ID {connectedNodeID}");
                            continue;
                        }

                        //find first input port still not connected to other nodes and with correct Port Type
                        GraphNode connectedNode = loadedNodes[connectedNodeID];
                        List<Port> inputPorts = connectedNode.inputContainer.Query<Port>().ToList();
                        Port connectedPort = inputPorts.Where(port => port.connected == false && port.portType.IsAssignableFrom(outputPort.portType)).FirstOrDefault();

                        //connect ports
                        Edge edge = outputPort.ConnectTo(connectedPort);
                        graph.AddElement(edge);
                        node.RefreshPorts();
                    }
                }
            }
        }

        #endregion
    }
}
#endif