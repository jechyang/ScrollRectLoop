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

    public Button TestBtn;

    private void Awake()
    {
        _rectLoop = GetComponent<ScrollRectLoop>();
        _rectLoop.RegisterCell(CellEnum.Test,TestTrans.gameObject,typeof(TestCell));
        TestBtn.onClick.AddListener(() =>
        {
            var models = GetTestModels();
            _rectLoop.LoopModels = models;
            _rectLoop.RefreshView();
        });
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private List<BaseLoopModel> GetTestModels()
    {
        var result = new List<BaseLoopModel>();
        result.Add(new TestModel(){content = "111111111111111111111112312111111111111111111111112312"});
        result.Add(new TestModel(){content = "111111111111111111111112312222222222222222222222"});
        result.Add(new TestModel(){content = "333333333333"});
        result.Add(new TestModel(){content = "111111111111111111111112312"});
        result.Add(new TestModel(){content = "11111111111111111111111231244444444444444444444444444444444444444"});
        result.Add(new TestModel(){content = "11111111111111111111111231266666666666666666666666666666666666666666666666666"});
        result.Add(new TestModel(){content = "111111111111111111111112312111111111111111111111112312"});
        result.Add(new TestModel(){content = "1111111111111111111111123 77777777777777777777777777777777777777777777777777"});
        result.Add(new TestModel(){content = "333333333333dsaddsadsadasdsadsadsa888888888888888888888888888888888888888888888888888888"});
        result.Add(new TestModel(){content = "111111111111111111111112312dasdasdsadasdasdasdsa9999999999999999999999999999999999999999999999999999999"});
        result.Add(new TestModel(){content = "111111111111111111111112312十十十十十十十十十十十十十十十十十十十十十十十十十十十十十十十十十十十十dsadsadasdasdsa"});
        result.Add(new TestModel(){content = "111111111111111111111112312十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一十一"});
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
