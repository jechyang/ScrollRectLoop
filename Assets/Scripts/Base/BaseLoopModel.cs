using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLoopModel
{
    private Rect _cellRect;
    private Vector2 _startOffsets = new Vector2();
    private Vector2 _sizes = new Vector2();

    public Vector2 Sizes => _sizes;

    public abstract CellEnum CellType { get; }

    public void RefreshCellRect()
    {
        _cellRect = new Rect(_startOffsets[0],_startOffsets[1],_sizes[0],_sizes[1]);
    }

    public void RefreshCellSizeData(int axis, float startOffset, float size)
    {
        _startOffsets[axis] = startOffset;
        _sizes[axis] = size;
        if (axis == 1)
        {
            RefreshCellRect();
        }
    }

    public float GetOffset(int axis)
    {
        return _startOffsets[axis];
    }

    public float GetSize(int axis)
    {
        return _sizes[axis];
    }

    public Rect GetCellRect()
    {
        return _cellRect;
    }
}