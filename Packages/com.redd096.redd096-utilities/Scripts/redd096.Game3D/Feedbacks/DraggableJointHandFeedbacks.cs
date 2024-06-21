using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Show hand and handle in game
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/Feedbacks/DraggableJoint Hand Feedbacks")]
    public class DraggableJointHandFeedbacks : MonoBehaviour
    {
        [Header("Necessary Components (by default get from this gameObject)")]
        [SerializeField] DraggableJointInteractable draggable;
        [SerializeField] bool showHandle = true;
        [SerializeField] bool showHand = false;
        [SerializeField] Color handleColor = Color.red - new Color(0, 0, 0, 0.7f);
        [SerializeField] Color handColor = Color.yellow - new Color(0, 0, 0, 0.7f);

        //static to generate only one and make it common between every feedback of this type
        static Material mat;
        GameObject handle;
        GameObject hand;

        private void Awake()
        {
            //be sure to have components
            if (draggable == null && TryGetComponent(out draggable) == false)
                Debug.LogError("Miss DraggableJointInteractable on " + name, gameObject);
            if (mat == null)
                mat = new Material(Shader.Find("Transparent/Diffuse"));

            //add events
            if (draggable)
            {
                draggable.onInteract += OnInteract;
                draggable.onDismiss += OnDismiss;
            }
        }

        private void OnDestroy()
        {
            //remove events
            if (draggable)
            {
                draggable.onInteract -= OnInteract;
                draggable.onDismiss -= OnDismiss;
            }
        }

        private void OnInteract()
        {
            //create or active objects
            if (showHandle)
                handle = ShowObject(handle, draggable.Handle, handleColor);

            if (showHand)
                hand = ShowObject(hand, draggable.HandRb.transform, handColor);
        }

        private void OnDismiss()
        {
            //deactive objects (probably not necessary, because handle and hand parents will be destroyed on dismiss)
            if (handle)
                handle.SetActive(false);

            if (hand)
                hand.SetActive(false);
        }

        private GameObject ShowObject(GameObject go, Transform parent, Color color)
        {
            //generate sphere without collider
            if (go == null) go = GenerateSphere(color);

            //set child of handle or hand
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;

            //and active object if necessary
            go.SetActive(true);

            return go;
        }

        private GameObject GenerateSphere(Color color)
        {
            //create sphere and set size
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = Vector3.one * 0.1f;

            //remove collider
            Collider col = go.GetComponent<Collider>();
            if (col) Destroy(col);

            //set color
            Renderer r = go.GetComponent<Renderer>();
            if (r)
            {
                r.material = mat;
                r.material.color = color;
            }

            return go;
        }
    }
}