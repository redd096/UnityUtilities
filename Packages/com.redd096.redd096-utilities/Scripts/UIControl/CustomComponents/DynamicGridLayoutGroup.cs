using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.UIControl
{
    [AddComponentMenu("redd096/UIControl/Custom Components/Dynamic Grid Layout Group")]
    public class DynamicGridLayoutGroup : LayoutGroup
    {
        #region GridLayoutGroup variables

        //TODO edit SetChildrenAlongAxis to use startCorner, like it does in GridLayoutGroup.SetCellsAlongAxis
        //[SerializeField] protected Corner m_StartCorner = Corner.UpperLeft;

        ///// <summary>
        ///// Which corner should the first cell be placed in?
        ///// </summary>
        //public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }

        [SerializeField] protected Axis m_StartAxis = Axis.Horizontal;

        /// <summary>
        /// Which axis should cells be placed along first
        /// </summary>
        /// <remarks>
        /// When startAxis is set to horizontal, an entire row will be filled out before proceeding to the next row. When set to vertical, an entire column will be filled out before proceeding to the next column.
        /// </remarks>
        public Axis startAxis { get { return m_StartAxis; } set { SetProperty(ref m_StartAxis, value); } }

        [SerializeField] protected Vector2 m_Spacing = Vector2.zero;

        /// <summary>
        /// The spacing to use between layout elements in the grid on both axises.
        /// </summary>
        public Vector2 spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

        #endregion

        #region HorizontalOrVerticalLayoutGroup variables

        [SerializeField] protected bool m_ChildForceExpandWidth = true;

        /// <summary>
        /// Whether to force the children to expand to fill additional available horizontal space.
        /// </summary>
        public bool childForceExpandWidth { get { return m_ChildForceExpandWidth; } set { SetProperty(ref m_ChildForceExpandWidth, value); } }

        [SerializeField] protected bool m_ChildForceExpandHeight = true;

        /// <summary>
        /// Whether to force the children to expand to fill additional available vertical space.
        /// </summary>
        public bool childForceExpandHeight { get { return m_ChildForceExpandHeight; } set { SetProperty(ref m_ChildForceExpandHeight, value); } }

        [SerializeField] protected bool m_ChildControlWidth = true;

        /// <summary>
        /// Returns true if the Layout Group controls the widths of its children. Returns false if children control their own widths.
        /// </summary>
        /// <remarks>
        /// If set to false, the layout group will only affect the positions of the children while leaving the widths untouched. The widths of the children can be set via the respective RectTransforms in this case.
        ///
        /// If set to true, the widths of the children are automatically driven by the layout group according to their respective minimum, preferred, and flexible widths. This is useful if the widths of the children should change depending on how much space is available.In this case the width of each child cannot be set manually in the RectTransform, but the minimum, preferred and flexible width for each child can be controlled by adding a LayoutElement component to it.
        /// </remarks>
        public bool childControlWidth { get { return m_ChildControlWidth; } set { SetProperty(ref m_ChildControlWidth, value); } }

        [SerializeField] protected bool m_ChildControlHeight = true;

        /// <summary>
        /// Returns true if the Layout Group controls the heights of its children. Returns false if children control their own heights.
        /// </summary>
        /// <remarks>
        /// If set to false, the layout group will only affect the positions of the children while leaving the heights untouched. The heights of the children can be set via the respective RectTransforms in this case.
        ///
        /// If set to true, the heights of the children are automatically driven by the layout group according to their respective minimum, preferred, and flexible heights. This is useful if the heights of the children should change depending on how much space is available.In this case the height of each child cannot be set manually in the RectTransform, but the minimum, preferred and flexible height for each child can be controlled by adding a LayoutElement component to it.
        /// </remarks>
        public bool childControlHeight { get { return m_ChildControlHeight; } set { SetProperty(ref m_ChildControlHeight, value); } }

        [SerializeField] protected bool m_ChildScaleWidth = false;

        /// <summary>
        /// Whether to use the x scale of each child when calculating its width.
        /// </summary>
        public bool childScaleWidth { get { return m_ChildScaleWidth; } set { SetProperty(ref m_ChildScaleWidth, value); } }

        [SerializeField] protected bool m_ChildScaleHeight = false;

        /// <summary>
        /// Whether to use the y scale of each child when calculating its height.
        /// </summary>
        public bool childScaleHeight { get { return m_ChildScaleHeight; } set { SetProperty(ref m_ChildScaleHeight, value); } }

        [SerializeField] protected bool m_ReverseArrangement = false;

        /// <summary>
        /// Whether the order of children objects should be sorted in reverse.
        /// </summary>
        /// <remarks>
        /// If False the first child object will be positioned first.
        /// If True the last child object will be positioned first.
        /// </remarks>
        public bool reverseArrangement { get { return m_ReverseArrangement; } set { SetProperty(ref m_ReverseArrangement, value); } }

        #endregion

        #region DynamicGridLayout variables

        /// <summary>
        /// Create a new row or column, when this value reach the edge of the container
        /// </summary>
        public enum WrappingMode
        {
            /// <summary>
            /// Using Min size of every child
            /// </summary>
            MinSize = 0,
            /// <summary>
            /// Using Preferred size of every child
            /// </summary>
            PreferredSize = 1
        }

        [SerializeField] protected WrappingMode m_WrappingMode = WrappingMode.MinSize;

        /// <summary>
        /// Whether create a new row or column, using this childrens size
        /// </summary>
        public WrappingMode wrappingMode { get { return m_WrappingMode; } set { SetProperty(ref m_WrappingMode, value); } }

        protected int m_LineCount = 0;
        protected List<int> m_CellsPerLine = new List<int>();
        protected List<Vector2> m_LineTotalMinSize = new List<Vector2>();
        protected List<Vector2> m_LineTotalPreferredSize = new List<Vector2>();
        protected List<Vector2> m_LineTotalFlexibleSize = new List<Vector2>();

        #endregion

        protected DynamicGridLayoutGroup()
        { }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            int startIndex = m_ReverseArrangement ? m_LineCount - 1 : 0;
            int endIndex = m_ReverseArrangement ? 0 : m_LineCount;
            int increment = m_ReverseArrangement ? -1 : 1;

            int startChild = m_ReverseArrangement ? rectChildren.Count - 1 : 0;
            int fromChild = 0;
            int toChild = 0;
            for (int i = startIndex; m_ReverseArrangement ? i >= endIndex : i < endIndex; i += increment)
            {
                fromChild = i == startIndex ? startChild : toChild;
                toChild = m_ReverseArrangement ? fromChild - m_CellsPerLine[i] : fromChild + m_CellsPerLine[i];

                SetChildrenAlongAxis(0, fromChild, toChild, i);
            }
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutVertical()
        {
            int startIndex = m_ReverseArrangement ? m_LineCount - 1 : 0;
            int endIndex = m_ReverseArrangement ? 0 : m_LineCount;
            int increment = m_ReverseArrangement ? -1 : 1;

            int startChild = m_ReverseArrangement ? rectChildren.Count - 1 : 0;
            int fromChild = 0;
            int toChild = 0;
            for (int i = startIndex; m_ReverseArrangement ? i >= endIndex : i < endIndex; i += increment)
            {
                fromChild = i == startIndex ? startChild : toChild;
                toChild = m_ReverseArrangement ? fromChild - m_CellsPerLine[i] : fromChild + m_CellsPerLine[i];

                SetChildrenAlongAxis(1, fromChild, toChild, i);
            }
        }

        #region main functions

        /// <summary>
        /// Calculate the layout element properties for this layout element along the given axis. (copied from HorizontalOrVerticalLayoutGroup)
        /// </summary>
        /// <param name="axis">The axis to calculate for. 0 is horizontal and 1 is vertical.</param>
        protected void CalcAlongAxis(int axis)
        {
            bool startHorizontal = startAxis == Axis.Horizontal;

            //horizontal
            float horizontalCombinedPadding = padding.horizontal;
            bool horizontalControlSize = m_ChildControlWidth;
            bool horizontalUseScale = m_ChildScaleWidth;
            bool horizontalChildForceExpandSize = m_ChildForceExpandWidth;
            float horizontalSpacing = m_Spacing.x;

            //vertical
            float verticalCombinedPadding = padding.vertical;
            bool verticalControlSize = m_ChildControlHeight;
            bool verticalUseScale = m_ChildScaleHeight;
            bool verticalChildForceExpandSize = m_ChildForceExpandHeight;
            float verticalSpacing = m_Spacing.y;

            //we have to save the total for the current line (row or column), so we know which one is the bigger and when create a new line
            float limit = (startHorizontal ? rectTransform.rect.width : rectTransform.rect.height);
            m_LineCount = 1;
            m_CellsPerLine.Clear();
            m_CellsPerLine.Add(0);
            m_LineTotalMinSize.Clear();
            m_LineTotalPreferredSize.Clear();
            m_LineTotalFlexibleSize.Clear();
            float horizontalLineTotalMin = horizontalCombinedPadding;
            float horizontalLineTotalPreferred = horizontalCombinedPadding;
            float horizontalLineTotalFlexible = 0;
            float verticalLineTotalMin = verticalCombinedPadding;
            float verticalLineTotalPreferred = verticalCombinedPadding;
            float verticalLineTotalFlexible = 0;

            //we have to use reverseArrangement also here, otherwise columns and rows could be different
            int startIndex = m_ReverseArrangement ? rectChildren.Count - 1 : 0;
            int endIndex = m_ReverseArrangement ? 0 : rectChildren.Count;
            int increment = m_ReverseArrangement ? -1 : 1;

            for (int i = startIndex; m_ReverseArrangement ? i >= endIndex : i < endIndex; i += increment)
            {
                RectTransform child = rectChildren[i];
                float horizontalMin, horizontalPreferred, horizontalFlexible;
                GetChildSizes(child, 0, horizontalControlSize, horizontalChildForceExpandSize, out horizontalMin, out horizontalPreferred, out horizontalFlexible);
                float verticalMin, verticalPreferred, verticalFlexible;
                GetChildSizes(child, 1, verticalControlSize, verticalChildForceExpandSize, out verticalMin, out verticalPreferred, out verticalFlexible);

                if (horizontalUseScale)
                {
                    float scaleFactor = child.localScale[0];
                    horizontalMin *= scaleFactor;
                    horizontalPreferred *= scaleFactor;
                    horizontalFlexible *= scaleFactor;
                }
                if (verticalUseScale)
                {
                    float scaleFactor = child.localScale[1];
                    verticalMin *= scaleFactor;
                    verticalPreferred *= scaleFactor;
                    verticalFlexible *= scaleFactor;
                }

                //check if reach limit and create another row/column
                float checkLineTotalMin = (startHorizontal ? horizontalLineTotalMin + horizontalMin : verticalLineTotalMin + verticalMin);
                float checkLineTotalPreferred = (startHorizontal ? horizontalLineTotalPreferred + horizontalPreferred : verticalLineTotalPreferred + verticalPreferred);
                if ((wrappingMode == WrappingMode.MinSize && checkLineTotalMin > limit) || (wrappingMode == WrappingMode.PreferredSize && checkLineTotalPreferred > limit))
                {
                    //only if this isn't the first element in the line (this to have at least one element in every line)
                    if (m_CellsPerLine[m_LineCount - 1] > 0)
                    {
                        SaveLine(ref horizontalLineTotalMin, ref horizontalLineTotalPreferred, ref horizontalLineTotalFlexible, horizontalSpacing,
                            ref verticalLineTotalMin, ref verticalLineTotalPreferred, ref verticalLineTotalFlexible, verticalSpacing);

                        //reset line vars
                        horizontalLineTotalMin = horizontalCombinedPadding;
                        horizontalLineTotalPreferred = horizontalCombinedPadding;
                        horizontalLineTotalFlexible = 0;
                        verticalLineTotalMin = verticalCombinedPadding;
                        verticalLineTotalPreferred = verticalCombinedPadding;
                        verticalLineTotalFlexible = 0;

                        //create new line
                        m_LineCount++;
                        m_CellsPerLine.Add(0);
                    }
                }

                m_CellsPerLine[m_LineCount - 1]++;

                //move along horizontal line but save the greater element in vertical size
                if (startHorizontal)
                {
                    horizontalLineTotalMin += horizontalMin + horizontalSpacing;
                    horizontalLineTotalPreferred += horizontalPreferred + horizontalSpacing;
                    horizontalLineTotalFlexible += horizontalFlexible;  // Increment flexible size with element's flexible size.
                    if (verticalLineTotalMin < verticalMin) verticalLineTotalMin = verticalMin;
                    if (verticalLineTotalPreferred < verticalPreferred) verticalLineTotalPreferred = verticalPreferred;
                    if (verticalLineTotalFlexible < verticalFlexible) verticalLineTotalFlexible = verticalFlexible;
                }
                //and viceversa
                else
                {
                    verticalLineTotalMin += verticalMin + verticalSpacing;
                    verticalLineTotalPreferred += verticalPreferred + verticalSpacing;
                    verticalLineTotalFlexible += verticalFlexible;      // Increment flexible size with element's flexible size.
                    if (horizontalLineTotalMin < horizontalMin) horizontalLineTotalMin = horizontalMin;
                    if (horizontalLineTotalPreferred < horizontalPreferred) horizontalLineTotalPreferred = horizontalPreferred;
                    if (horizontalLineTotalFlexible < horizontalFlexible) horizontalLineTotalFlexible = horizontalFlexible;
                }
            }

            //save last line
            SaveLine(ref horizontalLineTotalMin, ref horizontalLineTotalPreferred, ref horizontalLineTotalFlexible, horizontalSpacing,
                ref verticalLineTotalMin, ref verticalLineTotalPreferred, ref verticalLineTotalFlexible, verticalSpacing);

            //set greater min, preferred and flexible
            float totalMin, totalPreferred, totalFlexible;
            GetTotalSizes(axis, axis == 0 ? horizontalSpacing : verticalSpacing, out totalMin, out totalPreferred, out totalFlexible);
            SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, axis);
        }

        /// <summary>
        /// Set the positions and sizes of the child layout elements for the given axis. (copied from HorizontalOrVerticalLayoutGroup and GridLayoutGroup.SetCellsAlongAxis())
        /// </summary>
        /// <param name="axis">The axis to handle. 0 is horizontal and 1 is vertical.</param>
        protected void SetChildrenAlongAxis(int axis, int startIndex, int endIndex, int currentLine)
        {
            //horizontal
            float horizontalSize = rectTransform.rect.size[0];
            bool horizontalControlSize = m_ChildControlWidth;
            bool horizontalUseScale = m_ChildScaleWidth;
            bool horizontalChildForceExpandSize = m_ChildForceExpandWidth;
            float horizontalAlignmentOnAxis = GetAlignmentOnAxis(0);
            float horizontalSpacing = m_Spacing.x;

            //vertical
            float verticalSize = rectTransform.rect.size[1];
            bool verticalControlSize = m_ChildControlHeight;
            bool verticalUseScale = m_ChildScaleHeight;
            bool verticalChildForceExpandSize = m_ChildForceExpandHeight;
            float verticalAlignmentOnAxis = GetAlignmentOnAxis(1);
            float verticalSpacing = m_Spacing.y;

            int increment = m_ReverseArrangement ? -1 : 1;

            // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
            // and only vertical values when invoked for the vertical axis.
            // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
            // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
            // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.
            if (axis == 0)
            {
                // Only set the sizes when invoked for horizontal axis, not the positions.

                float horizontalInnerSize = horizontalSize - padding.horizontal;
                float verticalInnerSize = verticalSize - padding.vertical;

                for (int i = startIndex; m_ReverseArrangement ? i > endIndex : i < endIndex; i += increment)
                {
                    RectTransform child = rectChildren[i];

                    //horizontal
                    float horizontalMin, horizontalPreferred, horizontalFlexible;
                    GetChildSizes(child, 0, horizontalControlSize, horizontalChildForceExpandSize, out horizontalMin, out horizontalPreferred, out horizontalFlexible);
                    float horizontalScaleFactor = horizontalUseScale ? child.localScale[0] : 1f;

                    float horizontalRequiredSpace = Mathf.Clamp(horizontalInnerSize, horizontalMin, horizontalFlexible > 0 ? horizontalSize : horizontalPreferred);
                    float horizontalStartOffset = GetStartOffset(0, horizontalRequiredSpace * horizontalScaleFactor);

                    //vertical
                    float verticalMin, verticalPreferred, verticalFlexible;
                    GetChildSizes(child, 1, verticalControlSize, verticalChildForceExpandSize, out verticalMin, out verticalPreferred, out verticalFlexible);
                    float verticalScaleFactor = verticalUseScale ? child.localScale[1] : 1f;

                    float verticalRequiredSpace = Mathf.Clamp(verticalInnerSize, verticalMin, verticalFlexible > 0 ? verticalSize : verticalPreferred);
                    float verticalStartOffset = GetStartOffset(1, verticalRequiredSpace * verticalScaleFactor);

                    //calculate new size
                    SetChildWithScale(child, horizontalControlSize, horizontalStartOffset, horizontalRequiredSpace, horizontalScaleFactor, horizontalAlignmentOnAxis,
                        verticalControlSize, verticalStartOffset, verticalRequiredSpace, verticalScaleFactor, verticalAlignmentOnAxis);
                }
                return;
            }

            //set positions for vertical axis

            bool startHorizontal = startAxis == Axis.Horizontal;

            //horizontal
            float horizontalPos = (startHorizontal ? padding.left : GetPositionAtLine(0, currentLine, padding.left, horizontalSpacing));
            float horizontalItemFlexibleMultiplier = 0;
            float horizontalSurplusSpace = horizontalSize - GetTotalPreferredSize(currentLine, 0);

            if (horizontalSurplusSpace > 0)
            {
                if (GetTotalFlexibleSize(currentLine, 0) == 0 && startHorizontal)
                    horizontalPos = GetStartOffset(0, GetTotalPreferredSize(currentLine, 0) - padding.horizontal);
                else if (GetTotalFlexibleSize(currentLine, 0) > 0)
                    horizontalItemFlexibleMultiplier = horizontalSurplusSpace / GetTotalFlexibleSize(currentLine, 0);
            }

            float horizontalMinMaxLerp = 0;
            if (GetTotalMinSize(currentLine, 0) != GetTotalPreferredSize(currentLine, 0))
                horizontalMinMaxLerp = Mathf.Clamp01((horizontalSize - GetTotalMinSize(currentLine, 0)) / (GetTotalPreferredSize(currentLine, 0) - GetTotalMinSize(currentLine, 0)));

            //vertical
            float verticalPos = (startHorizontal == false ? padding.top : GetPositionAtLine(1, currentLine, padding.top, verticalSpacing));
            float verticalItemFlexibleMultiplier = 0;
            float verticalSurplusSpace = verticalSize - GetTotalPreferredSize(currentLine, 1);

            if (verticalSurplusSpace > 0)
            {
                if (GetTotalFlexibleSize(currentLine, 1) == 0 && startHorizontal == false)
                    verticalPos = GetStartOffset(0, GetTotalPreferredSize(currentLine, 1) - padding.vertical);
                else if (GetTotalFlexibleSize(currentLine, 1) > 0)
                    verticalItemFlexibleMultiplier = verticalSurplusSpace / GetTotalFlexibleSize(currentLine, 1);
            }

            float verticalMinMaxLerp = 0;
            if (GetTotalMinSize(currentLine, 1) != GetTotalPreferredSize(currentLine, 1))
                verticalMinMaxLerp = Mathf.Clamp01((verticalSize - GetTotalMinSize(currentLine, 1)) / (GetTotalPreferredSize(currentLine, 1) - GetTotalMinSize(currentLine, 1)));

            //cycle childs
            for (int i = startIndex; m_ReverseArrangement ? i > endIndex : i < endIndex; i += increment)
            {
                RectTransform child = rectChildren[i];

                //horizontal
                float horizontalMin, horizontalPreferred, horizontalFlexible;
                GetChildSizes(child, 0, horizontalControlSize, horizontalChildForceExpandSize, out horizontalMin, out horizontalPreferred, out horizontalFlexible);
                float horizontalScaleFactor = horizontalUseScale ? child.localScale[0] : 1f;
                float horizontalChildSize = Mathf.Lerp(horizontalMin, horizontalPreferred, horizontalMinMaxLerp);
                horizontalChildSize += horizontalFlexible * horizontalItemFlexibleMultiplier;

                //vertical
                float verticalMin, verticalPreferred, verticalFlexible;
                GetChildSizes(child, 1, verticalControlSize, verticalChildForceExpandSize, out verticalMin, out verticalPreferred, out verticalFlexible);
                float verticalScaleFactor = verticalUseScale ? child.localScale[1] : 1f;
                float verticalChildSize = Mathf.Lerp(verticalMin, verticalPreferred, verticalMinMaxLerp);
                verticalChildSize += verticalFlexible * verticalItemFlexibleMultiplier;

                //calculate new size
                SetChildWithScale(child, horizontalControlSize, horizontalPos, horizontalChildSize, horizontalScaleFactor, horizontalAlignmentOnAxis,
                    verticalControlSize, verticalPos, verticalChildSize, verticalScaleFactor, verticalAlignmentOnAxis);

                //increase position
                if (startHorizontal)
                    horizontalPos += horizontalChildSize * horizontalScaleFactor + horizontalSpacing;
                else
                    verticalPos += verticalChildSize * verticalScaleFactor + verticalSpacing;
            }
        }

        #endregion

        #region custom functions

        protected void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand,
            out float min, out float preferred, out float flexible)
        {
            //copied from HorizontalOrVerticalLayoutGroup because is private
            if (!controlSize)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }

            if (childForceExpand)
                flexible = Mathf.Max(flexible, 1);
        }

        protected void SaveLine(ref float horizontalLineTotalMin, ref float horizontalLineTotalPreferred, ref float horizontalLineTotalFlexible, float horizontalSpacing,
            ref float verticalLineTotalMin, ref float verticalLineTotalPreferred, ref float verticalLineTotalFlexible, float verticalSpacing)
        {
            if (m_CellsPerLine[m_LineCount - 1] > 0)
            {
                //remove spacing for next element
                if (startAxis == Axis.Horizontal)
                {
                    horizontalLineTotalMin -= horizontalSpacing;
                    horizontalLineTotalPreferred -= horizontalSpacing;
                }
                else
                {
                    verticalLineTotalMin -= verticalSpacing;
                    verticalLineTotalPreferred -= verticalSpacing;
                }
                horizontalLineTotalPreferred = Mathf.Max(horizontalLineTotalMin, horizontalLineTotalPreferred);
                verticalLineTotalPreferred = Mathf.Max(verticalLineTotalMin, verticalLineTotalPreferred);
            }

            //save the line
            m_LineTotalMinSize.Add(new Vector2(horizontalLineTotalMin, verticalLineTotalMin));
            m_LineTotalPreferredSize.Add(new Vector2(horizontalLineTotalPreferred, verticalLineTotalPreferred));
            m_LineTotalFlexibleSize.Add(new Vector2(horizontalLineTotalFlexible, verticalLineTotalFlexible));
        }

        protected void GetTotalSizes(int axis, float lineSpacing, out float minSize, out float preferredSize, out float flexibleSize)
        {
            minSize = 0;
            preferredSize = 0;
            flexibleSize = 0;

            for (int i = 0; i < m_LineCount; i++)
            {
                //if we are using this axis, we already reach the edge of the container. Just take the bigger between every line
                if (axis == (int)startAxis)
                {
                    if (minSize < m_LineTotalMinSize[i][axis]) minSize = m_LineTotalMinSize[i][axis];
                    if (preferredSize < m_LineTotalPreferredSize[i][axis]) preferredSize = m_LineTotalPreferredSize[i][axis];
                    if (flexibleSize < m_LineTotalFlexibleSize[i][axis]) flexibleSize = m_LineTotalFlexibleSize[i][axis];
                }
                //else, sum every line
                else
                {
                    minSize += m_LineTotalMinSize[i][axis] + lineSpacing;
                    preferredSize += m_LineTotalPreferredSize[i][axis] + lineSpacing;
                    flexibleSize += m_LineTotalFlexibleSize[i][axis];
                }
            }
        }

        protected float GetPositionAtLine(int axis, int line, float startPosition, float lineSpacing)
        {
            //calculate position for this axis
            float f = startPosition;
            for (int i = 0; i < line; i++)
                f += m_LineTotalPreferredSize[i][axis] + lineSpacing;

            return f;
        }

        protected void SetChildWithScale(RectTransform rect,
            bool horizontalControlSize, float horizontalPos, float horizontalChildSize, float horizontalScaleFactor, float horizontalAlignmentOnAxis,
            bool verticalControlSize, float verticalPos, float verticalChildSize, float verticalScaleFactor, float verticalAlignmentOnAxis)
        {
            //set read-only in inspector the controlled size
            if (horizontalControlSize && verticalControlSize)
            {
                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);
            }
            else if (horizontalControlSize)
            {
                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDeltaX);
            }
            else if (verticalControlSize)
            {
                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDeltaY);
            }
            else
            {
                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition);
            }

            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;

            //set size delta
            rect.sizeDelta = new Vector2(horizontalControlSize ? horizontalChildSize : rect.sizeDelta.x, verticalControlSize ? verticalChildSize : rect.sizeDelta.y);

            //set anchored position
            if (horizontalControlSize == false) horizontalPos += (horizontalChildSize - rect.sizeDelta[0]) * horizontalAlignmentOnAxis;
            if (verticalControlSize == false) verticalPos += (verticalChildSize - rect.sizeDelta[1]) * verticalAlignmentOnAxis;

            rect.anchoredPosition = new Vector2(horizontalPos + rect.sizeDelta[0] * rect.pivot[0] * horizontalScaleFactor,
                -verticalPos - rect.sizeDelta[1] * (1f - rect.pivot[1]) * verticalScaleFactor);
        }

        protected float GetTotalMinSize(int line, int axis)
        {
            return m_LineTotalMinSize[line][axis];
        }

        protected float GetTotalPreferredSize(int line, int axis)
        {
            return m_LineTotalPreferredSize[line][axis];
        }

        protected float GetTotalFlexibleSize(int line, int axis)
        {
            return m_LineTotalFlexibleSize[line][axis];
        }

        #endregion

        #region HorizontalOrVerticalLayoutGroup editor fixes

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            // For new added components we want these to be set to false,
            // so that the user's sizes won't be overwritten before they
            // have a chance to turn these settings off.
            // However, for existing components that were added before this
            // feature was introduced, we want it to be on be default for
            // backwardds compatibility.
            // Hence their default value is on, but we set to off in reset.
            m_ChildControlWidth = false;
            m_ChildControlHeight = false;
        }

        private int m_Capacity = 10;
        private Vector2[] m_Sizes = new Vector2[10];

        protected virtual void Update()
        {
            if (Application.isPlaying)
                return;

            int count = transform.childCount;

            if (count > m_Capacity)
            {
                if (count > m_Capacity * 2)
                    m_Capacity = count;
                else
                    m_Capacity *= 2;

                m_Sizes = new Vector2[m_Capacity];
            }

            // If children size change in editor, update layout (case 945680 - Child GameObjects in a Horizontal/Vertical Layout Group don't display their correct position in the Editor)
            bool dirty = false;
            for (int i = 0; i < count; i++)
            {
                RectTransform t = transform.GetChild(i) as RectTransform;
                if (t != null && t.sizeDelta != m_Sizes[i])
                {
                    dirty = true;
                    m_Sizes[i] = t.sizeDelta;
                }
            }

            if (dirty)
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

#endif

        #endregion
    }

    #region custom editor

#if UNITY_EDITOR

    [CustomEditor(typeof(DynamicGridLayoutGroup), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the DynamicGridLayout Component.
    /// Extend this class to write a custom editor for a component derived from DynamicGridLayout.
    /// </summary>
    public class DynamicGridLayoutGroupEditor : Editor
    {
        SerializedProperty m_Padding;
        SerializedProperty m_Spacing;
        SerializedProperty m_StartCorner;
        SerializedProperty m_StartAxis;
        SerializedProperty m_ChildAlignment;
        SerializedProperty m_ChildControlWidth;
        SerializedProperty m_ChildControlHeight;
        SerializedProperty m_ChildScaleWidth;
        SerializedProperty m_ChildScaleHeight;
        SerializedProperty m_ChildForceExpandWidth;
        SerializedProperty m_ChildForceExpandHeight;
        SerializedProperty m_ReverseArrangement;
        SerializedProperty m_WrappingMode;

        protected virtual void OnEnable()
        {
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_StartCorner = serializedObject.FindProperty("m_StartCorner");
            m_StartAxis = serializedObject.FindProperty("m_StartAxis");
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
            m_ChildControlWidth = serializedObject.FindProperty("m_ChildControlWidth");
            m_ChildControlHeight = serializedObject.FindProperty("m_ChildControlHeight");
            m_ChildScaleWidth = serializedObject.FindProperty("m_ChildScaleWidth");
            m_ChildScaleHeight = serializedObject.FindProperty("m_ChildScaleHeight");
            m_ChildForceExpandWidth = serializedObject.FindProperty("m_ChildForceExpandWidth");
            m_ChildForceExpandHeight = serializedObject.FindProperty("m_ChildForceExpandHeight");
            m_ReverseArrangement = serializedObject.FindProperty("m_ReverseArrangement");
            m_WrappingMode = serializedObject.FindProperty("m_WrappingMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_StartCorner, true);
            EditorGUILayout.PropertyField(m_StartAxis, true);
            EditorGUILayout.PropertyField(m_ChildAlignment, true);
            EditorGUILayout.PropertyField(m_WrappingMode, true);
            EditorGUILayout.PropertyField(m_ReverseArrangement, true);

            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Control Child Size"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_ChildControlWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, m_ChildControlHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Use Child Scale"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_ChildScaleWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, m_ChildScaleHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Child Force Expand"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_ChildForceExpandWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, m_ChildForceExpandHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            serializedObject.ApplyModifiedProperties();
        }

        void ToggleLeft(Rect position, SerializedProperty property, GUIContent label)
        {
            bool toggle = property.boolValue;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            toggle = EditorGUI.ToggleLeft(position, label, toggle);
            EditorGUI.indentLevel = oldIndent;
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = property.hasMultipleDifferentValues ? true : !property.boolValue;
            }
            EditorGUI.EndProperty();
        }
    }

#endif

    #endregion
}