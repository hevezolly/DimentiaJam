using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ScriptableValue<T> : ScriptableObject
{
    [SerializeField]
    protected T currentValue;

    [SerializeField]
    private UnityEvent<T> valueChangeEvent = new UnityEvent<T>();

    public virtual UnityEvent<T> ValueChangeEvent => valueChangeEvent;

    private T _previusValue;

    public virtual T GetPreviusValue() => _previusValue;


    public static implicit operator T(ScriptableValue<T> value) => value.Value;

    public virtual T Value
    {
        get => currentValue;
        set
        {
            if (_previusValue != null && _previusValue.Equals(value))
                return;
            currentValue = value;
            _previusValue = value;
            if (Application.isPlaying)
                ValueChangeEvent?.Invoke(currentValue);
        }
    }

    protected virtual void OnValidate()
    {
        Value = currentValue;
    }
}
