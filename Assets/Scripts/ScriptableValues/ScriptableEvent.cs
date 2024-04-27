using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableEvents/Empty")]
public class ScriptableEvent: ScriptableObject 
{
    [SerializeField] private UnityEvent unityEvent;

    public void AddListener(UnityAction action) => unityEvent.AddListener(action);
    public void RemoveListener(UnityAction action) => unityEvent.RemoveListener(action);
    public void RemoveAllListeners() => unityEvent.RemoveAllListeners();

    public void Trigger() => unityEvent?.Invoke();
}

public abstract class ScriptableEvent<T>: ScriptableObject 
{
    [SerializeField] private UnityEvent<T> unityEvent;

    public void AddListener(UnityAction<T> action) => unityEvent.AddListener(action);
    public void RemoveListener(UnityAction<T> action) => unityEvent.RemoveListener(action);
    public void RemoveAllListeners() => unityEvent.RemoveAllListeners();

    public void Trigger(T value) => unityEvent?.Invoke(value);
}
