namespace redd096
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(MapManager))]
    public class MapManagerEditor : Editor
    {
        private MapManager mapManager;

        private void OnEnable()
        {
            mapManager = target as MapManager;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Regen Map - OLD"))
            {
                mapManager.DestroyMap();
                mapManager.CreateEditorMap();

                //set undo
                Undo.RegisterFullObjectHierarchyUndo(target, "Regen World");
            }

            if (GUILayout.Button("Destroy Map"))
            {
                mapManager.DestroyMap();

                //set undo
                Undo.RegisterFullObjectHierarchyUndo(target, "Regen World");
            }
        }
    }
}