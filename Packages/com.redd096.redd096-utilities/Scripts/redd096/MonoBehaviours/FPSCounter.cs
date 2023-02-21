using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/MonoBehaviours/FPS Counter")]
    public class FPSCounter : MonoBehaviour
    {
        [Header("Show on Screen? Only editor or also in build?")]
        [SerializeField] bool showOnScreen = true;
        [SerializeField] bool showAlsoInBuild = false;

        [Header("Use GUI or UI Text")]
        [SerializeField] bool useOnGUI = false;
        [SerializeField] UnityEngine.UI.Text uiText = default;

        [Header("DEBUG - Set FPS Limit")]
        [SerializeField] bool setFpsLimit = false;
        [SerializeField][Min(-1)] int limitFps = -1;

        private static FPSCounter instance;
        private int countFPS;

        private int lastFrameIndex;
        private float[] frameDeltaTimeArray;

        public int CurrentFPS => countFPS;

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
                return;
            }

            //create array
            frameDeltaTimeArray = new float[50];

            //hide canvas if can't show
            if (CanShow() == false)
            {
                if (uiText != null)
                    uiText.gameObject.SetActive(false);
            }
            //else show warning if not setted
            else
            {
                if (CanShow() && useOnGUI == false && uiText == null)
                    Debug.LogWarning($"Missing UIText on {this} - {gameObject}");
            }

            //set fps limit if necessary
            if (setFpsLimit)
                Application.targetFrameRate = limitFps;
        }

        private void OnValidate()
        {
            //when change in inspector, be sure to update also canvas
            if (CanShow() == false || useOnGUI)
            {
                if (uiText != null)
                    uiText.gameObject.SetActive(false);
            }
            else if (CanShow() && useOnGUI == false)
            {
                if (uiText != null)
                    uiText.gameObject.SetActive(true);
            }

            //set fps limit if necessary
            if (setFpsLimit)
                Application.targetFrameRate = limitFps;
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
            if (useOnGUI && CanShow())
                GUI.Label(new Rect(30, 40, 100, 25), "FPS: " + countFPS);
        }

        #region private API

        private bool CanShow()
        {
            //must be shown on screen, and is in editor or is enabled also in build
            return showOnScreen && (Application.isEditor || showAlsoInBuild);
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

        #endregion
    }
}