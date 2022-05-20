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
        [SerializeField] bool useTextMeshPro = false;
        [HideIf("useTextMeshPro")] [SerializeField] Text text = default;
        [ShowIf("useTextMeshPro")] [SerializeField] TextMeshPro textMesh = default;
        [SerializeField] string[] possibleTexts = default;

        [Header("Movement")]
        [SerializeField] Vector2 movement = Vector2.up * 20f;
        [Tooltip("Float without fade")] [SerializeField] float floatTime = 2f;
        [Tooltip("After floatTime, continue to float but start to fade")] [SerializeField] float fadeDuration = 3f;
        [Tooltip("Deactivate instead of Destroy")] [SerializeField] bool usePooling = true;

        Color startColor;
        Color textColor;
        float time; //from floatTime to 0

        void Awake()
        {
            //get refs
            if (text == null) text = GetComponentInChildren<Text>();
            if (textMesh == null) textMesh = GetComponentInChildren<TextMeshPro>();

            //save start color
            if (useTextMeshPro && textMesh)
                startColor = textMesh.color;
            else if (useTextMeshPro == false && text)
                startColor = text.color;
        }

        bool CheckTextNotNull()
        {
            //return if used text is not null
            return (useTextMeshPro && textMesh) || (useTextMeshPro == false && text);
        }

        void Update()
        {
            if (CheckTextNotNull() == false)
                return;

            //move text
            if (useTextMeshPro) textMesh.transform.position += (Vector3)movement * Time.deltaTime;
            else text.transform.position += (Vector3)movement * Time.deltaTime;

            //finished float time
            time -= Time.deltaTime;
            if (time <= 0)
            {
                //start fade out
                textColor.a -= Time.deltaTime / fadeDuration;

                //set text color
                if (useTextMeshPro) textMesh.color = textColor;
                else text.color = textColor;

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

            //set text
            if (useTextMeshPro)
                textMesh.SetText(textsToUse[Random.Range(0, textsToUse.Length)]);
            else
                text.text = textsToUse[Random.Range(0, textsToUse.Length)];

            //and reset (necessary for pooling)
            time = floatTime;
            textColor = startColor;

            //set text color
            if (useTextMeshPro) textMesh.color = textColor;
            else text.color = textColor;
        }

        #endregion
    }
}