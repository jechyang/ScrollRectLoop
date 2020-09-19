using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestModel : BaseLoopModel
{
    public string content;
    public override CellEnum CellType => CellEnum.Test;
}


public class TestLayout : MonoBehaviour
{
    public RectTransform TestTrans;
    private ScrollRectLoop _rectLoop;

    public Button TestAddCellBtn;
    public Button TestFocusBtn;
    
    public int modelType = 1;

    private void Awake()
    {
        _rectLoop = GetComponent<ScrollRectLoop>();
        _rectLoop.RegisterCell(CellEnum.Test,TestTrans.gameObject,typeof(TestCell));
        TestAddCellBtn.onClick.AddListener(() =>
        {
            List<BaseLoopModel> models = null;
            if (modelType == 1)
            {
                models = GetTestModels();
            }
            else
            {
                models = GetGridModels();
            }
            _rectLoop.LoopModels = models;
            _rectLoop.RefreshView();
        });
        
        TestFocusBtn.onClick.AddListener(() =>
        {
            _rectLoop.ScrollToIndex(5,true);
        });
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private List<BaseLoopModel> GetTestModels()
    {
        var result = new List<BaseLoopModel>();
        result.Add(new TestModel(){content = "111111111111"});
        result.Add(new TestModel(){content = "22222222222222222222222222"});
        result.Add(new TestModel(){content = "3333333333333333333333333333333333"});
        result.Add(new TestModel(){content = "444444444444444444444444444444444444444444"});
        result.Add(new TestModel(){content = "5555555555555555555555555555555555555555555555555555"});
        result.Add(new TestModel(){content = "6666666666666666666666666666666666666666666666666666666666666666"});
        result.Add(new TestModel(){content = "777777777777777777777777777777777777777777777777777777777777777777777777777777777777777"});
        result.Add(new TestModel(){content = "888888888888888888888888888888888888888888888888888888888888888"});
        result.Add(new TestModel(){content = "999999999999999999999999999999999999999999"});
        result.Add(new TestModel(){content = "十十十十十十十十十十十十十十十十十十"});
        result.Add(new TestModel(){content = "十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一"});
        result.Add(new TestModel(){content = "十二十二十二十二十二十二十二十二十二十二十二十二十二十二十二十二十二十二十二十二"});
        return result;
    }

    private List<BaseLoopModel> GetGridModels()
    {
        var result = new List<BaseLoopModel>();
        result.Add(new TestModel(){content = "1"});
        result.Add(new TestModel(){content = "2"});
        result.Add(new TestModel(){content = "3"});
        result.Add(new TestModel(){content = "4"});
        result.Add(new TestModel(){content = "5"});
        result.Add(new TestModel(){content = "6"});
        result.Add(new TestModel(){content = "7"});
        result.Add(new TestModel(){content = "8"});
        result.Add(new TestModel(){content = "9"});
        result.Add(new TestModel(){content = "10"});
        result.Add(new TestModel(){content = "11"});
        result.Add(new TestModel(){content = "12"});
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
