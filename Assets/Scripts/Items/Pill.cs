using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill : Item, IUsable
{
    [SerializeField] private ScriptableValue<float> sanityLevel;
    [SerializeField] private ScriptableValueField<float> sanityRestore;
    [SerializeField] private Pool useEffect;


    public bool CanBeUsed => true;

    public void Use()
    {
        sanityLevel.Value += sanityRestore;
        Holder.itemHandle.DestryHeldItem();
        useEffect.Get<PooledItem>(transform.position, Quaternion.identity);
    }
}
