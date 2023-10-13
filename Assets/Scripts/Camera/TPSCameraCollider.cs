using UnityEngine;

[System.Serializable]
public class TPSCameraCollider
{
    [Header("Collider Settings")]
    [SerializeField] private LayerMask environment;
    [SerializeField] private bool enabled;
    [SerializeField] private float normalDistance;
    [SerializeField] private float radius;
    [SerializeField] private float pullDampening;
    private float desiredPull = 0f;
    private float vel = 0f;

    public bool Enabled { get => enabled; set { enabled = value; } }
    public float SmoothPull { get; private set; }

    public void ColliderUpdate(Vector3 curPos, Vector3 targetPos)
    {
        //If You dont want the third person camera, just center the camera and ignore the rest
        if (!enabled)
        {
            SmoothPull = Mathf.SmoothDamp(SmoothPull, 0f, ref vel, pullDampening);
            return;
        }

        //Find the nearest point to the player that isnt intersecting with the environment
        Vector3 targetToCam = (curPos - targetPos).normalized;
        desiredPull = Physics.SphereCast(targetPos, radius, targetToCam, out var hit, normalDistance, environment)
            ? Vector3.Distance(hit.point, targetPos) : normalDistance;

        SmoothPull = Mathf.SmoothDamp(SmoothPull, desiredPull, ref vel, pullDampening);
    }

    public void OnDrawGizmosSelected(Vector3 curPos)
    {
        Gizmos.DrawWireSphere(curPos, radius);
    }
}
