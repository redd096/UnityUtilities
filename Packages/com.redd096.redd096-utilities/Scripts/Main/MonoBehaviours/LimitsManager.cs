using redd096.Attributes;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Generate Colliders at the limits of the camera
    /// </summary>
    [AddComponentMenu("redd096/Main/MonoBehaviours/Limits Manager")]
    public class LimitsManager : MonoBehaviour
    {
        [Header("If enabled, continue update with screen size")]
        [Tooltip("If this isn't enabled, you can generate walls in editor with a button in inspector, or in awake with this set at true")][SerializeField] bool regenOnAwake;
        [Tooltip("Use 3d (XZ and BoxCollider) or 2d (XY and BoxCollider2D)")][SerializeField] bool is3D;

        Camera cam;
        float width;
        float height;

        private void Awake()
        {
            //regen on awake
            if (regenOnAwake)
                GenerateLimits();
        }

        private void Update()
        {
            //if there aren't 4 limits, don't resize
            if (transform.childCount < 4)
                return;

            //if different screen size, resize limits
            if (Screen.width != width || Screen.height != height)
            {
                width = Screen.width;
                height = Screen.height;

                SetSize();
            }
        }

        [Button]
        void GenerateLimits()
        {
            cam = Camera.main;

            DestroyPrevious();
            CreateNew();
            SetSize();
        }

        #region private API

        /// <summary>
        /// Remove every child
        /// </summary>
        void DestroyPrevious()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Create new objects
        /// </summary>
        void CreateNew()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject obj = is3D ? GameObject.CreatePrimitive(PrimitiveType.Cube) : new GameObject("Limit", typeof(BoxCollider2D));
                obj.name = "Limit";
                obj.transform.SetParent(transform);
            }
        }

        /// <summary>
        /// Set size for every limit
        /// </summary>
        void SetSize()
        {
            Transform[] limits = new Transform[4];
            for (int i = 0; i < 4; i++)
                limits[i] = transform.GetChild(i);

            GetViewportToWorldSize(out Vector3 leftDown, out Vector3 rightUp, out Vector3 center, out Vector3 size, out Vector3 halfSize);
            if (is3D == false)
                size = new Vector3(size.x, size.y, 1);

            //right
            limits[0].position = is3D ? new Vector3(rightUp.x + halfSize.x, center.y, center.z) : new Vector3(rightUp.x + halfSize.x, center.y, 0);
            limits[0].localScale = size;

            //left
            limits[1].position = is3D ? new Vector3(leftDown.x - halfSize.x, center.y, center.z) : new Vector3(leftDown.x - halfSize.x, center.y, 0);
            limits[1].localScale = size;

            //up
            limits[2].position = is3D ? new Vector3(center.x, center.y, rightUp.z + halfSize.z) : new Vector3(center.x, rightUp.y + halfSize.y, 0);
            limits[2].localScale = size;

            //down
            limits[3].position = is3D ? new Vector3(center.x, center.y, leftDown.z - halfSize.z) : new Vector3(center.x, leftDown.y - halfSize.y, 0);
            limits[3].localScale = size;
        }

        /// <summary>
        /// Calculate points of the screen in world space
        /// </summary>
        /// <param name="leftDown"></param>
        /// <param name="rightUp"></param>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="halfSize"></param>
        void GetViewportToWorldSize(out Vector3 leftDown, out Vector3 rightUp, out Vector3 center, out Vector3 size, out Vector3 halfSize)
        {
            float depthScreen = cam.WorldToViewportPoint(transform.position).z;

            leftDown = cam.ViewportToWorldPoint(new Vector3(0, 0, depthScreen));
            rightUp = cam.ViewportToWorldPoint(new Vector3(1, 1, depthScreen));

            size = rightUp - leftDown;
            halfSize = size * 0.5f;

            center = rightUp - halfSize;
        }

        #endregion
    }
}