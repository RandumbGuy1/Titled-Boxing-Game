using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveCollision : MonoBehaviour
{
    [Header("Collision")]
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
    [SerializeField] Vector3 startRot;
    [SerializeField] Quaternion hitRotStraight;
    [SerializeField] Quaternion hitRotHook;
    [SerializeField] Transform gfx;
    [SerializeField] ParticleSystem smoke;
    private float overExtendDelay = 1f;
    bool hitSomething = false;
    bool straightPunch = false;

    //Data to modify during runtime
    float punchElapsed;

    public bool Active { get; set; } = false;
    public bool CanPunch => punchElapsed >= punchBackTime + punchDelay * overExtendDelay;

    Vector3 lastPos = Vector3.zero;
    public void DetectCollisions(Transform thrower)
    {
        //Detection
        Vector3 gloveTravel = transform.position - lastPos;
        if (Active && Physics.SphereCast(transform.position - gloveTravel * 5f, gloveCollider.radius, gloveTravel, out var hit, gloveTravel.magnitude * 5f, hitLayer))
        {
            //Damage Logic
            if (hit.transform.IsChildOf(thrower)) return;

            print(thrower + ": " + hit.collider);

            BoxingController boxer = hit.collider.GetComponent<BoxingController>();
            if (boxer != null)
            {
                if (boxer.Movement.Rolling || boxer.Movement.SlipDirection != 0 && straightPunch) return;

                switch (boxer.AttackState)
                {
                    case BoxerAttackState.Punching:
                        boxer.Health.Damage(punchDamage * 1.25f);
                        boxer.Health.Counter();
                        boxer.Movement.Stun(punchStun);
                        break;

                    case BoxerAttackState.Blocking:
                        boxer.Health.Damage(punchDamage * 0.25f);
                        break;
                    default:
                        boxer.Health.Damage(punchDamage);
                        boxer.Movement.Stun(punchStun);
                        break;
                }
            }

            Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-hit.normal * punchForce, ForceMode.Impulse);
                rb.AddTorque(-hit.normal * punchForce, ForceMode.Impulse);
            }

            AudioManager.Instance.PlayOnce(punchClips, transform.position);
            if (smoke) smoke.Stop();

            Instantiate(hitSpark, hit.point, Quaternion.identity);

            SetGlove(false);
            endPunchPos = transform.position;
            hitSomething = true;
        }

        lastPos = transform.position;
    }

    Vector3 endPunchPos = Vector3.zero;
    public void HandleGloves(Transform handPosition, Vector3 forward)
    {
        //Return if physics enabled
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
                else overExtendDelay = 5f;

                SetGlove(false);
                return;
            }

            endPunchPos = handPosition.position + forward * punchRange * (straightPunch ? 1f : 0.5f);
            gfx.localRotation = Quaternion.Slerp(gfx.localRotation, straightPunch ? hitRotStraight : hitRotHook, EaseInQuad(punchElapsed / punchForwardTime));
            transform.position = Vector3.Lerp(handPosition.position, endPunchPos, EaseInQuad(punchElapsed / punchForwardTime));
            return;
        }

        transform.position = Vector3.Lerp(endPunchPos, handPosition.position, punchElapsed / punchBackTime);
        gfx.localRotation = Quaternion.Slerp(gfx.localRotation, Quaternion.Euler(startRot), EaseInQuad(punchElapsed / punchForwardTime));
    }

    public void SetGlove(bool active = true, float punchElapsed = 0f, StaminaController stamina = null, bool straightPunch = false)
    {
        this.punchElapsed = punchElapsed;
        this.straightPunch = straightPunch;

        endPunchPos = transform.position;
        Active = active;

        if (!active || stamina == null) return;

        stamina.TakeStamina(staminaCost);
        AudioManager.Instance.PlayOnce(throwClips, transform.position);
        if (smoke) smoke.Play();
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
        return x * x;
    }
}
