using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptableValueField<T>
{
    [SerializeField]
    private ScriptableValue<T> scriptableValue;

    [SerializeField] private bool UseSO = false;
    [SerializeField] private T noSOValue;
    

    public bool HasValue => (UseSO && scriptableValue != null) || (!UseSO && noSOValue != null);

    public T Value
    {
        get {
            if (UseSO)
                return scriptableValue.Value;
            else 
                return noSOValue;
        } 
        set {
            if (UseSO)
                scriptableValue.Value = value;
            else
                noSOValue = value;
        }
    }

    public static implicit operator T(ScriptableValueField<T> val) => val.Value;
}