# ScrollRectLoop

## 介绍

在阅读UGUI源码的时候发现LayoutGroup已经帮忙做了有关ChildSize处理，所以基于UGUI的LayoutSystem，写了这个循环滚动列表。

如果对UGUI的LayoutSystem不是很了解的话，可以先看官方文档或者戳这里查看我的源码阅读笔记

https://jechyang.github.io/2020/09/14/LayoutGroup%E4%BB%A5%E5%8F%8AContentSizeFitter/

如果使用过程中有什么问题或者好的建议都可以给我提issue或者在博客底下评论。

## 设计思路

循环滚动列表设计的主要思路就是Content大小的计算以及每个Cell(格子)的位置记录，得到相关信息之后就可以根据ScrollRect在ViewRect中区域确定所需要渲染的Cell了。

这里基于UGUI的HorzontalLayoutGroup、VerticalLayoutGroup、GridLayoutGroup，写了对应的Loop版本，然后主要就是更改RebuildLayout中的四个流程方法，本来是根据ChildrenGameObjects来计算各种sizes的(preferred/min/flex size)，这里直接改成使用设置的Models来进行计算。得到了content的sizes(min/preferred/flex)之后就可以利用ContentSizeFitter控件将ContentSize设置上去了，同时也得到了child相关的位置信息，同样存储在model中，这样在scrollrect滚动的过程中就知道要刷新哪些cell了。

这里的size设置是遵循ugui的布局的，因此不规则的列表也可以很好地显示(聊天之类的按照文字确认高度)

## 效果展示

![img](https://github.com/jechyang/ScrollRectLoop/blob/master/gif/scrollrectloop.gif)

## 使用步骤及示例

- ScrollRect的component的挂载

  首先ScrollRect上要挂载ScrollRectLoop组件，然后Content上要挂载所需要的XXXXLoopLayout以及ContentSizeFitter(用来设置Content的大小)

- 在CellEnum.cs里声明CellType

  ```c#
  public enum CellEnum 
  {
      Test,
  }
  ```

- 编写CellType对应的CellComponent以及CellModel。

  CellComponent即我们会挂在Cell上的Component，需要继承自BaseCell，到时候会调用流程方法**BuildData**并传入对应的CellModel，如下

  ```c#
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
  ```

  CellModel也需要继承自BaseModel，然后只需要复写所对应的**CellType**属性即可，如下所示

  ```c#
  public class TestModel : BaseLoopModel
  {
      public string content;
      public override CellEnum CellType => CellEnum.Test;
  }
  ```

- 使用ScrollRectLoopComponent注册CellType对应的Prefab以及CellComponent

  ```c#
  _rectLoop = GetComponent<ScrollRectLoop>();
  _rectLoop.RegisterCell(CellEnum.Test,TestTrans.gameObject,typeof(TestCell));
  ```

- 设置ScrollRectLoop对应的models，调用refreshView方法

  ```c#
  var models = GetTestModels();
  _rectLoop.LoopModels = models;
  _rectLoop.RefreshView();
  ```

  这些东西都在项目中的example这个文件夹中，如果有不清楚的地方可以直接打开项目进行查看。

## 其他方法

可以使用**ScrollToIndex**方法来跳转到指定index的cell。即gif中演示的效果，方法签名如下

```c#
public void ScrollToIndex(int index,bool smoothMove = false,float duration = 0.1f)
```

smoothMove的话就会有个滑动效果，然后是在duration的时间内达到指定cell的位置

使用协程实现，监听了OnBeginDrag，如果检测到drag就会取消滑动的协程。