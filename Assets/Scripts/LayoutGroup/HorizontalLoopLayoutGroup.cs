using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalLoopLayoutGroup : LoopHorizontalOrVerticalLayout
{
    protected HorizontalLoopLayoutGroup()
    {}

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, false);
    }

    public override void CalculateLayoutInputVertical()
    {
        CalcAlongAxis(1, false);
    }

    public override void SetLayoutHorizontal()
    {
        SetChildrenAlongAxis(0, false);
    }

    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1, false);
    }
}
