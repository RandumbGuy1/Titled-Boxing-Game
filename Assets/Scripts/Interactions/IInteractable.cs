using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInteractable
{
    public PlayerRef Player { get; set; }
    public GameObject GameObject { get; }

    [SerializeField] Outline selectedOutline;
    public abstract string GetDescription(PlayerRef player);
    public abstract void OnInteract(PlayerRef player);
    public abstract void OnStartHover(PlayerRef player);
    public abstract void OnEndHover(PlayerRef player);

    public void OutlineHoverStart()
    {
        selectedOutline.enabled = true;
    }

    public void OutlineHoverEnd()
    {
        selectedOutline.enabled = false;
    }
}
