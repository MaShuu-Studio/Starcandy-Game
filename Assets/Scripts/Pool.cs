using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T: Poolable
{
    protected T objPrefab;

    protected Stack<T> pool;

    public void Init(T prefab)
    {
        objPrefab = prefab;
        if (objPrefab.gameObject.activeSelf)
        {
            objPrefab.gameObject.name = "BASE";
            objPrefab.transform.SetParent(transform);
            objPrefab.gameObject.SetActive(false);
        }
        pool = new Stack<T>();

        for (int i = 0; i < prefab.Amount; i++)
        {
            CreateObject();
        }
    }

    public void CreateObject()
    {
        T poolable = Instantiate(objPrefab);
        poolable.MakePrefab(objPrefab.Name);
        Push(poolable);
    }

    public void Push(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Push(obj);
    }

    public T Pop()
    {
        if (pool.Count == 0) CreateObject();

        T obj = pool.Pop();
        obj.gameObject.SetActive(true);

        return obj;
    }
}
