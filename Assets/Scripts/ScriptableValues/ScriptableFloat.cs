using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Values/Float")]
public class ScriptableFloat : ScriptableValue<float>
{
    [SerializeField] private bool ClampMax = false;
    [SerializeField] private float Max = 1;
    [SerializeField] private bool ClampMin = false;
    [SerializeField] private float Min = 0;
    public override float Value 
    { 
        get => base.Value; 
        set 
        {
            base.Value = Clamp(value);
        }      
    }

    private float Clamp(float value) 
    {
        var realValue = value;
        if (ClampMax)
            realValue = Mathf.Min(realValue, Max);
        if (ClampMin)
            realValue = Mathf.Max(realValue, Min);
        return realValue;
    }

    protected override void OnValidate()
    {
        currentValue = Clamp(currentValue);
        Value = currentValue;
    }
}
