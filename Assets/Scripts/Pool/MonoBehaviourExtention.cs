using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtention
{
    public static void Despawn(this MonoBehaviour behaviour) 
    {
        if (behaviour.TryGetComponent<PooledItem>(out var pool)) 
        {
            pool.ReturnToPool();
        }
        else 
        {
            GameObject.Destroy(behaviour.gameObject);
        }
    }
}
