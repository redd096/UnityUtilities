using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
//used by preprocess build, disabled now for the package. Enable if you need it
//using UnityEditor.Build.Reporting;
//using UnityEditor.Build;
#endif

public class ExampleProjectSettingsAndPreprocessBuild : ScriptableObject
{
    public bool isTestBuild = false;
    public Texture2D normalBuildIcon = default;
    public Texture2D testBuildIcon = default;

    #region editor
#if UNITY_EDITOR

    public const string PATH = "Assets/Resources/BuildCustomSettings.asset";    //asset path
    public const string NAME_EDITOR = "Project/Example Build Custom Settings";  //name in editor
    public const SettingsScope SETTINGS = SettingsScope.Project;                //project = project settings / user = preferences

    public static SerializedProperty[] GetSerializedProperties(SerializedObject serializedObject)
    {
        List<SerializedProperty> serializedProperties = new List<SerializedProperty>();

        //get every serialized property
        SerializedProperty iterator = serializedObject.GetIterator();   //first property is "Base"
        iterator.NextVisible(true);                                     //second property is "m_Script"
        while (iterator.NextVisible(enterChildren: true))
        {
            SerializedProperty property = serializedObject.FindProperty(iterator.name);
            if (serializedProperties.Contains(property) == false)
                serializedProperties.Add(property);
        }

        return serializedProperties.ToArray();
    }

    //enable this to add manually every serialized variable. Check also OnGUI and CreateBuildCustomSettingsProvider
    //public class Styles
    //{
    //    public static GUIContent isTestBuild = new GUIContent("Is Test Build");
    //    public static GUIContent normalBuildIcon = new GUIContent("Normal Build Icon");
    //    public static GUIContent testBuildIcon = new GUIContent("Test Build Icon");
    //}

#endif
    #endregion
}

#region settings provider
#if UNITY_EDITOR
/*
public class BuildCustomSettingsProvider : SettingsProvider
{
    private SerializedObject buildCustomSettings;
    private SerializedProperty[] serializedProperties;

    public BuildCustomSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
        : base(path, scopes, keywords) { }

    // This function is called when the user clicks on this element in the Settings window.
    public override void OnActivate(string searchContext, UnityEngine.UIElements.VisualElement rootElement)
    {
        base.OnActivate(searchContext, rootElement);

        //be sure there are the folders to the path
        string[] pathFolders = ExampleProjectSettingsAndPreprocessBuild.PATH.Split("/");    //split every folder
        string currentPath = pathFolders.Length > 1 ? pathFolders[0] : "";
        for (int i = 1; i < pathFolders.Length - 1; i++)                                    //-1 because last one is asset name
        {
            if (AssetDatabase.IsValidFolder($"{currentPath}/{pathFolders[i]}") == false)
                AssetDatabase.CreateFolder(currentPath, pathFolders[i]);

            currentPath += $"/{pathFolders[i]}";                                            //update current reached path
        }

        //try find scriptable or create one
        var settings = AssetDatabase.LoadAssetAtPath<ExampleProjectSettingsAndPreprocessBuild>(ExampleProjectSettingsAndPreprocessBuild.PATH);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<ExampleProjectSettingsAndPreprocessBuild>();
            AssetDatabase.CreateAsset(settings, ExampleProjectSettingsAndPreprocessBuild.PATH);
            AssetDatabase.SaveAssets();
        }
        buildCustomSettings = new SerializedObject(settings);

        //get serialized properties
        serializedProperties = ExampleProjectSettingsAndPreprocessBuild.GetSerializedProperties(buildCustomSettings);
    }

    public override void OnGUI(string searchContext)
    {
        EditorGUI.BeginChangeCheck();

        // Use IMGUI to display UI:
        //EditorGUILayout.PropertyField(buildCustomSettings.FindProperty("isTestBuild"), BuildCustomSettings.Styles.isTestBuild));
        //EditorGUILayout.PropertyField(buildCustomSettings.FindProperty("normalBuildIcon"), BuildCustomSettings.Styles.normalBuildIcon));
        //EditorGUILayout.PropertyField(buildCustomSettings.FindProperty("testBuildIcon"), BuildCustomSettings.Styles.testBuildIcon));

        for (int i = 0; i < serializedProperties.Length; i++)
        {
            //null-check because for some reason arrays return 2 properties: one is the array and the second is null
            if (serializedProperties[i] != null)
                EditorGUILayout.PropertyField(serializedProperties[i], new GUIContent(serializedProperties[i].displayName));
        }

        if (EditorGUI.EndChangeCheck())
            buildCustomSettings.ApplyModifiedProperties();
    }

    // Register the SettingsProvider
    [SettingsProvider]
    public static SettingsProvider CreateBuildCustomSettingsProvider()
    {
        //create provider - SettingsScope.Project for projectSettings, SettingsScope.User for preferences
        var provider = new BuildCustomSettingsProvider(ExampleProjectSettingsAndPreprocessBuild.NAME_EDITOR, ExampleProjectSettingsAndPreprocessBuild.SETTINGS);

        // Automatically extract all keywords from the Styles.
        //provider.keywords = GetSearchKeywordsFromGUIContentProperties<BuildCustomSettings.Styles>();

        provider.keywords = GetSearchKeywordsFromPath(ExampleProjectSettingsAndPreprocessBuild.PATH);
        return provider;
    }
}
*/
#endif
#endregion

#region preprocess build
#if UNITY_EDITOR
/*
public class BuildCustomPreprocessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        //Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);

        //set icon if test or normal build
        ExampleProjectSettingsAndPreprocessBuild buildCustomSettings = Resources.Load<ExampleProjectSettingsAndPreprocessBuild>("BuildCustomSettings");
        if (buildCustomSettings)
        {
            Texture2D icon = buildCustomSettings.isTestBuild ? buildCustomSettings.testBuildIcon : buildCustomSettings.normalBuildIcon;
            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { icon }, IconKind.Any);
        }
    }
}
*/
#endif
#endregion

#region override Build and Build And Run buttons
/*
/// <summary>
/// When press Build or Build And Run, show a popup before start the build
/// </summary>
public class PopupBeforeBuild
{
    private enum EDevices { RealWear, Vuzix, Touch }

    [InitializeOnLoadMethod]
    private static void ShowPopupBeforeBuild()
    {
        //overwrite build button function
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }

    private static void BuildPlayerHandler(BuildPlayerOptions options)
    {
        //show dialogue -> return true if press Yes, return false if press No
        //bool confirm = EditorUtility.DisplayDialog("What to do with managers", "Deactivate RealWearManager?", "Yes", "No");
        //OnClickMenuItem(new FUserData(options, confirm ? 1 : 0));

        string[] devices = System.Enum.GetNames(typeof(EDevices));

        //show popup and set click event
        GenericMenu menu = new GenericMenu();
        for (int i = 0; i < devices.Length; i++)
        {
            bool isSelected = false;
            object userData = new FUserData(options, i);
            menu.AddItem(new GUIContent(devices[i]), isSelected, OnClickMenuItem, userData);
        }
        menu.ShowAsContext();
    }

    private static async void OnClickMenuItem(object userData)
    {
        FUserData data = (FUserData)userData;
        EDevices selectedDevice = (EDevices)data.menuElementIndex;

        //load start scene
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(data.options.scenes[0], UnityEditor.SceneManagement.OpenSceneMode.Additive);
        while (scene.isLoaded == false)
        {
            await System.Threading.Tasks.Task.Delay(1000);
        }

        //set managers
        RealWearManager realWearManager = Object.FindObjectOfType<RealWearManager>(true);
        VuzixManager vuzixManager = Object.FindObjectOfType<VuzixManager>(true);
        if (selectedDevice == EDevices.RealWear)
        {
            realWearManager.gameObject.SetActive(true);
            vuzixManager.gameObject.SetActive(false);
        }
        else if (selectedDevice == EDevices.Vuzix)
        {
            realWearManager.gameObject.SetActive(false);
            vuzixManager.gameObject.SetActive(true);
        }
        else
        {
            realWearManager.gameObject.SetActive(false);
            vuzixManager.gameObject.SetActive(false);
        }

        //start build
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(data.options);
    }

    private struct FUserData
    {
        public BuildPlayerOptions options;
        public int menuElementIndex;

        public FUserData(BuildPlayerOptions options, int menuElementIndex)
        {
            this.options = options;
            this.menuElementIndex = menuElementIndex;
        }
    }
}
*/
#endregion