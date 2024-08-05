using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.UI;
using UnityEditor;
#endif

namespace redd096.UIControl
{
    [AddComponentMenu("redd096/UIControl/Custom Components/Custom Button")]
    public class CustomButton : Button
    {
        [SerializeField] FCustomTargetGraphics[] m_customTargetGraphics;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            //base check
            if (!gameObject.activeInHierarchy)
                return;

            if (m_customTargetGraphics == null)
                return;

            //copy-paste base.DoStateTransition but for every element in array
            Color tintColor;
            Sprite transitionSprite;
            string triggername;

            foreach (var v in m_customTargetGraphics)
            {
                switch (state)
                {
                    case SelectionState.Normal:
                        tintColor = v.Colors.normalColor;
                        transitionSprite = null;
                        triggername = v.AnimationTriggers.normalTrigger;
                        break;
                    case SelectionState.Highlighted:
                        tintColor = v.Colors.highlightedColor;
                        transitionSprite = v.SpriteState.highlightedSprite;
                        triggername = v.AnimationTriggers.highlightedTrigger;
                        break;
                    case SelectionState.Pressed:
                        tintColor = v.Colors.pressedColor;
                        transitionSprite = v.SpriteState.pressedSprite;
                        triggername = v.AnimationTriggers.pressedTrigger;
                        break;
                    case SelectionState.Selected:
                        tintColor = v.Colors.selectedColor;
                        transitionSprite = v.SpriteState.selectedSprite;
                        triggername = v.AnimationTriggers.selectedTrigger;
                        break;
                    case SelectionState.Disabled:
                        tintColor = v.Colors.disabledColor;
                        transitionSprite = v.SpriteState.disabledSprite;
                        triggername = v.AnimationTriggers.disabledTrigger;
                        break;
                    default:
                        tintColor = Color.black;
                        transitionSprite = null;
                        triggername = string.Empty;
                        break;
                }

                switch (v.Transition)
                {
                    case Transition.ColorTint:
                        StartColorTween(v, tintColor * v.Colors.colorMultiplier, instant);
                        break;
                    case Transition.SpriteSwap:
                        DoSpriteSwap(v, transitionSprite);
                        break;
                    case Transition.Animation:
                        TriggerAnimation(v, triggername);
                        break;
                }
            }
        }

        protected override void InstantClearState()
        {
            base.InstantClearState();

            if (m_customTargetGraphics == null)
                return;

            //copy-paste base.InstantClearState but for every element in array
            foreach (var v in m_customTargetGraphics)
            {
                string triggerName = v.AnimationTriggers.normalTrigger;

                switch (v.Transition)
                {
                    case Transition.ColorTint:
                        StartColorTween(v, Color.white, instant: true);
                        break;
                    case Transition.SpriteSwap:
                        DoSpriteSwap(v, null);
                        break;
                    case Transition.Animation:
                        TriggerAnimation(v, triggerName);
                        break;
                }
            }
        }

        private void StartColorTween(FCustomTargetGraphics target, Color targetColor, bool instant)
        {
            if (target.TargetGraphic == null)
                return;

            target.TargetGraphic.CrossFadeColor(targetColor, instant ? 0f : target.Colors.fadeDuration, ignoreTimeScale: true, useAlpha: true);
        }

        private void DoSpriteSwap(FCustomTargetGraphics target, Sprite newSprite)
        {
            if (target.TargetGraphic as Image == null)
                return;

            (target.TargetGraphic as Image).overrideSprite = newSprite;
        }

        private void TriggerAnimation(FCustomTargetGraphics target, string triggername)
        {
#if PACKAGE_ANIMATION
            if (target.Transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(target.AnimationTriggers.normalTrigger);
            animator.ResetTrigger(target.AnimationTriggers.highlightedTrigger);
            animator.ResetTrigger(target.AnimationTriggers.pressedTrigger);
            animator.ResetTrigger(target.AnimationTriggers.selectedTrigger);
            animator.ResetTrigger(target.AnimationTriggers.disabledTrigger);

            animator.SetTrigger(triggername);
#endif
        }

        [System.Serializable]
        public struct FCustomTargetGraphics
        {
            public Graphic TargetGraphic;
            public Transition Transition;
            public ColorBlock Colors;
            public SpriteState SpriteState;
            public AnimationTriggers AnimationTriggers;
        }
    }

    #region editor

#if UNITY_EDITOR

    /// <summary>
    ///   Custom Editor for the CustomButton Component.
    ///   Extend this class to write a custom editor for a component derived from CustomButton.
    /// </summary>
    [CustomEditor(typeof(CustomButton), true)]
    [CanEditMultipleObjects]
    public class CustomButtonEditor : ButtonEditor
    {
        SerializedProperty m_customTargetGraphicsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_customTargetGraphicsProperty = serializedObject.FindProperty("m_customTargetGraphics");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_customTargetGraphicsProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// Custom Editor for the Custom Target Graphics
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomButton.FCustomTargetGraphics), true)]
    public class CustomButtonTargetGraphicsEditor : PropertyDrawer
    {
        SerializedProperty TargetGraphic;
        SerializedProperty Transition;
        SerializedProperty PropertyToDraw;  //Colors, SpriteState or AnimationTriggers

        Selectable.Transition transition;

        float heightTargetGraphic;
        float heightTransition;
        float heightPropertyToDraw;

        Rect rectTargetGraphic;
        Rect rectTransition;
        Rect rectPropertyToDraw;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return base.GetPropertyHeight(property, label);

            UpdateValues(property);
            return heightTargetGraphic + heightTransition + heightPropertyToDraw;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);

            UpdateValues(property);

            //calculate rects
            rectTargetGraphic = new Rect(position.x, position.y, position.width, heightTargetGraphic);
            rectTransition = new Rect(position.x, rectTargetGraphic.y + rectTargetGraphic.height, position.width, heightTransition);
            rectPropertyToDraw = PropertyToDraw != null ? new Rect(position.x, rectTransition.y + rectTransition.height, position.width, heightPropertyToDraw) : Rect.zero;

            //draw properties
            EditorGUI.PropertyField(rectTargetGraphic, TargetGraphic);
            EditorGUI.PropertyField(rectTransition, Transition);
            if (PropertyToDraw != null) EditorGUI.PropertyField(rectPropertyToDraw, PropertyToDraw);

        }

        void UpdateValues(SerializedProperty property)
        {
            TargetGraphic = property.FindPropertyRelative("TargetGraphic");
            Transition = property.FindPropertyRelative("Transition");

            //get transition value
            transition = (Selectable.Transition)Transition.enumValueIndex;

            if (transition == Selectable.Transition.ColorTint)
                PropertyToDraw = property.FindPropertyRelative("Colors");
            else if (transition == Selectable.Transition.SpriteSwap)
                PropertyToDraw = property.FindPropertyRelative("SpriteState");
            else if (transition == Selectable.Transition.Animation)
                PropertyToDraw = property.FindPropertyRelative("AnimationTriggers");
            else
                PropertyToDraw = null;

            //calculate heights
            heightTargetGraphic = EditorGUI.GetPropertyHeight(TargetGraphic, true);
            heightTransition = EditorGUI.GetPropertyHeight(Transition, true);
            heightPropertyToDraw = PropertyToDraw != null ? EditorGUI.GetPropertyHeight(PropertyToDraw, true) : 0f;
        }
    }

#endif

    #endregion
}