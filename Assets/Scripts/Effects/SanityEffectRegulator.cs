using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityEffectRegulator : MonoBehaviour
{
    [SerializeField] private ScriptableValue<float> sanity;
    [SerializeField] private Material material;

    private void Start()
    {
        UpdateValues(sanity.Value);
        sanity.ValueChangeEvent.AddListener(UpdateValues);
    }

    private void UpdateValues(float sanity) 
    {
        material.SetFloat("_sanity", 1 - sanity);
        
    }
}
