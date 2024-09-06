using System.IO;
using redd096.Attributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.IconGenerator
{
    /// <summary>
    /// Call function to generate a png from the camera renderer
    /// </summary>
    [AddComponentMenu("redd096/IconGenerator/Icon Generator From Camera")]
    public class IconGeneratorFromCamera : MonoBehaviour
    {
        [Header("Render main camera to this renderTexture, then save as png")]
        [SerializeField] RenderTexture renderTexture;
        [SerializeField] string fileName = "image000";
        [SerializeField] string filePathInsideAssets = "Textures/Icons";
        [SerializeField] bool changeCameraBackground = true;
        [SerializeField] Color cameraBackgroundColor = Color.clear;

        [Button]
        public void GenerateIcon()
        {
            Camera cam = Camera.main;

            //save previous variables
            RenderTexture previousTargetTexture = cam.targetTexture;
            CameraClearFlags previousClearFlags = cam.clearFlags;
            Color previousBackgroundColor = cam.backgroundColor;
            RenderTexture previousActiveRenderTexture = RenderTexture.active;

            //set variables for icon generator
            cam.targetTexture = renderTexture;
            if (changeCameraBackground)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = cameraBackgroundColor;
            }
            RenderTexture.active = cam.targetTexture;

            //render, apply to texture 2d and encode to png
            cam.Render();
            Texture2D texture = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
            texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            texture.Apply();
            byte[] bytes = texture.EncodeToPNG();

            //restore variables
            if (Application.isPlaying) Destroy(texture);
            else DestroyImmediate(texture);

            cam.targetTexture = previousTargetTexture;
            cam.clearFlags = previousClearFlags;
            cam.backgroundColor = previousBackgroundColor;
            RenderTexture.active = previousActiveRenderTexture;

            //then save
            string path = Path.Combine(Application.dataPath, filePathInsideAssets);
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            path = Path.Combine(path, fileName + ".png");
            File.WriteAllBytes(path, bytes);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        [Button]
        void OpenFolderPath()
        {
#if UNITY_EDITOR
            string path = Path.Combine("Assets", filePathInsideAssets);
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            Selection.activeObject = obj;       //select folder
            EditorGUIUtility.PingObject(obj);   //highlight yellow
#endif
        }
    }
}