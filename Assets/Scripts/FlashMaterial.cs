using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashMaterial : MonoBehaviour
{
    [SerializeField] float smoothTime;
    [SerializeField] MeshRenderer meshRenderer;

    void Update()
    {
        meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, Color.white *1f, Time.deltaTime * smoothTime);
    }

    public void Flash(Color color)
    {
        meshRenderer.material.color = color * 5f;
    }
}
