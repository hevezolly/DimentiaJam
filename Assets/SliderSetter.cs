using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetter : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private ScriptableValueField<float> sensitivity;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable() 
    {
        slider.value = sensitivity.Value;
    }
}
