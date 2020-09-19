using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectLoop : MonoBehaviour,IBeginDragHandler
{
    private ScrollRect _scroll;
    public List<BaseLoopModel> LoopModels;
    public ScrollRect ScrollRectComp
    {
        get => _scroll ?? (_scroll = GetComponent<ScrollRect>());
    }
    
    private RectTransform rectTransform => transform as RectTransform;

    private RectTransform _contentRectTransform;
    private RectTransform contentRectTransform
    {
        get=> _contentRectTransform ?? (_contentRectTransform = ScrollRectComp.content);
    }

    private HashSet<int> _showIndexes = new HashSet<int>();
    private List<BaseCell> _currentRenderCell = new List<BaseCell>();
    private HashSet<int> _rendingIndexes = new HashSet<int>();
    private LoopObjectPool _objectPool = new LoopObjectPool();

    private ILoopLayout _loopLayout;

    private void ReturnObject(GameObject go)
    {
        _objectPool.ReturnObject(go);
    }
    
    private BaseCell GetObject(CellEnum cellEnum)
    {
        var go = _objectPool.GetObject(cellEnum);
        
        return go.GetComponent<BaseCell>();
    }

    private void Awake()
    {
        _loopLayout = contentRectTransform.GetComponent<ILoopLayout>();
        _loopLayout.ObjectPool = _objectPool;
        ScrollRectComp.onValueChanged.AddListener(OnScrollValueChange);
        _loopLayout.AddCalculateCompleteEvent(() => { RenderCells(true); });
        _objectPool.SetLoopContent(contentRectTransform);
    }

    public void RegisterCell(CellEnum cellEnum, GameObject go, Type cellType)
    {
        var rect = go.transform as RectTransform;
        rect.anchorMin = Vector2.up;
        rect.anchorMax = Vector2.up;
        _objectPool.RegisterCell(cellEnum,go,cellType);
    }

    private void OnScrollValueChange(Vector2 normalization)
    {
        RenderCells();
    }

    private void RenderCells(bool isRemoveAll = false)
    {
        var normalization = ScrollRectComp.normalizedPosition;
        GetVisualCell(normalization);
        RemoveCells(isRemoveAll);

        foreach (int index in _showIndexes)
        {
            if (!_rendingIndexes.Contains(index))
            {
                var baseLoopModel = LoopModels[index];
                var cell = GetObject(baseLoopModel.CellType);
                cell.RenderIndex = index;
                _currentRenderCell.Add(cell);
                cell.BuildData(baseLoopModel);
                _loopLayout.SetChildAlongAxis(cell.transform as RectTransform, baseLoopModel,0);
                _loopLayout.SetChildAlongAxis(cell.transform as RectTransform, baseLoopModel,1);   
            }
        }
    }

    public void RefreshView()
    {
        _loopLayout.LoopModels = LoopModels;
    }

    private void RemoveCells(bool isRemoveAll = false)
    {
        _rendingIndexes.Clear();
        if (isRemoveAll)
        {
            foreach (RectTransform rectTransform in contentRectTransform)
            {
                ReturnObject(rectTransform.gameObject);
                
            }
//            _showIndexes.Clear();
            _currentRenderCell.Clear();
            return;
        }

        var tmpRemove = new List<BaseCell>();
        foreach (BaseCell baseCell in _currentRenderCell)
        {
            if (!_showIndexes.Contains(baseCell.RenderIndex))
            {
                tmpRemove.Add(baseCell);
                ReturnObject(baseCell.gameObject);
            }
            else
            {
                _rendingIndexes.Add(baseCell.RenderIndex);
            }
        }

        foreach (BaseCell cell in tmpRemove)
        {
            _currentRenderCell.Remove(cell);
        }
    }

    private void GetVisualCell(Vector2 normalization)
    {
        _showIndexes.Clear();
        if (LoopModels == null)
        {
            return;
        }
        var sizeDelta = contentRectTransform.sizeDelta;

        var scrollSizeDelta = rectTransform.sizeDelta;
        
        float scrollXFrom = normalization.x * (sizeDelta[0] - scrollSizeDelta[0]);
        float scrollYFrom = (1 - normalization.y) * (sizeDelta[1] - scrollSizeDelta[1]);
        float scrollXTo = scrollXFrom + scrollSizeDelta[0];
        float scrollYTo = scrollYFrom + scrollSizeDelta[1];
    
        var contentRect = new Rect(scrollXFrom, scrollYFrom, scrollXTo - scrollXFrom, scrollYTo - scrollYFrom);
        for (int i = 0; i < LoopModels.Count; i++)
        {
            var loopModel = LoopModels[i];
            var cellRect = loopModel.GetCellRect();
            if (contentRect.Overlaps(cellRect))
            {
                _showIndexes.Add(i);
            }
        }
    }

    private Coroutine _smoothMoveCo;

    public void ScrollToIndex(int index,bool smoothMove = false,float duration = 0.1f)
    {
        var model = LoopModels[index];
        var rect = model.GetCellRect();
        var sizeDelta = contentRectTransform.sizeDelta;
        var scrollSizeDelta = rectTransform.sizeDelta;
        var normalizationDelta = sizeDelta - scrollSizeDelta;
        var xNormalization = rect.xMin/ normalizationDelta[0];
        var yNormalization = rect.yMin / normalizationDelta[1];
        if (smoothMove)
        {
            var targetNormalizationPos = new Vector2(xNormalization, 1 - yNormalization);
            _smoothMoveCo = StartCoroutine(SmoothMove(targetNormalizationPos,duration));
        }
        else
        {
            ScrollRectComp.normalizedPosition = new Vector2(xNormalization,1-yNormalization);   
        }
    }

    private IEnumerator SmoothMove(Vector2 targetPos,float duration)
    {
        var currentPos = ScrollRectComp.normalizedPosition;
        var time = 0f;
        while (true)
        {
            var t = time / duration;
            ScrollRectComp.normalizedPosition = Vector2.Lerp(currentPos,targetPos,t);
            time += Time.deltaTime;
            yield return null;
            if (time >= duration)
            {
                ScrollRectComp.normalizedPosition = targetPos;
                break;
            }
        }
    }
    
    

    private void OnDestroy()
    {
        _objectPool.ClearPool();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_smoothMoveCo != null)
        {
            StopCoroutine(_smoothMoveCo);
        }
    }
}
