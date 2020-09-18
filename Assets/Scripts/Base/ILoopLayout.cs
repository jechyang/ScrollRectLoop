using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoopLayout
{
    LoopObjectPool ObjectPool { set; }
    List<BaseLoopModel> LoopModels { set; }
    void AddCalculateCompleteEvent(Action action);
    void SetChildAlongAxis(RectTransform child, BaseLoopModel model, int axis);
}
