using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCell : MonoBehaviour
{
    public int RenderIndex { get; set; }
    public CellEnum CellType { get; set; }
    // Start is called before the first frame update
    public abstract void BuildData(BaseLoopModel model);

    public virtual void OnReset()
    {
        
    }

}
