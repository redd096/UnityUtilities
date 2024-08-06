
namespace redd096.v2.PathFinding
{
    /// <summary>
    /// Use this interface to mark the obstacles. They will update nodes walkable or movement penalty, without recreate all the grid
    /// </summary>
    public interface IObstacle
    {
        bool IsUnwalkable();
        int GetMovementPenalty();
    }
}