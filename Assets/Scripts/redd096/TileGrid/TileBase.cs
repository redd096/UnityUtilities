namespace redd096
{
    using UnityEngine;

    public abstract class TileBase : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] protected Vector2Int positionInGrid;

        public void Init(Vector2Int positionInGrid)
        {
            this.positionInGrid = positionInGrid;
        }
    }
}