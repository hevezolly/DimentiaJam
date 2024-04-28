using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : Item, IUsable
{
    [SerializeField] private Vector2 recoilMinMax;
    [SerializeField] private float recoilAngle;
    [SerializeField] Transform muzzleFlashPosition;
    [SerializeField] Pool MuzzleFlash;
    [SerializeField] private ScriptableEvent<float> screenShakeEvent;
    [SerializeField] private ScriptableValueField<float> screenShakeStrength;
    [SerializeField] private ScriptableValueField<float> damageAngle;
    [SerializeField] private ScriptableValueField<float> damageDistance;
    [SerializeField] private ScriptableValueField<float> damage;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private ScriptableValueField<float> reloadTime;

    private float lastShootTime = 0; 
    public bool CanBeUsed => lastShootTime + reloadTime < Time.time;

    public void Use()
    {
        Shoot();
    }

    private void Shoot() 
    {
        MuzzleFlash.Get<PooledItem>(muzzleFlashPosition.position, muzzleFlashPosition.rotation);
        var recoilStrength = Random.Range(recoilMinMax.x, recoilMinMax.y);
        var angle = Random.Range(-recoilAngle, recoilAngle) * 0.5f;
        var vector = Quaternion.AngleAxis(angle, Vector3.forward) * -transform.up;

        Holder.rb.AddForceAtPosition(vector * recoilStrength, muzzleFlashPosition.position, ForceMode2D.Impulse);
        screenShakeEvent.Trigger(screenShakeStrength);
        lastShootTime = Time.time;
        CheckDamage();
    }

    private void CheckDamage() 
    {
        var halfAngle = damageAngle / 2f;

        var halfSide = Mathf.Tan(Mathf.Deg2Rad * halfAngle) * damageDistance;
        var enemies = Physics2D.OverlapBoxAll(muzzleFlashPosition.position + transform.up * damageDistance, 
            new Vector2(halfSide * 2, damageDistance), transform.rotation.eulerAngles.y, enemyLayer);
        
        foreach (var e in enemies) 
        {
            if (!e.TryGetComponent(out AIManager ai))
                continue;
            var angle = Vector2.Angle(transform.up, e.transform.position - Holder.transform.position);
            var distance = Vector2.Dot(transform.up, e.transform.position - muzzleFlashPosition.position);


            var angleCoefficient = 1 - Mathf.Clamp01(angle / halfAngle);
            var distanceCoefficient = 1 - Mathf.Clamp01(distance / damageDistance);

            var damageValue = Mathf.Max(angleCoefficient, distanceCoefficient) * damage;

            Debug.LogAssertion(damageValue);
            ai.Damage(damageValue);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;

        var far = muzzleFlashPosition.position + transform.up * damageDistance;
        var halfAngle = damageAngle / 2f;

        var halfSide = Mathf.Tan(Mathf.Deg2Rad * halfAngle) * damageDistance;

        Gizmos.DrawLine(muzzleFlashPosition.position, far + transform.right * halfSide);
        Gizmos.DrawLine(muzzleFlashPosition.position, far - transform.right * halfSide);
        Gizmos.DrawLine(far + transform.right * halfSide, far - transform.right * halfSide);
    }
}
