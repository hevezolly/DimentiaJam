using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledItem : MonoBehaviour
{
    private Pool pool;

    public void AssignToPool(Pool pool) 
    {
        this.pool = pool;
    }

    public void ReturnToPool() 
    {
        if (pool == null || !pool.Initiated)
            Destroy(gameObject);
        else
            pool.ReturnItem(this);
    }
}
