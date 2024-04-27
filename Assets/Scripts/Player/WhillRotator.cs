using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhillRotator : MonoBehaviour
{
    [SerializeField] private ScriptableValue<float> WheelRevolutions;
    [SerializeField] private Material material;

    private void Awake()
    {
        WheelRevolutions.ValueChangeEvent.AddListener(OnRevolutionsChanged);
    }

    private void OnRevolutionsChanged(float revolutionCount) 
    {
        material.SetFloat("_RevolutionsCount", revolutionCount * 2f);
    }
}
