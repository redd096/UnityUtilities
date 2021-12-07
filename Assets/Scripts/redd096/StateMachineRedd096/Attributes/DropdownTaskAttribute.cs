using UnityEngine;

namespace redd096
{
    #region editor property drawer
#if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(DropdownTaskAttribute))]
    public class DropdownTaskPropertyDrawer : PropertyDrawer
    {
        BaseTask[] possibleTasks;
        string[] tasksNames;

        const float heightRef = 20;             //height for readonly reference
        const float spaceBetweemElements = 5;   //space between elements in array

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //add height to show readonly ref (if array, add height for every element + a space between elements)
            return base.GetPropertyHeight(property, label) + (property.isArray ? property.arraySize * (heightRef + spaceBetweemElements) : heightRef + spaceBetweemElements);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            //get tasks
            GetTasks(property);

            //show every array element
            if (property.isArray)
            {
                //move down every element to create a column + set height removing space between elements of the array
                float spaceBetween = rect.height / property.arraySize;
                float heightSingleElement = rect.height / property.arraySize - (spaceBetweemElements * property.arraySize);

                for (int i = 0; i < property.arraySize; i++)
                {
                    SetProperty(new Rect(rect.x, rect.y + spaceBetween * i, rect.width, heightSingleElement), property.GetArrayElementAtIndex(i));
                }
            }
            //else show only a single element (if not array)
            else
            {
                SetProperty(new Rect(rect.x, rect.y, rect.width, rect.height - spaceBetweemElements), property);
            }

            EditorGUI.EndProperty();
        }

        void SetProperty(Rect rect, SerializedProperty property)
        {
            //show dropdown to select and set property
            if (possibleTasks != null && possibleTasks.Length > 0)
            {
                //get current index
                int selectedIndex = GetSelectedIndex(property);

                selectedIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, rect.height - heightRef), selectedIndex, tasksNames);      //remove height, because will be used for readonly ref
                property.objectReferenceValue = possibleTasks[selectedIndex];
            }
            //else show NONE and don't set anything
            else
            {
                EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, rect.height - heightRef), 0, new string[1] { "NONE" });                    //remove height, because will be used for readonly ref
            }

            //show readonly reference
            GUI.enabled = false;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + rect.height - heightRef, rect.width, heightRef), property);                       //move bottom (-height) and set height for readonly ref
            GUI.enabled = true;
        }

        void GetTasks(SerializedProperty property)
        {
            //get owner of the property
            Component owner = property.serializedObject.targetObject as Component;
            if(owner)
            {
                //get tasks in children
                Component[] components = owner.GetComponentsInChildren(((DropdownTaskAttribute)attribute).TaskType);

                //set arrays for dropdown
                if (components != null && components.Length > 0)
                {
                    possibleTasks = new BaseTask[components.Length +1];
                    tasksNames = new string[possibleTasks.Length +1];

                    //set NONE as first task
                    possibleTasks[0] = null;
                    tasksNames[0] = "NONE";

                    for (int i = 0; i < components.Length; i++)
                    {
                        possibleTasks[i +1] = components[i] as BaseTask;
                        tasksNames[i +1] = possibleTasks[i +1].TaskName;
                    }

                    return;
                }
            }

            //if there are no tasks, set NONE as unique name
            possibleTasks = new BaseTask[1] { null };
            tasksNames = new string[1] { "NONE" };
        }

        int GetSelectedIndex(SerializedProperty property)
        {
            //return current index
            for(int i = 0; i < possibleTasks.Length; i++)
            {
                if (possibleTasks[i] == property.objectReferenceValue)
                    return i;
            }

            return 0;
        }
    }

#endif
    #endregion

    //attribute
    public class DropdownTaskAttribute : PropertyAttribute
    {
        public readonly System.Type TaskType;

        /// <summary>
        /// Attribute to draw a dropdown for a BaseTask
        /// </summary>
        /// <param name="TaskType">Type of task (ActionTask or ConditionTask)</param>
        public DropdownTaskAttribute(System.Type TaskType)
        {
            this.TaskType = TaskType;
        }
    }
}