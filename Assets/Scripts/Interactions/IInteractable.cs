using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class IInteractable : MonoBehaviour
{
    public abstract PlayerRef Player { get; }

    [SerializeField] TextMeshPro interactText;
    [SerializeField] Outline selectedOutline;
    [SerializeField] float hoverAmplitude;
    [SerializeField] float hoverSpeed;

    public abstract string GetDescription(Player player);
    public abstract void OnInteract(Player player);
    public abstract void OnStartHover(Player player);
    public abstract void OnEndHover(Player player);

    public TextMeshPro InteractText => interactText;

    Vector3 localPosition;
    void Start()
    {
        localPosition = transform.localPosition;
    }

    float elapsed = 0f;
    void Update()
    {
        elapsed += Time.deltaTime * hoverSpeed;
        transform.localPosition = hoverAmplitude * Mathf.Sin(elapsed) * Vector3.up + localPosition;

    }

    public void OutlineHoverStart()
    {
        interactText.enabled = true;
        selectedOutline.enabled = true;
    }

    public void OutlineHoverEnd()
    {
        interactText.enabled = false;
        selectedOutline.enabled = false;
    }
}
