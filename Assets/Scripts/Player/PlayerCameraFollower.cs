using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFollower : MonoBehaviour
{
    [SerializeField] private ScriptableValue<Vector2> position;
    [SerializeField] private ScriptableValueField<Vector2> offset;
    [SerializeField] private ScriptableValueField<float> ease;

    private void Start()
    {
        var pos2D = position.Value + offset;
        transform.position = new Vector3(pos2D.x, pos2D.y, transform.position.z);
        position.ValueChangeEvent.AddListener(OnPositionChanged);
    }

    private void OnPositionChanged(Vector2 position) 
    {
        var pos2D = position + offset;
        transform.position = Vector3.Lerp(transform.position, 
            new Vector3(pos2D.x, pos2D.y, transform.position.z),
            ease);
    }
}
