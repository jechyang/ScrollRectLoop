using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopGridLayout : LayoutGroup,ILoopLayout
{
    /// <summary>
    /// Which corner is the starting corner for the grid.
    /// </summary>
    public enum Corner
    {
        /// <summary>
        /// Upper Left corner.
        /// </summary>
        UpperLeft = 0,

        /// <summary>
        /// Upper Right corner.
        /// </summary>
        UpperRight = 1,

        /// <summary>
        /// Lower Left corner.
        /// </summary>
        LowerLeft = 2,

        /// <summary>
        /// Lower Right corner.
        /// </summary>
        LowerRight = 3
    }

    /// <summary>
    /// The grid axis we are looking at.
    /// </summary>
    /// <remarks>
    /// As the storage is a [][] we make access easier by passing a axis.
    /// </remarks>
    public enum Axis
    {
        /// <summary>
        /// Horizontal axis
        /// </summary>
        Horizontal = 0,

        /// <summary>
        /// Vertical axis.
        /// </summary>
        Vertical = 1
    }

    /// <summary>
    /// Constraint type on either the number of columns or rows.
    /// </summary>
    public enum Constraint
    {
        /// <summary>
        /// Don't constrain the number of rows or columns.
        /// </summary>
        Flexible = 0,

        /// <summary>
        /// Constrain the number of columns to a specified number.
        /// </summary>
        FixedColumnCount = 1,

        /// <summary>
        /// Constraint the number of rows to a specified number.
        /// </summary>
        FixedRowCount = 2
    }

    [SerializeField] protected Corner m_StartCorner = Corner.UpperLeft;

    /// <summary>
    /// Which corner should the first cell be placed in?
    /// </summary>
    public Corner startCorner
    {
        get { return m_StartCorner; }
        set { SetProperty(ref m_StartCorner, value); }
    }

    [SerializeField] protected Axis m_StartAxis = Axis.Horizontal;

    /// <summary>
    /// Which axis should cells be placed along first
    /// </summary>
    /// <remarks>
    /// When startAxis is set to horizontal, an entire row will be filled out before proceeding to the next row. When set to vertical, an entire column will be filled out before proceeding to the next column.
    /// </remarks>
    public Axis startAxis
    {
        get { return m_StartAxis; }
        set { SetProperty(ref m_StartAxis, value); }
    }

    [SerializeField] protected Vector2 m_CellSize = new Vector2(100, 100);

    /// <summary>
    /// The size to use for each cell in the grid.
    /// </summary>
    public Vector2 cellSize
    {
        get { return m_CellSize; }
        set { SetProperty(ref m_CellSize, value); }
    }

    [SerializeField] protected Vector2 m_Spacing = Vector2.zero;

    /// <summary>
    /// The spacing to use between layout elements in the grid on both axises.
    /// </summary>
    public Vector2 spacing
    {
        get { return m_Spacing; }
        set { SetProperty(ref m_Spacing, value); }
    }

    [SerializeField] protected Constraint m_Constraint = Constraint.Flexible;

    /// <summary>
    /// Which constraint to use for the GridLayoutGroup.
    /// </summary>
    /// <remarks>
    /// Specifying a constraint can make the GridLayoutGroup work better in conjunction with a [[ContentSizeFitter]] component. When GridLayoutGroup is used on a RectTransform with a manually specified size, there's no need to specify a constraint.
    /// </remarks>
    public Constraint constraint
    {
        get { return m_Constraint; }
        set { SetProperty(ref m_Constraint, value); }
    }

    [SerializeField] protected int m_ConstraintCount = 2;

    /// <summary>
    /// How many cells there should be along the constrained axis.
    /// </summary>
    public int constraintCount
    {
        get { return m_ConstraintCount; }
        set { SetProperty(ref m_ConstraintCount, Mathf.Max(1, value)); }
    }

    protected LoopGridLayout()
    {
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        constraintCount = constraintCount;
    }

#endif
    private List<BaseLoopModel> _loopModels;

    public List<BaseLoopModel> LoopModels
    {
        get => _loopModels;
        set
        {
            _loopModels = value;
            SetDirty();
        }
    }
    
    public LoopObjectPool ObjectPool { get; set; }

    
    private Action _onLayoutCalculateCompleteEvent;
    public void AddCalculateCompleteEvent(Action action)
    {
        _onLayoutCalculateCompleteEvent = action;
    }

    private ContentSizeFitter _contentSizeFitter;

    private ContentSizeFitter contentSizeFitter =>
        _contentSizeFitter ?? (_contentSizeFitter = GetComponent<ContentSizeFitter>());

    protected new void SetDirty()
    {
        enabled = true;
        contentSizeFitter.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        enabled = false;
        contentSizeFitter.enabled = false;
        _onLayoutCalculateCompleteEvent?.Invoke();
    }

    protected override void OnTransformChildrenChanged() {}

    protected override void OnDidApplyAnimationProperties() { }

    protected override void OnRectTransformDimensionsChange() { }

    protected override void OnEnable() { }

    protected override void OnDisable() { }

    /// <summary>
    /// Called by the layout system to calculate the horizontal layout size.
    /// Also see ILayoutElement
    /// </summary>
    public override void CalculateLayoutInputHorizontal()
    {
        if (ObjectPool == null || _loopModels == null)
        {
            return;
        }

        base.CalculateLayoutInputHorizontal();

        int minColumns = 0;
        int preferredColumns = 0;
        if (m_Constraint == Constraint.FixedColumnCount)
        {
            minColumns = preferredColumns = m_ConstraintCount;
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            minColumns = preferredColumns = Mathf.CeilToInt(LoopModels.Count / (float) m_ConstraintCount - 0.001f);
        }
        else
        {
            minColumns = 1;
            preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(LoopModels.Count));
        }

        SetLayoutInputForAxis(
            padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x,
            padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x,
            -1, 0);
    }

    /// <summary>
    /// Called by the layout system to calculate the vertical layout size.
    /// Also see ILayoutElement
    /// </summary>
    public override void CalculateLayoutInputVertical()
    {
        if (ObjectPool == null || _loopModels == null)
        {
            return;
        }

        int minRows = 0;
        if (m_Constraint == Constraint.FixedColumnCount)
        {
            minRows = Mathf.CeilToInt(LoopModels.Count / (float) m_ConstraintCount - 0.001f);
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            minRows = m_ConstraintCount;
        }
        else
        {
            float width = rectTransform.rect.width;
            int cellCountX = Mathf.Max(1,
                Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
            minRows = Mathf.CeilToInt(LoopModels.Count / (float) cellCountX);
        }

        float minSpace = padding.vertical + (cellSize.y + spacing.y) * minRows - spacing.y;
        SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
    }

    public override void SetLayoutHorizontal()
    {
        SetCellsAlongAxis(0);
    }

    public override void SetLayoutVertical()
    {
        SetCellsAlongAxis(1);
    }

    private void SetCellsAlongAxis(int axis)
    {
        if (ObjectPool == null || _loopModels == null)
        {
            return;
        }

        if (axis == 0)
        {
            var allPrefabs = ObjectPool.GetAllPrefabs();
            foreach (GameObject prefab in allPrefabs)
            {
                var rect = prefab.transform as RectTransform;
                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);

                rect.anchorMin = Vector2.up;
                rect.anchorMax = Vector2.up;
                rect.sizeDelta = cellSize;
            }
            return;
        }

        float width = rectTransform.rect.size.x;
        float height = rectTransform.rect.size.y;

        int cellCountX = 1;
        int cellCountY = 1;
        if (m_Constraint == Constraint.FixedColumnCount)
        {
            cellCountX = m_ConstraintCount;

            if (LoopModels.Count > cellCountX)
                cellCountY = LoopModels.Count / cellCountX + (LoopModels.Count % cellCountX > 0 ? 1 : 0);
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            cellCountY = m_ConstraintCount;

            if (LoopModels.Count > cellCountY)
                cellCountX = LoopModels.Count / cellCountY + (LoopModels.Count % cellCountY > 0 ? 1 : 0);
        }
        else
        {
            if (cellSize.x + spacing.x <= 0)
                cellCountX = int.MaxValue;
            else
                cellCountX = Mathf.Max(1,
                    Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

            if (cellSize.y + spacing.y <= 0)
                cellCountY = int.MaxValue;
            else
                cellCountY = Mathf.Max(1,
                    Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
        }

        int cornerX = (int) startCorner % 2;
        int cornerY = (int) startCorner / 2;

        int cellsPerMainAxis, actualCellCountX, actualCellCountY;
        if (startAxis == Axis.Horizontal)
        {
            cellsPerMainAxis = cellCountX;
            actualCellCountX = Mathf.Clamp(cellCountX, 1, LoopModels.Count);
            actualCellCountY =
                Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(LoopModels.Count / (float) cellsPerMainAxis));
        }
        else
        {
            cellsPerMainAxis = cellCountY;
            actualCellCountY = Mathf.Clamp(cellCountY, 1, LoopModels.Count);
            actualCellCountX =
                Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(LoopModels.Count / (float) cellsPerMainAxis));
        }

        Vector2 requiredSpace = new Vector2(
            actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
            actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
        );
        Vector2 startOffset = new Vector2(
            GetStartOffset(0, requiredSpace.x),
            GetStartOffset(1, requiredSpace.y)
        );

        for (int i = 0; i < LoopModels.Count; i++)
        {
            int positionX;
            int positionY;
            if (startAxis == Axis.Horizontal)
            {
                positionX = i % cellsPerMainAxis;
                positionY = i / cellsPerMainAxis;
            }
            else
            {
                positionX = i / cellsPerMainAxis;
                positionY = i % cellsPerMainAxis;
            }

            if (cornerX == 1)
                positionX = actualCellCountX - 1 - positionX;
            if (cornerY == 1)
                positionY = actualCellCountY - 1 - positionY;

            var go = ObjectPool.GetObject(_loopModels[i].CellType);
            var child = go.transform as RectTransform;

            SetChildAlongAxis(child, 0, startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
//            _loopModels[i].RefreshCellRect(child, 0);
            _loopModels[i].RefreshCellSizeData(0, startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
            SetChildAlongAxis(child, 1, startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
//            _loopModels[i].RefreshCellRect(child, 1);
            _loopModels[i].RefreshCellSizeData(1, startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
            
            ObjectPool.ReturnObject(go);
        }
    }
    
    public void SetChildAlongAxis(RectTransform child,BaseLoopModel model,int axis)
    {
        SetChildAlongAxis(child, axis, model.GetOffset(axis),model.GetSize(axis));
    }
}