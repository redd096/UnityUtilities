using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/MonoBehaviours/FPS Counter")]
    public class FPSCounter : MonoBehaviour
    {
        [Header("Use GUI or UI Text - default UI get in children")]
        [SerializeField] bool useOnGUI = true;
        [SerializeField] UnityEngine.UI.Text uiText = default;

        private static FPSCounter instance;
        private int countFPS;

        private int lastFrameIndex;
        private float[] frameDeltaTimeArray;

        private void Awake()
        {
            //check singleton
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            //create array
            frameDeltaTimeArray = new float[50];

            //get references
            if (uiText == null)
                uiText = GetComponentInChildren<UnityEngine.UI.Text>();

            //warning
            if (useOnGUI == false && uiText == null) Debug.LogWarning("Missing UIText on FPS Counter - " + gameObject);
        }

        private void Update()
        {
            //add current delta time to array
            frameDeltaTimeArray[lastFrameIndex] = Time.unscaledDeltaTime;
            lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length; //always +1, when reach last index restart from 0

            //calculate FPS
            countFPS = Mathf.RoundToInt(CalculateFPS());

            //if not use OnGUI, set UI Text
            if (useOnGUI == false && uiText)
                uiText.text = "FPS: " + countFPS;
        }

        private void OnGUI()
        {
            //if use OnGUI, set it
            if (useOnGUI)
                GUI.Label(new Rect(30, 40, 100, 25), "FPS: " + countFPS);
        }

        private float CalculateFPS()
        {
            //calculate average between every saved delta time
            float total = 0f;
            foreach (float deltaTime in frameDeltaTimeArray)
            {
                total += deltaTime;
            }

            return frameDeltaTimeArray.Length / total;
        }
    }
}