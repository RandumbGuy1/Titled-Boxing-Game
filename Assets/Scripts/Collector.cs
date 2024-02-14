using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] LayerMask pickupLayer;
    [SerializeField] float pickupRange;

    void Update()
    {
        if (Physics.SphereCast(transform.position, pickupRange, Vector3.down, out var hit, 1f, pickupLayer))
        {
            Collectable collect = hit.collider.GetComponent<Collectable>();
            if (collect == null) return;

            collect.Pickup();
        }
    }
}
