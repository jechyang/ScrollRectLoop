using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCell : BaseCell
{
    private Text _testTxt;
    private void Awake()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        _testTxt = transform.Find("Text").GetComponent<Text>();
    }

    public override void BuildData(BaseLoopModel model)
    {
        var testModel = model as TestModel;
        _testTxt.text = testModel.content;
    }
}
