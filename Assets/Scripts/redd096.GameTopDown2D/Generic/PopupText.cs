using UnityEngine;
using UnityEngine.UI;
using TMPro;
using redd096.Attributes;

//create a prefab with this script and instantiate where you need

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Generic/Popup Text")]
    public class PopupText : MonoBehaviour
    {
        [Header("Popup - default get from children")]
        public bool UseTextMeshPro = false;
        [HideIf("UseTextMeshPro")] public Text TextUI = default;
        [ShowIf("UseTextMeshPro")] public TextMeshPro TextMesh = default;
        [SerializeField] string[] possibleTexts = default;

        [Header("Movement")]
        [SerializeField] Vector2 movement = Vector2.up * 2f;
        [Tooltip("Float without fade")] [SerializeField] float floatTime = 0.1f;
        [Tooltip("After floatTime, continue to float but start to fade")] [SerializeField] float fadeDuration = 2f;
        [Tooltip("Deactivate instead of Destroy")] [SerializeField] bool usePooling = true;

        Color startColor;
        Color textColor;
        float time; //from floatTime to 0

        void Awake()
        {
            //get refs
            if (TextUI == null) TextUI = GetComponentInChildren<Text>();
            if (TextMesh == null) TextMesh = GetComponentInChildren<TextMeshPro>();

            //save start color
            if (UseTextMeshPro && TextMesh)
                startColor = TextMesh.color;
            else if (UseTextMeshPro == false && TextUI)
                startColor = TextUI.color;
        }

        bool CheckTextNotNull()
        {
            //return if used text is not null
            return (UseTextMeshPro && TextMesh) || (UseTextMeshPro == false && TextUI);
        }

        void Update()
        {
            if (CheckTextNotNull() == false)
                return;

            //move text
            if (UseTextMeshPro) TextMesh.transform.position += (Vector3)movement * Time.deltaTime;
            else TextUI.transform.position += (Vector3)movement * Time.deltaTime;

            //finished float time
            time -= Time.deltaTime;
            if (time <= 0)
            {
                //start fade out
                textColor.a -= Time.deltaTime / fadeDuration;

                //set text color
                if (UseTextMeshPro) TextMesh.color = textColor;
                else TextUI.color = textColor;

                //destroy when alpha at 0
                if (textColor.a <= 0f)
                {
                    if (usePooling)
                        gameObject.SetActive(false);
                    else
                        Destroy(gameObject);
                }
            }
        }

        #region public API

        /// <summary>
        /// Initialize to set one random text from the array in the prefab
        /// </summary>
        public void Init()
        {
            Init(possibleTexts);
        }

        /// <summary>
        /// Initialize to set text
        /// </summary>
        /// <param name="textToUse"></param>
        public void Init(string textToUse)
        {
            Init(new string[1] { textToUse });
        }

        /// <summary>
        /// Initialize to set one random text from array
        /// </summary>
        /// <param name="textsToUse"></param>
        public void Init(string[] textsToUse)
        {
            if (CheckTextNotNull() == false)
                return;

            if (textsToUse == null || textsToUse.Length <= 0)
            {
                Debug.LogWarning("Miss texts on " + name);
                return;
            }

            //set text
            if (UseTextMeshPro)
                TextMesh.SetText(textsToUse[Random.Range(0, textsToUse.Length)]);
            else
                TextUI.text = textsToUse[Random.Range(0, textsToUse.Length)];

            //and reset (necessary for pooling)
            time = floatTime;
            textColor = startColor;

            //set text color
            if (UseTextMeshPro) TextMesh.color = textColor;
            else TextUI.color = textColor;
        }

        #endregion
    }
}