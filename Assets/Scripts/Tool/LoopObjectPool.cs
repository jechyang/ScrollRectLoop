using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class LoopObjectPool
{
    private Dictionary<CellEnum,Queue<GameObject>> _poolDic = new Dictionary<CellEnum, Queue<GameObject>>();
    private Dictionary<CellEnum,GameObject> _prefabDic = new Dictionary<CellEnum, GameObject>();
    private Dictionary<CellEnum, Type> _cellTypeDic = new Dictionary<CellEnum, Type>();
    private RectTransform _contentTrans;


    public void SetLoopContent(RectTransform contentTrans)
    {
        _contentTrans = contentTrans;
    }

    public void RegisterCell(CellEnum cellEnum,GameObject gameObject,Type cellType)
    {
        _prefabDic.Add(cellEnum,gameObject);
        _cellTypeDic.Add(cellEnum,cellType);
    }
    public GameObject GetObject(CellEnum cellEnum)
    {
        if (!_poolDic.TryGetValue(cellEnum, out var queue))
        {
            queue = new Queue<GameObject>();
            _poolDic.Add(cellEnum,queue);
        }

        var go = queue.Count != 0 ? queue.Dequeue() : CreateObject(cellEnum);
        go.SetActive(true);
        go.transform.SetParent(_contentTrans);
        go.transform.localScale = Vector3.one;
        go.transform.position = Vector3.zero;
        return go;
    }

    private GameObject CreateObject(CellEnum cellEnum)
    {
        GameObject prefab;
        if (_prefabDic.TryGetValue(cellEnum, out prefab))
        {
            var go = Object.Instantiate(prefab, _contentTrans, true);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            var type = GetCellType(cellEnum);
            var cell = go.AddComponent(type) as BaseCell;
            cell.CellType = cellEnum;
            return go;
        }

        Debug.LogError($"没有声明枚举类型为{cellEnum.ToString()}的预制");
        return null;
    }

    public void ReturnObject(GameObject go)
    {
        var cell = go.GetComponent<BaseCell>();
        var cellEnum = cell.CellType;
        cell.OnReset();
        if (_poolDic.TryGetValue(cellEnum, out var queue))
        {
            go.transform.SetParent(null);
            go.SetActive(false);
            queue.Enqueue(go);
        }
    }
    
    private Type GetCellType(CellEnum cellEnum)
    {
        if (_cellTypeDic.TryGetValue(cellEnum, out var type))
        {
            if (type.IsSubclassOf(typeof(BaseCell)))
            {
                return type;   
            }
            else
            {
                Debug.LogError($"所获取的cell类型非继承自BaseCell");
            }
        }
        else
        {
            Debug.LogError($"请求了没有声明的cell");
            
        }
        return null;
        
    }

    public void ClearPool()
    {
        _contentTrans = null;
        _prefabDic.Clear();
        foreach (Queue<GameObject> queue in _poolDic.Values)
        {
            foreach (GameObject go in queue)
            {
                GameObject.Destroy(go);
            }
        }
        _poolDic.Clear();
        _cellTypeDic.Clear();
    }
    
    //这个方法是给一些对cell的rect有要求的用的，可以直接获取到所有register的cell原型
    public List<GameObject> GetAllPrefabs()
    {
        List<GameObject> result = new List<GameObject>();
        foreach (KeyValuePair<CellEnum,GameObject> keyValuePair in _prefabDic)
        {
            var go = keyValuePair.Value;
            result.Add(go);
        }

        return result;
    }

}
