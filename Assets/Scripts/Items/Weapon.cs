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
    public bool CanBeUsed => true;

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
    }
}
