#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace redd096.NodesGraph.Editor.Example
{
    /// <summary>
    /// Example of node with InputPorts, OutputPorts, Content and Foldout. 
    /// It has also an example of Add/Delete Output Port and change Port Name
    /// </summary>
    public class ExampleDynamicNode : GraphNode
    {
        public Dictionary<Port, string> DeletablePortsNames = new Dictionary<Port, string>();
        public string ContentText;

        protected override void DrawInputPorts()
        {
            //input port - if portName is null it uses the type passed as last parameter as name
            Port inputPort = CreateElementsUtilities.CreatePort(this, "Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputContainer.Add(inputPort);
        }

        protected override void DrawOutputPorts()
        {
            //output ports - here we create two ports: True and False
            Port truePort = CreateElementsUtilities.CreatePort(this, "True", Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            truePort.userData = true;       //value to send to input in another node
            Port falsePort = CreateElementsUtilities.CreatePort(this, "False", Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            falsePort.userData = false;     //value to send to input in another node
            outputContainer.Add(truePort);
            outputContainer.Add(falsePort);

            #region button to add deletable output ports

            //example of button to add new output port
            Button addPortButton = CreateElementsUtilities.CreateButton("Add Output", AddOutputPort);
            outputContainer.Add(addPortButton);

            //add style
            StyleSheetUtilities.AddToClassesList(addPortButton, "ds-node__button");

            #endregion
        }

        protected override void DrawContent()
        {
            //create container for custom style
            VisualElement customDataContainer = new VisualElement();

            //foldout and text field
            Foldout textFoldout = CreateElementsUtilities.CreateFoldout("Content");
            TextField contentTextField = CreateElementsUtilities.CreateTextArea(null, ContentText, callback => ContentText = callback.newValue);
            textFoldout.Add(contentTextField);                      //text inside foldout (so we can open and close it)
            customDataContainer.Add(textFoldout);                   //foldout inside custom container (so we can give it a custom style)
            extensionContainer.Add(customDataContainer);            //container inside extension container (so under input and output)

            //add styles
            StyleSheetUtilities.AddToClassesList(customDataContainer, "ds-node__custom-data-container");
            StyleSheetUtilities.AddToClassesList(contentTextField, "ds-node__textfield", "ds-node__quote-textfield");
        }

        #region add deletable output ports

        private void AddOutputPort()
        {
            //insert always before the Add button (outputContainer.childCount - 1)
            Port outputPort = CreateOutputPort();
            outputContainer.Insert(outputContainer.childCount - 1, outputPort);
        }

        private Port CreateOutputPort()
        {
            //output port without name, so we can use TextField
            Port outputPort = CreateElementsUtilities.CreatePort(this, "", Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Object));

            //create a TextField to change portName
            DeletablePortsNames.Add(outputPort, "Output");
            TextField portTextField = CreateElementsUtilities.CreateTextField(null, DeletablePortsNames[outputPort], callback => DeletablePortsNames[outputPort] = callback.newValue);

            //create a ObjectField to change data
            outputPort.userData = null;
            ObjectField portObjectField = CreateElementsUtilities.CreateObjectField("", null, typeof(Object), callback => outputPort.userData = callback.newValue);

            //create an X button to delete the port
            Button deletePortButton = CreateElementsUtilities.CreateButton("X", () => DeleteOutputPort(outputPort));

            //on output port, elements will be added from right to left, so to have the X button to the left we have to add it as the last element
            outputPort.Add(portTextField);
            outputPort.Add(portObjectField);
            outputPort.Add(deletePortButton);

            //add styles
            StyleSheetUtilities.AddToClassesList(portTextField, "ds-node__textfield", "ds-node__choice-textfield");
            StyleSheetUtilities.AddToClassesList(portObjectField, "ds-node__objectfield", "ds-node__objectfield__aligned");
            StyleSheetUtilities.AddToClassesList(deletePortButton, "ds-node__button");

            return outputPort;
        }

        private void DeleteOutputPort(Port outputPort)
        {
            //if connected, remove connections
            if (outputPort.connected)
                graph.DeleteElements(outputPort.connections);

            //remove port
            graph.RemoveElement(outputPort);
        }

        #endregion
    }
}
#endif