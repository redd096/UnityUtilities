#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//https://docs.unity3d.com/Manual/UIE-ElementRef.html
//https://docs.unity3d.com/2020.1/Documentation/ScriptReference/UIElements.Image.html

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

        public static Foldout CreateFoldoutWithButton(string foldoutTitle, string buttonText, out Button button, bool collapsed = false, System.Action onClick = null)
        {
            //create foldout
            Foldout foldout = CreateFoldout(foldoutTitle, collapsed);

            //create a button inside foldout
            button = CreateButton(buttonText, onClick);
            foldout.Add(button);

            return foldout;
        }

        public static Port CreatePort(Node node, string portName, Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, type);
            port.portName = portName;

            return port;
        }

        /// <summary>
        /// Create a TextField
        /// </summary>
        /// <param name="label">Name to visualize to the left of the textfield</param>
        /// <param name="value">Text to visualize inside the textfield</param>
        /// <param name="onValueChanged"></param>
        /// <returns></returns>
        public static TextField CreateTextField(string label, string value, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            //create textfield
            TextField textField = new TextField()
            {
                label = label,
                value = value,
            };

            //register event if not null
            if (onValueChanged != null)
                textField.RegisterValueChangedCallback(onValueChanged);

            return textField;
        }

        public static TextField CreateTextArea(string label, string value, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            //create textfield multiline
            TextField textArea = CreateTextField(label, value, onValueChanged);
            textArea.multiline = true;
            return textArea;
        }

        public static Label CreateLabel(string value)
        {
            Label label = new Label(value);
            return label;
        }

        public static ObjectField CreateObjectField(string label, Object value, System.Type objectType, EventCallback<ChangeEvent<Object>> onValueChanged = null, bool allowSceneObjects = false)
        {
            //create object field
            ObjectField objectField = new ObjectField(label)
            {
                value = value,
                objectType = objectType,
                allowSceneObjects = allowSceneObjects
            };

            //register event if not null
            if (onValueChanged != null)
                objectField.RegisterValueChangedCallback(onValueChanged);

            return objectField;
        }

        /// <summary>
        /// Create an ObjectField of type Sprite, with an Image bottom
        /// </summary>
        /// <param name="label"></param>
        /// <param name="previewSize"></param>
        /// <param name="onChangeValue"></param>
        public static ObjectField CreateObjectFieldWithPreview(string label, Sprite value, Vector2 previewSize, out Image previewImage, EventCallback<ChangeEvent<Object>> onChangeValue = null)
        {
            //create image for preview
            Image image = CreateImage(value, previewSize);
            previewImage = image;

            //create objectField, when change value set sprite on Image
            ObjectField objectField = CreateObjectField(label, value, typeof(Sprite), x =>
            {
                onChangeValue?.Invoke(x);
                image.style.backgroundImage = new StyleBackground(x.newValue as Sprite);
            });

            return objectField;
        }

        public static Toggle CreateToggle(string label, bool value, EventCallback<ChangeEvent<bool>> onValueChanged = null)
        {
            Toggle toggle = new Toggle(label)
            {
                value = value
            };

            //register event if not null
            if (onValueChanged != null)
                toggle.RegisterValueChangedCallback(onValueChanged);

            return toggle;
        }

        public static IntegerField CreateIntegerField(string label, int value, EventCallback<ChangeEvent<int>> onValueChanged = null, int maxLength = 1000)
        {
            IntegerField integerField = new IntegerField(label, maxLength)
            {
                value = value
            };

            //register event if not null
            if (onValueChanged != null)
                integerField.RegisterValueChangedCallback(onValueChanged);

            return integerField;
        }

        public static FloatField CreateFloatField(string label, float value, EventCallback<ChangeEvent<float>> onValueChanged = null, int maxLength = 1000)
        {
            FloatField floatField = new FloatField(label, maxLength)
            {
                value = value
            };

            //register event if not null
            if (onValueChanged != null)
                floatField.RegisterValueChangedCallback(onValueChanged);

            return floatField;
        }

        public static VisualElement CreateSpace(Vector2 size)
        {
            VisualElement space = new VisualElement();

            //set size
            space.style.minWidth = size.x;
            space.style.maxWidth = size.x;
            space.style.minHeight = size.y;
            space.style.maxHeight = size.y;

            return space;
        }

        public static Image CreateImage(Sprite sprite, Vector2 size)
        {
            //create image with sprite
            return CreateImage(sprite, EImageType.Sprite, size);
        }

        public static Image CreateImage(Texture2D texture, Vector2 size)
        {
            //create image with texture
            return CreateImage(texture, EImageType.Texture2D, size);
        }

        public static Image CreateImage(VectorImage vectorImage, Vector2 size)
        {
            //create image with vector image
            return CreateImage(vectorImage, EImageType.VectorImage, size);
        }

        #region private API

        private static Image CreateImage(Object obj, EImageType imageType, Vector2 size)
        {
            //create image, but don't use Image.sprite and scaleMode, because we can't control it. It is always the size of the Sprite. Instead change Style
            Image image = new Image();

            //set size
            image.style.minWidth = size.x;
            image.style.maxWidth = size.x;
            image.style.minHeight = size.y;
            image.style.maxHeight = size.y;

            //set backgroundImage
            if (imageType == EImageType.Sprite)
            {
                Sprite sprite = obj as Sprite;
                image.style.backgroundImage = new StyleBackground(sprite);
            }
            else if (imageType == EImageType.Texture2D)
            {
                Texture2D texture = obj as Texture2D;
                image.style.backgroundImage = new StyleBackground(texture);
            }
            else if (imageType == EImageType.VectorImage)
            {
                VectorImage vectorImage = obj as VectorImage;
                image.style.backgroundImage = new StyleBackground(vectorImage);
            }
            else
            {
                Debug.LogError("EImageToUse is wrong");
            }

            return image;
        }

        #endregion
    }
}
#endif