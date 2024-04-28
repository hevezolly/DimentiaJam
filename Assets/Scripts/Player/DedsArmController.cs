using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class DedsArmController : MonoBehaviour
{
    private enum WheelType {
        Left = 0,
        Right = 1,
    }

    [SerializeField] private Vector2 wheelCenter;
    [SerializeField] private ScriptableValueField<float> wheelRadius;
    [SerializeField] private WheelType wheelType;
    [SerializeField] private Transform wheelArm;

    [SerializeField] private ScriptableValueField<AnimationCurve> frictionCurve;
    [SerializeField] private ScriptableValueField<float> maxSpeed;
    [SerializeField] private ScriptableValueField<float> groundWheelFrictionCoefficient;
    [SerializeField] private ScriptableValueField<float> wheelRotationFrictionCoefficient;
    [SerializeField] private ScriptableValueField<float> _mouseSensitivity;
    [SerializeField] private ScriptableValueField<bool> GrabState;
    [SerializeField] private ScriptableValueField<float> HandPosition;
    [SerializeField] private ScriptableValueField<float> RevolutionsCount;

    private Rigidbody2D rb;

    private float MoveVel;

    private Vector2 cameraWorldSize;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody2D>();

        float aspect = (float)Screen.width / Screen.height;
        cameraWorldSize = new Vector2();
        cameraWorldSize.y = Camera.main.orthographicSize * 2;
        
        cameraWorldSize.x = cameraWorldSize.y * aspect;

    }

    private void Start()
    {
        PlaceArm(0.5f);
        GrabState.Value = false;
        RevolutionsCount.Value = 0;
    }

    private void Update()
    {
        GrabState.Value = Input.GetMouseButton((int)wheelType);
        
        var mouseScreen = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseScreen *= _mouseSensitivity;

        MoveVel = Vector2.Dot(mouseScreen, transform.up);

        var forward = (Vector2)transform.up;
        var newPos = Mathf.Clamp01(HandPosition + MoveVel);
        var wheelDelta = ((newPos - HandPosition) * Mathf.PI * wheelRadius);
        var wheelSpeed = wheelDelta / Time.deltaTime;
        if (GrabState) 
        {
            var targetVelocity = forward * wheelSpeed;
            RevolutionsCount.Value += wheelDelta * 0.5f;
            var velocity = PointVelocity() - targetVelocity;            

            ApplyForce(velocity);
        }
        else 
        {
            var velocity = PointVelocity();
            var alongVelocityCoefficient = Vector2.Dot(forward, velocity);
            var alongVelocity = alongVelocityCoefficient * forward;
            RevolutionsCount.Value += alongVelocityCoefficient * 2 * Mathf.PI * wheelRadius * Time.deltaTime;
            var prepVelocity = velocity -  alongVelocity;
            var frictionVelocity = prepVelocity * GetFrictionCoefficient(prepVelocity.magnitude);
            var resistanceFriction = alongVelocity * wheelRotationFrictionCoefficient;
            ApplyForce(frictionVelocity, resistanceFriction);
        }

        PlaceArm(newPos);
    }

    private void ApplyForce(Vector2 frictionComponent, Vector2 resistanceComponent = default) 
    {
        var friction = frictionComponent * GetFrictionCoefficient(frictionComponent.magnitude);
        var resistance = resistanceComponent * wheelRotationFrictionCoefficient;

        var maximalFriction = frictionComponent.magnitude / Time.deltaTime;
        if (friction.magnitude > maximalFriction)
            friction = friction.normalized * maximalFriction;

        rb.AddForceAtPosition(-(friction + resistance) * rb.mass, transform.TransformPoint(wheelCenter));
    }

    private float GetFrictionCoefficient(float speed) 
    {
        // return groundWheelFrictionCoefficient;
        var t = Mathf.Clamp01(Mathf.Abs(speed) / maxSpeed);
        var result = frictionCurve.Value.Evaluate(t) * groundWheelFrictionCoefficient;
        return result;
    }

    private Vector2 PointVelocity() 
    {
        var point = transform.TransformPoint(wheelCenter);
        var velocity = rb.GetPointVelocity(point);
        return velocity;
    }

    private void PlaceArm(float t) 
    {
        HandPosition.Value = t;
        var position2D = Vector2.Lerp(wheelCenter - Vector2.up * wheelRadius.Value, wheelCenter + Vector2.up * wheelRadius.Value, t);
        wheelArm.transform.localPosition = new Vector3(position2D.x, position2D.y, wheelArm.transform.localPosition.z);
    }

    private void OnValidate()
    {

        if (Application.isPlaying)
            return;
        if (wheelArm == null)
            return;
        PlaceArm(0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wheelCenter - Vector2.up * wheelRadius.Value, 
            wheelCenter + Vector2.up * wheelRadius.Value);
    }
}
