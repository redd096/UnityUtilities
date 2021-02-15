namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class GridBase : MonoBehaviour
    {
        [Header("Use Z instead of Y")]
        [SerializeField] bool useZ = true;

        [Header("Grid Base")]
        [SerializeField] protected Vector3 startPosition = Vector3.zero;
        [SerializeField] protected Vector3 tileSize = Vector3.one;

        protected Dictionary<Vector2Int, TileBase> grid = new Dictionary<Vector2Int, TileBase>();

        protected virtual void Awake()
        {
            //update in editor doesn't save dictionary, so we need to regenerate it
            GenerateReferences();
        }

        #region regen grid

        protected void RemoveOldGrid()
        {
            //remove every child
            foreach (Transform child in transform)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }

            //then clear dictionary
            grid.Clear();
        }

        protected virtual void GenerateGrid(int gridSizeX, int gridsizeY)
        {
            //for every pixel in image
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridsizeY; y++)
                {
                    //instantiate tile
                    TileBase tile = Instantiate(GetTilePrefab(x, y), transform);
                    tile.transform.position = startPosition +                   //from start position
                        (useZ ?
                        new Vector3(x * tileSize.x, 0, y * tileSize.z) :        //if use Z, move on X and Z
                        new Vector3(x * tileSize.x, y * tileSize.y, 0));        //if use Y, move on X and Y
                    tile.transform.rotation = Quaternion.identity;  //set rotation

                    //init tile and add to dictionary
                    Vector2Int positionInGrid = new Vector2Int(x, y);
                    tile.Init(positionInGrid);
                    grid.Add(positionInGrid, tile);
                }
            }
        }

        #endregion

        #region awake

        void GenerateReferences()
        {
            //create dictionary
            grid.Clear();
            foreach (Transform child in transform)
            {
                TileBase tile = child.GetComponent<TileBase>();
                if (tile != null)
                {
                    Vector2Int index = new Vector2Int(Mathf.FloorToInt(tile.transform.position.x / tileSize.x),
                        useZ ?
                        Mathf.FloorToInt(tile.transform.position.z / tileSize.z) :  //if use Z, check Z axis
                        Mathf.FloorToInt(tile.transform.position.y / tileSize.y));  //if use Y, check Y axis

                    grid.Add(index, tile);
                }
            }
        }

        #endregion

        /// <summary>
        /// Get Tile Prefab
        /// </summary>
        protected abstract TileBase GetTilePrefab(int x, int y);
    }
}