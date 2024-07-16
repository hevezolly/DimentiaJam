using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayersMaxSpeed : MonoBehaviour
{
    [SerializeField] private ScriptableValueField<float> maxSpeed;
    [SerializeField] private ScriptableValueField<float> deceleration;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var velocity = rb.velocity;
        if (velocity.sqrMagnitude <= maxSpeed * maxSpeed)
            return;
        
        var targetVelocity = velocity.normalized * maxSpeed;
        var velocityDelta = targetVelocity - velocity;

        var velocityDecrease = Mathf.Min(velocityDelta.magnitude, deceleration * Time.deltaTime);

        rb.velocity += velocityDelta.normalized * velocityDecrease;
    }
}
