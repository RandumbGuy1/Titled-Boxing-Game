using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringToZero : MonoBehaviour
{
    [SerializeField] float strength;
    [SerializeField] float dampening;
    [SerializeField] Vector3 axis;
    [SerializeField] Rigidbody rb;

    Vector3 startPos;
    Quaternion startRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.localRotation;
    }

    public void UpdateSpring()
    {
        Vector3 toStart = startPos - transform.position;
        rb.AddForce((toStart * strength) - (rb.velocity * dampening));
        rb.AddTorque(axis * ((startRot.y - transform.rotation.y) * strength * 5f - (rb.angularVelocity.y * dampening)));
    }

    public void UpdateSpring(float strength, float dampening)
    {
        Vector3 toStart = startPos - transform.position;
        rb.AddForce((toStart * strength) - (rb.velocity * dampening));
        rb.AddTorque(axis * ((startRot.y - transform.rotation.y) * strength * 5f - (rb.angularVelocity.y * dampening)));
    }
}
