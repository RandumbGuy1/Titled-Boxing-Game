using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRope : MonoBehaviour
{
    [SerializeField] float segmentMass;
    [SerializeField] float strength;
    [SerializeField] float dampening;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] bool useChildren = false;
    [SerializeField] Transform[] points = new Transform[0];
    SpringToZero[] springs;

    void Start()
    {
        lineRenderer.positionCount = points.Length;

        if (useChildren)
        {
            points = new Transform[transform.childCount];
            lineRenderer.positionCount = points.Length;

            for (int i = 0; i < transform.childCount; i++) points[i] = transform.GetChild(i);
        }

        springs = new SpringToZero[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            SpringToZero spring = points[i].GetComponent<SpringToZero>();
            if (spring) springs[i] = spring;

            Rigidbody rb = points[i].GetComponent<Rigidbody>();
            rb.mass = segmentMass;
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
            if (springs[i]) springs[i].UpdateSpring(strength, dampening);
        }
    }
}
