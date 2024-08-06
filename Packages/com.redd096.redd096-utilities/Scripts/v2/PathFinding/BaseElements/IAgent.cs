
namespace redd096.v2.PathFinding
{
    /// <summary>
    /// We use this interface to check if this element can move on the nodes of a grid. 
    /// For example, we check if this agent is too big to move on this node, if nodes around are walls
    /// </summary>
    public interface IAgent
    {
        bool CanMoveOnThisNode(Node node, Node[,] grid);
    }
}