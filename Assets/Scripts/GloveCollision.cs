using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveCollision : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] ParticleSystem hitSpark;
    [SerializeField] float punchForce;
    [SerializeField] SphereCollider gloveCollider;

    public bool Active { get; set; } = false;
    public BoxingGloves GloveOwner { get; set; } = null;
    public int GloveIndex { get; set; } = -1;

    void OnTriggerEnter(Collider col)
    {
        if (!Active || GloveOwner == null || GloveIndex == -1) return;

        Instantiate(hitSpark, transform.position, Quaternion.identity);

        Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();

        GloveOwner.SetGloveInactive(GloveIndex);

        if (rb == null) return;

        rb.AddExplosionForce(punchForce, transform.position, 1f, gloveCollider.radius, ForceMode.Impulse);
    }
}
