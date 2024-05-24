using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveCollision : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField] private PunchSide side;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] ParticleSystem hitSpark;
    [SerializeField] SphereCollider gloveCollider;
    [SerializeField] int punchDamage;
    [SerializeField] float punchStun;
    [SerializeField] float punchForce;
    [SerializeField] AudioClip[] punchClips;

    [Header("Attack Settings")]
    [SerializeField] float staminaCost;
    [SerializeField] float punchRange;
    [SerializeField] float punchForwardTime;
    [SerializeField] float punchBackTime;
    [SerializeField] float punchDelay;
    [SerializeField] AudioClip[] throwClips;

    [Header("Glove GFX Settings")]
    [SerializeField] FlashMaterial flash;
    [SerializeField] Vector3 startRot;
    [SerializeField] Quaternion hitRotStraight;
    [SerializeField] Quaternion hitRotHook;
    [SerializeField] Transform gfx;
    [SerializeField] ParticleSystem smoke;
    private float overExtendDelay = 1f;
    bool hitSomething = false;

    //Data to modify during runtime
    float punchElapsed;
    private PunchType type;

    public bool Active { get; set; } = false;
    public float StaminaCost => staminaCost;
    public bool CanPunch => punchElapsed >= punchBackTime + punchDelay * overExtendDelay;

    Vector3 lastPos = Vector3.zero;
    public void DetectCollisions(Transform thrower)
    {
        Vector3 gloveTravel = transform.position - lastPos;
        lastPos = transform.position;

        if (!Active || !Physics.SphereCast(transform.position - gloveTravel * 5f, 0.75f, gloveTravel, out var hit, gloveTravel.magnitude * 5f, hitLayer)) return;
        if (hit.transform.IsChildOf(thrower)) return;

        BoxingController boxer = hit.collider.GetComponent<BoxingController>();
        if (boxer != null)
        {
            if (boxer.Movement.RollInvincible) return;
            if (boxer.Movement.SlipInvincibleleft && side == PunchSide.Left) return;
            if (boxer.Movement.SlipInvincibleright && side == PunchSide.Right) return;

            float damageMulti = 1f;

            float sqrDistanceToPunch = (transform.position - thrower.position).sqrMagnitude;
            if (type == PunchType.Straight) damageMulti *= sqrDistanceToPunch / (punchRange * punchRange);
            if (type == PunchType.Hook) damageMulti *= punchRange / sqrDistanceToPunch;

            damageMulti = Mathf.Clamp(damageMulti, 0.25f, 1.75f);

            switch (boxer.AttackState)
            {
                case BoxerAttackState.Punching:
                    boxer.Health.Damage(punchDamage * 1.25f * damageMulti);
                    boxer.Health.Counter();
                    boxer.Movement.Stun(punchStun);
                    break;

                case BoxerAttackState.Blocking:
                    boxer.Health.Damage(punchDamage * 0.25f * damageMulti);
                    break;
                default:
                    boxer.Health.Damage(punchDamage * damageMulti);
                    boxer.Movement.Stun(punchStun);
                    break;
            }
        }

        Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(-hit.normal * punchForce, ForceMode.Impulse);
            rb.AddTorque(-hit.normal * punchForce * 3.5f, ForceMode.Impulse);
        }

        AudioManager.Instance.PlayOnce(punchClips, transform.position);
        if (smoke) smoke.Stop();

        Instantiate(hitSpark, hit.point, Quaternion.identity);

        SetGlove(false);
        endPunchPos = transform.position;
        hitSomething = true;
    }

    Vector3 endPunchPos = Vector3.zero;
    public void HandleGloves(Transform handPosition, Vector3 forward)
    {
        if (rb)
        {
            Active = false;
            return;
        }

        punchElapsed += Time.deltaTime;
        overExtendDelay = Mathf.Max(1f, overExtendDelay -= Time.deltaTime);

        if (Active)
        {
            if (punchElapsed >= punchForwardTime)
            {
                if (hitSomething) hitSomething = false;
                else overExtendDelay = 10f;

                SetGlove(false);
                return;
            }

            endPunchPos = handPosition.position + (type == PunchType.Straight ? 1f : 0.75f) * punchRange * forward;
            gfx.localRotation = Quaternion.SlerpUnclamped(gfx.localRotation, type == PunchType.Straight ? hitRotStraight : hitRotHook, EaseInOutBack(punchElapsed / punchForwardTime));
            transform.position = Vector3.LerpUnclamped(handPosition.position, endPunchPos, EaseInQuad(punchElapsed / punchForwardTime));
            return;
        }

        transform.position = Vector3.Lerp(endPunchPos, handPosition.position, punchElapsed / punchBackTime);
        gfx.localRotation = Quaternion.Slerp(gfx.localRotation, Quaternion.Euler(startRot), EaseInQuad(punchElapsed / punchForwardTime));
    }

    public void SetGlove(bool active, PunchType type = PunchType.Straight, Color flashColor = default)
    {
        punchElapsed = -0.01f;
        this.type = type;

        endPunchPos = transform.position;
        Active = active;

        if (!active) return;

        AudioManager.Instance.PlayOnce(throwClips, transform.position);
        if (smoke) smoke.Play();
        if (flash) flash.Flash(flashColor);
    }

    Transform prevParent = null;
    Rigidbody rb = null;
    public void Ragdoll(bool active = false)
    {
        if (!active)
        {
            if (rb) return;

            prevParent = transform.parent;
            transform.parent = null;
            gloveCollider.isTrigger = false;
            rb = gameObject.AddComponent<Rigidbody>();
            rb.drag = 1f;
            return;
        }

        gloveCollider.isTrigger = true;
        Destroy(rb);
        rb = null;
        transform.parent = prevParent;
    }

    float EaseInQuad(float x)
    {
        return x * x * x;
    }

    float EaseOutSine(float x) {
        return Mathf.Sin((x* Mathf.PI) / 2);
    }

    float EaseInOutBack(float x) {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
          ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
          : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x* 2 - 2) + c2) + 2) / 2;
        }
}

public enum PunchType
{
    Straight,
    Hook,
}

public enum PunchSide
{
    Left,
    Right,
}
