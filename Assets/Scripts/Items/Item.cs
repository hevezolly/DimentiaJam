using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour
{
    [SerializeField] private Pose holdPose;
    public Pose HoldPose => holdPose;
    protected ItemHolder Holder {get; private set;}
    public bool IsHeld => Holder != null;
    public bool CanBePickedUp {get; private set;} = true;

    private SpriteRenderer[] renderers;

    private Rigidbody2D rb;

    private Coroutine cantPickUp;

    private static WaitForSeconds waitForPickUp = new WaitForSeconds(2);

    private int shiftedLayer = 0;

    protected virtual void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void PickUp(ItemHolder holder) 
    {
        rb.isKinematic = true;
        ShiftOrder(10);
        Holder = holder;
        OnPickUp();
    }

    private void ShiftOrder(int shift) 
    {
        if (renderers == null)
            return;
        shiftedLayer += shift;
        foreach (var r in renderers)
            r.sortingOrder += shift;
    }

    public void Drop() 
    {
        rb.isKinematic = false;
        rb.velocity = Holder.rb.GetPointVelocity(rb.position);
        rb.angularVelocity = Holder.rb.angularVelocity;
        rb.AddForce(Holder.transform.up * rb.mass, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-10, 10) * rb.mass);
        RunCantPickUp();
        OnDrop();
        Holder = null;
    }

    private void RunCantPickUp() 
    {
        if (cantPickUp != null)
            StopCoroutine(cantPickUp);
        cantPickUp = StartCoroutine(CantPickUpCoroutine());
    }
    
    private IEnumerator CantPickUpCoroutine() 
    {
        CanBePickedUp = false;
        yield return waitForPickUp;
        ShiftOrder(-shiftedLayer);
        CanBePickedUp = true;

        cantPickUp = null;
    }

    protected virtual void OnDrop() {}

    protected virtual void OnPickUp() {}

    protected virtual void OnItemDestroyed() {}

    protected virtual void OnDrawGizmos()
    {
        var matrix = Gizmos.matrix;
        var color = Gizmos.color;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
    
        var forward = holdPose.rotation * Vector3.up;
        var right = holdPose.rotation * Vector3.right;
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(holdPose.position, forward);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(holdPose.position, right);

        Gizmos.color = color;
        Gizmos.matrix = matrix;
    }
}

public interface IUsable 
{
    bool CanBeUsed {get;}
    void Use();
}

public interface IConsumable 
{
    bool CanBeConsumed {get;}
    void Consume();
}

public class ItemHolder 
{
    public readonly Rigidbody2D rb;
    public readonly ItemHandle itemHandle;
    public GameObject gameObject => itemHandle.gameObject;
    public Transform transform => itemHandle.transform;

    public ItemHolder(Rigidbody2D rb, ItemHandle handle)
    {
        this.rb = rb;
        this.itemHandle = handle;
    }
}
