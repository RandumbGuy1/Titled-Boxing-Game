using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    [Header("Lock On Settings")]
    [SerializeField] private float lockOnRadius;
    [SerializeField] private Transform lockOnTarget;
    [SerializeField] private PlayerRef player;
    [SerializeField] private LayerMask lockOnLayer;
    private Outline currOutline = null;

    [Header("Outline Settings")]
    [SerializeField] private float thickness;
    [SerializeField] private Color color;

    public Transform LockOnTarget => lockOnTarget;

    private void Awake()
    {
        player.PlayerInput.OnLockOnInput += DetectLockOn;

        if (lockOnTarget != null)
        {
            currOutline = lockOnTarget.gameObject.AddComponent<Outline>();
            currOutline.OutlineMode = Outline.Mode.OutlineVisible;
            currOutline.OutlineColor = color;
            currOutline.OutlineWidth = thickness;
        }
    }

    private void DetectLockOn(bool pressed)
    {
        if (!pressed) return;

        if (lockOnTarget != null)
        {
            lockOnTarget = null;
            Destroy(currOutline);
            return;
        }

        if (Physics.SphereCast(player.Orientation.position, lockOnRadius, player.Orientation.forward, out var hit, 10f, lockOnLayer))
        {
            lockOnTarget = hit.collider.transform;
            currOutline = lockOnTarget.gameObject.AddComponent<Outline>();
            currOutline.OutlineMode = Outline.Mode.OutlineVisible;
            currOutline.OutlineColor = color;
            currOutline.OutlineWidth = thickness;
            return;
        }

        lockOnTarget = null;
    }
}
