using System.Collections.Generic;
using System.IO;
using UnityEngine;
using redd096.Attributes;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.IconGenerator
{
    /// <summary>
    /// Generate icons for prefabs
    /// </summary>
    [AddComponentMenu("redd096/IconGenerator/Icon Generator")]
    public class IconGenerator : MonoBehaviour
    {
        [Header("Prefabs")]
        [Tooltip("IconGenerator will generate icons from these prefabs / objects")] [SerializeField] GameObject[] prefabs;
        [Tooltip("Use these names instead of prefab name")] [SerializeField] FPrefabName[] overwriteNames;
        [Tooltip("These images will be applied to EVERY icon generated. Higher index = on top")] [SerializeField] Sprite[] overlays;

        [Header("Directory path inside this project Assets/ folder")]
        [SerializeField] string directoryPath = "Icons";

        [Header("Background")]
        [SerializeField] bool replaceBackgroundColor = false;
        [Tooltip("In the icon, replace every pixel with this color")][SerializeField] Color defaultBackgroundColor;
        [Tooltip("This is the color to use for the icon background")][SerializeField] Color newColorForBackground;

        private List<Texture2D> overlayIcons = new List<Texture2D>();
        string path;

        [Button]
        void GenerateIcons()
        {
            //be sure there are prefabs
            if (prefabs == null || prefabs.Length <= 0)
            {
                Debug.LogError("You need to specify prefabs!");
                return;
            }

            //be sure there is the directory
            path = Path.Combine(Application.dataPath, directoryPath);
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            //get overlays and generate icons
            GetOverlayTextures();
            GeneratePrefabsIcons();

        }

        private async void GetOverlayTextures()
        {
            for (int i = 0; i < overlays.Length; i++)
            {
                if (overlays[i] == null)
                    continue;

                string overlayPath = AssetDatabase.GetAssetPath(overlays[i]);
                byte[] fileData = File.ReadAllBytes(overlayPath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                if (tex.height != 128)
                    TextureScale.Point(tex, 128, 128);

                //Debug.Log("Processing overlay: " + overlayIcons.Count.ToString());
                overlayIcons.Add(tex);

                //Only uncomment these lines, if you know what you are doing. All overlays will be exported to a folder.
                //byte[] bytes = tex.EncodeToPNG();
                //File.WriteAllBytes(Application.dataPath + "/" + "debug" + "/" + overlays[i].name + ".png", bytes);

                EditorUtility.DisplayProgressBar("Read overlays", $"Reading... {i}/{overlays.Length}", (float)i / overlays.Length);
                if (i % 20 == 0)
                    await Task.Delay((int)(Time.deltaTime * 1000));
            }

            EditorUtility.ClearProgressBar();
        }

        private async void GeneratePrefabsIcons()
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                GameObject prefab = prefabs[i];

                if (prefab == null)
                    return;

                //get prefab icon and name
                Texture rawIcon = AssetPreview.GetAssetPreview(prefab);
                Texture2D icon = rawIcon as Texture2D;
                string iconName = GetPrefabName(prefab, i);

                //be sure there is an asset preview
                if (icon == null)
                {
                    Debug.LogError($"There was an error generating icon for {prefab}. Maybe you have to select it in editor to makes unity generate its preview, " +
                        $"or maybe this isn't an element that unity can generates preview?");

                    return;
                }

                //apply overlays
                icon = IconHelper.ApplyOverlaysToTexture(icon, overlayIcons);

                //replace background color
                if (replaceBackgroundColor)
                    IconHelper.ReplaceColor(icon, defaultBackgroundColor, newColorForBackground);

                //create texture in project
                //TextureScale.Point(icon, 512, 512); // Used for rescaling the final icon
                byte[] bytes = icon.EncodeToPNG();
                string iconPath = Path.Combine(path, iconName);
                File.WriteAllBytes($"{iconPath}.png", bytes);
                Debug.Log($"File saved in: {iconPath}.png");

                EditorUtility.DisplayProgressBar("Generate icons", $"Generating... {i}/{prefabs.Length}", (float)i / prefabs.Length);
                if (i % 20 == 0)
                    await Task.Delay((int)(Time.deltaTime * 1000));
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        string GetPrefabName(GameObject prefab, int prefabIndex)
        {
            if (overwriteNames == null)
                return prefab.name;

            //find overwrite name
            for (int i = 0; i < overwriteNames.Length; i++)
            {
                if (overwriteNames[i].PrefabIndex == prefabIndex)
                {
                    if (string.IsNullOrEmpty(overwriteNames[i].IconName))
                    {
                        Debug.LogError($"OverwriteNames element {i}, has prefab index {prefabIndex} but the icon name is empty. We will use prefab.name");
                        return prefab.name;
                    }

                    //and return it
                    return overwriteNames[i].IconName;
                }
            }

            //else use prefab.name
            return prefab.name;
        }

        [System.Serializable]
        public struct FPrefabName
        {
            [Tooltip("This is the index inside prefabs list")] public int PrefabIndex;
            [Tooltip("This is the name to use instead of prefab name")] public string IconName;
        }
    }
}