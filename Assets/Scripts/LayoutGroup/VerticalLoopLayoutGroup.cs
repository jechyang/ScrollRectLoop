using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalLoopLayoutGroup : LoopHorizontalOrVerticalLayout
{
    protected VerticalLoopLayoutGroup()
    {}

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, true);
    }

    public override void CalculateLayoutInputVertical()
    {
        CalcAlongAxis(1, true);
    }

    public override void SetLayoutHorizontal()
    {
        SetChildrenAlongAxis(0, true);
    }

    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1, true);
    }
}
