#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//https://docs.unity3d.com/Manual/UIE-ElementRef.html

namespace redd096.NodesGraph.Editor
{
    /// <summary>
    /// Simple utilities to create various UI Elements
    /// </summary>
    public static class CreateElementsUtilities
    {
        public static Button CreateButton(string text, System.Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text,
            };

            return button;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collapsed,
            };

            return foldout;
        }

        public static Port CreatePort(Node node, string portName, Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, type);
            port.portName = portName;

            return port;
        }

        public static TextField CreatetextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            //create textfield
            TextField textField = new TextField()
            {
                value = value,
                label = label,
            };

            //register event if not null
            if (onValueChanged != null)
                textField.RegisterValueChangedCallback(onValueChanged);

            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreatetextField(value, label, onValueChanged);
            textArea.multiline = true;
            return textArea;
        }

        public static Label CreateLabel(string value)
        {
            Label label = new Label(value);
            return label;
        }

        public static ObjectField CreateObjectField(string label, System.Type objectType, EventCallback<ChangeEvent<Object>> onValueChanged = null, bool allowSceneObjects = false)
        {
            //create object field
            ObjectField objectField = new ObjectField(label)
            {
                objectType = objectType,
                allowSceneObjects = allowSceneObjects
            };

            //register event if not null
            if (onValueChanged != null)
                objectField.RegisterValueChangedCallback(onValueChanged);

            return objectField;
        }
    }
}
#endif