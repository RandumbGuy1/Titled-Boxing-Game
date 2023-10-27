using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothZeroRotation : MonoBehaviour
{
    [SerializeField] private Transform gfx;
    private float smoothTime = 0.1f;

    void Update()
    {
        gfx.localRotation = Quaternion.Slerp(gfx.localRotation, Quaternion.identity, Time.deltaTime * (1f / smoothTime) * 2f);
    }

    public void Rotate(Vector3 dir, float amount)
    {
        gfx.transform.up = dir;
        smoothTime = amount;
    }
}
