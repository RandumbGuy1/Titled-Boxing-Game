using UnityEngine;

public class LockOn : MonoBehaviour
{
    [Header("Lock On Settings")]
    [SerializeField] private float lockOnRadius;
    [SerializeField] private Transform lockOnTarget;
    [SerializeField] private BoxingController player;
    [SerializeField] private LayerMask lockOnLayer;
    [SerializeField] private StatDisplay display;
    private Outline currOutline = null;

    [Header("Outline Settings")]
    [SerializeField] private float thickness;
    [SerializeField] private Color color;
    public Transform LockOnTarget => lockOnTarget;

    private void Awake()
    {
        player.Keys.OnLockInput += DetectLockOn;

        if (lockOnTarget != null)
        {
            currOutline = lockOnTarget.gameObject.AddComponent<Outline>();
            currOutline.OutlineMode = Outline.Mode.OutlineVisible;
            currOutline.OutlineColor = color;
            currOutline.OutlineWidth = thickness;

            display.SetHealthAndStamina(lockOnTarget.gameObject);
        }
    }

    private void DetectLockOn(bool pressed)
    {
        if (!pressed) return;

        if (Physics.SphereCast(player.transform.position, lockOnRadius, player.Orientation.forward, out var hit, 15f, lockOnLayer))
        {
            if (lockOnTarget == hit.collider.transform)
            {
                lockOnTarget = null;
                display.SetHealthAndStamina(new GameObject());
                Destroy(currOutline);
                return;
            }

            if (currOutline != null) Destroy(currOutline);

            lockOnTarget = hit.collider.transform;
            currOutline = lockOnTarget.gameObject.AddComponent<Outline>();
            currOutline.OutlineMode = Outline.Mode.OutlineVisible;
            currOutline.OutlineColor = color;
            currOutline.OutlineWidth = thickness;

            display.SetHealthAndStamina(lockOnTarget.gameObject);

            return;
        }

        lockOnTarget = null;
        display.SetHealthAndStamina(new GameObject());
        Destroy(currOutline);
    }

    public RaycastHit[] SortRayHitByDist(RaycastHit[] hits)
    {
        var n = hits.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                float dist1 = (hits[j].point - transform.position).sqrMagnitude;
                float dist2 = (hits[j + 1].point - transform.position).sqrMagnitude;

                if (dist1 > dist2)
                {
                    var tempVar = hits[j];
                    hits[j] = hits[j + 1];
                    hits[j + 1] = tempVar;
                }
            }
        }

        return hits;
    }
}
