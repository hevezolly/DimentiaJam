using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ItemHandle : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Transform itemHolder;

    private Item heldItem;
    private IUsable usable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void DestryHeldItem() 
    {
        if (heldItem == null)
            return;
        Destroy(heldItem.gameObject);
        ClearItem();
    }

    private void Update()
    {
        var scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        if (heldItem == null)
            return;
        if (Input.GetMouseButtonDown(2)) 
        {
            if (usable?.CanBeUsed == true)
                usable.Use();
        }

        if (scrollAxis > 0.025f) 
        {
            DropHeldItem();
        }
    }

    private void ClearItem() 
    {
        heldItem = null;
        usable = null;
    }

    public void DropHeldItem() 
    {
        heldItem.transform.parent = null;
        heldItem.Drop();
        ClearItem();
    }

    private void PickUpItem(Item item) 
    {
        heldItem = item;
        usable = item as IUsable;
        item.transform.parent = itemHolder;
        var rotation = Quaternion.Inverse(item.HoldPose.rotation);
        var offset = rotation * item.HoldPose.position;
        item.transform.localPosition = -offset;
        item.transform.localRotation = rotation;
        item.PickUp(new ItemHolder(rb, this));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var item = other.GetComponentInParent<Item>(); 
        if (item == null)
            return;

        if (item.IsHeld)
            return;

        if (heldItem != null)
            return;
        
        if (!item.CanBePickedUp)
            return;

        PickUpItem(item);
    }
}
