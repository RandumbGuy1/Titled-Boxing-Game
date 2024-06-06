using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashMaterial : MonoBehaviour
{
    [SerializeField] bool usePlayerColor;
    [SerializeField] Color gloveColor = Color.white;
    [SerializeField] float smoothTime;
    [SerializeField] MeshRenderer meshRenderer;

    void Update()
    {
        meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, usePlayerColor ? GameManager.Instance.GloveColor * 1f : gloveColor * 1f, Time.deltaTime * smoothTime);
    }

    public void Flash(Color color)
    {
        meshRenderer.material.color = color * 4f;
    }
}
