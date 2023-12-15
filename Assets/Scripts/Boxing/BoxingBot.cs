using UnityEngine;

public class BoxingBot : MonoBehaviour, IBoxer
{
    [Header("AI Settings")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] Transform[] handPositions = new Transform[2];
    [SerializeField] GloveCollision[] gloves = new GloveCollision[2];
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float standingRange;
    [SerializeField] private Transform target;

    private float[] elapsed = new float[2];
    private float[] randomTimes = new float[2];

    [Header("Interface")]
    [SerializeField] Damageable health;
    [SerializeField] StaminaController stamina;
    [SerializeField] StunController stun;
    [SerializeField] BlockController block;

    public bool CanPreformActions => !stamina.RanOutofStamina && !stun.InStun;
    public bool CanDash => !stamina.RanOutofStamina && !stun.InStun;

    public bool Punching
    {
        get
        {
            foreach (GloveCollision glove in gloves) if (glove.Active) return true;
            return false;
        }
    }

    public bool CanPunch
    {
        get
        {
            foreach (GloveCollision glove in gloves) if (!glove.CanPunch) return false;
            return true;
        }
    }

    public Damageable Health => health;
    public StaminaController Stamina => stamina;
    public StunController Stun => stun;
    public BlockController Block => block;

    void FixedUpdate()
    {
        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
        {
            gloves[i].DetectCollisions(transform);
            gloves[i].HandleGloves(handPositions[i], transform.forward);
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        Vector3 toTargetFromDistance = (target.position - toTarget.normalized * standingRange * 0.35f) - transform.position;

        rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(toTarget), Time.fixedDeltaTime * turnSpeed);

        if (!CanPreformActions) return;

        rb.MovePosition(transform.position + Vector3.ClampMagnitude(toTargetFromDistance, 4f) * Time.fixedDeltaTime * speed);

        if (toTarget.sqrMagnitude < standingRange * standingRange)
        {
            for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
            {
                elapsed[i] += Time.fixedDeltaTime;

                if (elapsed[i] > randomTimes[i] && CanPunch)
                {
                    elapsed[i] = 0f;
                    randomTimes[i] = Random.Range(0.2f, 1f);
                    gloves[i].SetGlove(true, 0f, stamina);
                }
            }
        } 
    }

    public void ResetGloves(float x)
    {
        foreach (GloveCollision glove in gloves) glove.SetGlove(false);
    }

    public void Disable()
    {
        rb.freezeRotation = false;
        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++) gloves[i].Ragdoll();
        enabled = false;
    }
}
