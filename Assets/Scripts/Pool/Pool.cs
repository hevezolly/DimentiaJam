using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Pool : ScriptableObject
{
    [SerializeField] private PooledItem item;
    public PooledItem Item => item;
    [SerializeField] private ScriptableValueField<int> numberOfItems;
    public int NumberOfItems => numberOfItems;

    public bool Initiated => initiator != null;

    private PoolInitiator initiator;

    public void Init(PoolInitiator initiator) 
    {
        this.initiator = initiator;
    }

    public void Deinit() {
        initiator = null;
    }

    public T Get<T>(Vector3 position, Transform parent = null) 
        => Get<T>(position, Quaternion.identity, parent);

    public T Get<T>(Transform parent = null) => Get<T>(Vector3.zero, parent);

    public T Get<T>(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (Initiated)
            return initiator.Get<T>(position, rotation, parent);
        throw new NullReferenceException("attempt to get items from uninitialized pool");
    }

    public void ReturnItem(PooledItem item) 
    {
        if (Initiated)
            initiator.ReturnItem(item);
        else
            Destroy(item.gameObject);
    }
}
