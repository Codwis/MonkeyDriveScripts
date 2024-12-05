using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfScript : Interactable
{
    public int ShelfNum;
    public Transform BoxSpot;
    public bool Correct = false;
    public WarehouseObject CurrentObject { get; private set; }

    public override void Interact(Transform source)
    {
        base.Interact(source);
        if(CurrentObject != null)
        {
            source.GetComponent<MovementController>().Pickup(CurrentObject);
            CurrentObject = null;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.TryGetComponent<WarehouseObject>(out WarehouseObject obj))
        {
            if (CurrentObject != null) return;

            var t = obj.GetComponentInParent<MovementController>();
            if (t != null) t.Drop();

            CurrentObject = obj;
            CurrentObject.Place(BoxSpot);

            if(CurrentObject.ShelfNum == ShelfNum)
            {
                Correct = true;
                WarehouseHandler.instance.CheckForCorrect();
            }
        }
    }
}
