using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolInitiator : MonoBehaviour
{
    [SerializeField] private bool InitOnStart;
    [SerializeField] private Pool pool;

    private Queue<PooledItem> readyItems = new Queue<PooledItem>();

    private void Awake() {
        if (InitOnStart)
            Init();
    }

    public void Init() 
    {
        var initialLen = readyItems.Count;
        for (var i = 0; i < pool.NumberOfItems - initialLen; i++) 
        {
            var inst = Instantiate(pool.Item);
            inst.AssignToPool(pool);
            ReturnItem(inst);
        }    
        pool.Init(this);

    }

    private void OnDestroy()
    {
        pool.Deinit();
    }

    public T Get<T>(Vector3 position, Transform parent = null) 
        => Get<T>(position, Quaternion.identity, parent);

    public T Get<T>(Transform parent = null) => Get<T>(Vector3.zero, parent);

    public T Get<T>(Vector3 position, Quaternion rotation, Transform parent)
    {
#if UNITY_EDITOR
        if (!pool.Item.TryGetComponent<T>(out var _))
            throw new System.TypeAccessException($"this pool does not contain items of type {typeof(T).Name}");
#endif
        if (readyItems.Count == 0) 
        {
            PooledItem instance = null;
            if (parent != null)
                instance = Instantiate(pool.Item, position, rotation);
            else
                instance = Instantiate(pool.Item, position, rotation, parent);
            instance.AssignToPool(pool);
            return instance.GetComponent<T>();
        }
        else 
        {
            var inst = readyItems.Dequeue();
            inst.transform.SetParent(parent);
            inst.transform.SetPositionAndRotation(position, rotation);
            inst.AssignToPool(pool);
            inst.gameObject.SetActive(true);
            return inst.GetComponent<T>();
        }
    }

    public void ReturnItem(PooledItem item) 
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(transform, true);
        readyItems.Enqueue(item);
    }
}
