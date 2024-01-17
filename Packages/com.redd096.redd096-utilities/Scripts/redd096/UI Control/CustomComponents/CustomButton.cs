using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.UI;
using UnityEditor;
#endif

namespace redd096
{
    [AddComponentMenu("redd096/UI Control/Custom Components/Custom Button")]
    public class CustomButton : Button
    {
        [SerializeField] FTargetGraphics[] m_customTargetGraphics;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (base.gameObject.activeInHierarchy)
            {
                Color color;
                //Sprite newSprite;
                //string triggername;

                if (m_customTargetGraphics == null)
                    return;

                foreach (var v in m_customTargetGraphics)
                {
                    switch (state)
                    {
                        case SelectionState.Normal:
                            color = v.Colors.normalColor;
                            //newSprite = null;
                            //triggername = v.AnimationTriggers.normalTrigger;
                            break;
                        case SelectionState.Highlighted:
                            color = v.Colors.highlightedColor;
                            //newSprite = v.SpriteState.highlightedSprite;
                            //triggername = v.AnimationTriggers.highlightedTrigger;
                            break;
                        case SelectionState.Pressed:
                            color = v.Colors.pressedColor;
                            //newSprite = v.SpriteState.pressedSprite;
                            //triggername = v.AnimationTriggers.pressedTrigger;
                            break;
                        case SelectionState.Selected:
                            color = v.Colors.selectedColor;
                            //newSprite = v.SpriteState.selectedSprite;
                            //triggername = v.AnimationTriggers.selectedTrigger;
                            break;
                        case SelectionState.Disabled:
                            color = v.Colors.disabledColor;
                            //newSprite = v.SpriteState.disabledSprite;
                            //triggername = v.AnimationTriggers.disabledTrigger;
                            break;
                        default:
                            color = Color.black;
                            //newSprite = null;
                            //triggername = string.Empty;
                            break;
                    }

                    switch (v.Transition)
                    {
                        case Transition.ColorTint:
                            StartColorTween(v, color * v.Colors.colorMultiplier, instant);
                            break;
                        case Transition.SpriteSwap:
                            //DoSpriteSwap(v, newSprite);
                            break;
                        case Transition.Animation:
                            //TriggerAnimation(v, triggername);
                            break;
                    }
                }
            }
        }

        protected override void InstantClearState()
        {
            base.InstantClearState();

            if (m_customTargetGraphics == null)
                return;

            foreach (var v in m_customTargetGraphics)
            {
                //string normalTrigger = v.AnimationTriggers.normalTrigger;
                switch (v.Transition)
                {
                    case Transition.ColorTint:
                        StartColorTween(v, Color.white, instant: true);
                        break;
                    case Transition.SpriteSwap:
                        //DoSpriteSwap(v, null);
                        break;
                    case Transition.Animation:
                        //TriggerAnimation(v, normalTrigger);
                        break;
                }
            }
        }

        private void StartColorTween(FTargetGraphics target, Color targetColor, bool instant)
        {
            if (!(target.TargetGraphic == null))
            {
                target.TargetGraphic.CrossFadeColor(targetColor, instant ? 0f : target.Colors.fadeDuration, ignoreTimeScale: true, useAlpha: true);
            }
        }

        //private void DoSpriteSwap(FTargetGraphics target, Sprite newSprite)
        //{
        //    if (!((target.TargetGraphic as Image) == null))
        //    {
        //        (target.TargetGraphic as Image).overrideSprite = newSprite;
        //    }
        //}

        //private void TriggerAnimation(FTargetGraphics target, string triggername)
        //{
        //    if (target.Transition == Transition.Animation && !(animator == null) && animator.isActiveAndEnabled && animator.hasBoundPlayables && !string.IsNullOrEmpty(triggername))
        //    {
        //        animator.ResetTrigger(target.AnimationTriggers.normalTrigger);
        //        animator.ResetTrigger(target.AnimationTriggers.highlightedTrigger);
        //        animator.ResetTrigger(target.AnimationTriggers.pressedTrigger);
        //        animator.ResetTrigger(target.AnimationTriggers.selectedTrigger);
        //        animator.ResetTrigger(target.AnimationTriggers.disabledTrigger);
        //        animator.SetTrigger(triggername);
        //    }
        //}

        [System.Serializable]
        public struct FTargetGraphics
        {
            public Graphic TargetGraphic;
            public Transition Transition;
            public ColorBlock Colors;
            //public SpriteState SpriteState;
            //public AnimationTriggers AnimationTriggers;
        }
    }

    #region editor

#if UNITY_EDITOR

    [CustomEditor(typeof(CustomButton), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the CustomButton Component.
    ///   Extend this class to write a custom editor for a component derived from CustomButton.
    /// </summary>
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

#endif

    #endregion
}