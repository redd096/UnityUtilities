using UnityEngine;

namespace redd096.CsvImporter.Example
{
    /// <summary>
    /// Example scriptable object
    /// </summary>
    public class ExampleScriptableObject : ScriptableObject
    {
        public string Name;
        public int Life;
        public EExampleEnum Type;
        public string[] ExampleArrayString;
        public int[] ExampleArrayInt;
    }

    public enum EExampleEnum
    {
        Normale, Fuoco, Erba, Acqua
    }
}