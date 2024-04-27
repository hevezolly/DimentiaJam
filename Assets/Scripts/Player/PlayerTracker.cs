using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    [SerializeField] private ScriptableValueField<Vector2> playerPosition;

    private void Awake()
    {
        playerPosition.Value = transform.position;
    }

    private void LateUpdate()
    {
        playerPosition.Value = transform.position;
    }
}
