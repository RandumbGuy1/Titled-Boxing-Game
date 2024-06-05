using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInteractable : MonoBehaviour
{
    public abstract PlayerRef Player { get; }

    [SerializeField] Outline selectedOutline;
    public abstract string GetDescription(Player player);
    public abstract void OnInteract(Player player);
    public abstract void OnStartHover(Player player);
    public abstract void OnEndHover(Player player);

    public void OutlineHoverStart()
    {
        selectedOutline.enabled = true;
    }

    public void OutlineHoverEnd()
    {
        selectedOutline.enabled = false;
    }
}
