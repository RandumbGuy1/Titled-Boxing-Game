using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyHover : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] private LayerMask environment;
    [SerializeField] private float rideRayExtension;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    public bool Intersecting { get; private set; }

    [Header("Refrences")]
    [SerializeField] private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        void HoverOffGround(Vector3 bufferOffset)
        {
            Vector3 bufferPosition = transform.position + bufferOffset;
            bool buffer = Physics.Raycast(bufferPosition, Vector3.down, out var hit, rideHeight + rideRayExtension * 1.5f, environment);

            Intersecting = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out _, rideHeight + rideRayExtension, environment) || buffer;

            if (!buffer) return;

            Vector3 vel = rb.velocity;
            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;

            if (hitBody != null) otherVel = hitBody.velocity;

            float relVel = Vector3.Dot(Vector3.down, vel) - Vector3.Dot(Vector3.down, otherVel);
            float x = hit.distance - rideHeight;
            float springForce = (x * rideSpringStrength) - (relVel * rideSpringDamper);

            rb.AddForce(Vector3.down * springForce);

            if (hitBody != null) hitBody.AddForceAtPosition(Vector3.down * -springForce, hit.point);
        }

        HoverOffGround(Vector3.zero);
    }
}
