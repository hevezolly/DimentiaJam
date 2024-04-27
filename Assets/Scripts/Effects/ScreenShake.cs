using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private ScriptableEvent<float> screenShakeEvent;
    [SerializeField] private ScriptableValueField<float> ScreenshakeStrength;
    [SerializeField] private ScriptableValueField<float> ScreenShakeDecrease;

    private float currentStrength = 0f;

    private void Awake()
    {
        screenShakeEvent.AddListener(DoScreenShake);
    }

    private void Update()
    {
        if (currentStrength <= 0) 
        {
            transform.localPosition = Vector3.zero;
            return;
        }
        var offset = Random.insideUnitCircle * currentStrength * ScreenshakeStrength;
        transform.localPosition = offset;
        currentStrength -= Time.deltaTime * ScreenShakeDecrease;
    }

    private void DoScreenShake(float strength) 
    {
        currentStrength = Mathf.Max(currentStrength, strength);
        currentStrength = Mathf.Clamp01(currentStrength);
    }

    private void OnDestroy()
    {
        screenShakeEvent.RemoveListener(DoScreenShake);
    }
}
