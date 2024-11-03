
namespace redd096.NodesGraph.Runtime
{
    /// <summary>
    /// Data to save node output
    /// </summary>
    [System.Serializable]
    public class NodeOutputData
    {
        public string OutputType;
        public string ConnectedNodeID;
        public int ConnectedPortIndex;
    }
}