using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SanityDrain : MonoBehaviour
{
    [SerializeField] private ScriptableValue<float> sanityLevel;
    [SerializeField] private ScriptableValueField<float> sanityDrain;

    private void Awake()
    {
        sanityLevel.Value = 1f;
        sanityLevel.ValueChangeEvent.AddListener(OnSanityChange);
    }

    private static WaitForSeconds wait = new WaitForSeconds(10);

    private IEnumerator Start()
    {
        while (sanityLevel > 0) 
        {
            yield return wait;
            sanityLevel.Value -= sanityDrain * 10; 
        }
    }

    private void OnSanityChange(float value) 
    {
        if (value > 0)
            return;
        GameOver();
    }

    private void GameOver() 
    {
        Debug.LogAssertion("Game over");
    }
    
}
