using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControll : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;
    [SerializeField] private ScriptableValue<float> handPosition;
    [SerializeField] private ScriptableValue<bool> HandClosed;
    [SerializeField] private ScriptableValueField<float> closedHandSizeMult;
    [SerializeField] private ScriptableValueField<float> rotatedHandSquize;

    private Vector3 defaultScale;
    private Vector3 scale;
    private float position;

    private void Awake()
    {
        scale = renderer.transform.localScale;
        defaultScale = scale;
        handPosition.ValueChangeEvent.AddListener(OnHandPositionChanged);
        HandClosed.ValueChangeEvent.AddListener(OnOpenChange);
    }

    public void OnOpenChange(bool grab) 
    {
        if (grab) 
        {
            renderer.sprite = closeSprite;
            scale.x = defaultScale.x * closedHandSizeMult;
        }
        else 
        {
            renderer.sprite = openSprite;
            scale.x = defaultScale.x;
        }
        OnHandPositionChanged(position);
    }

    public void OnHandPositionChanged(float position) 
    {
        this.position = position;
        var angle = position * Mathf.PI;
        var squize = Mathf.Lerp(1, 1 - rotatedHandSquize, 1 - Mathf.Sin(angle));
        renderer.transform.localScale = new Vector3(scale.x, scale.y * squize, scale.z);
    }
}
