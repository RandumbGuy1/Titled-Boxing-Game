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
    private float overExtendDelay = 1f;
    bool hitSomething = false;

    //Data to modify during runtime
    float punchElapsed;

    public bool Active { get; set; } = false;
    public bool CanPunch => punchElapsed >= punchBackTime + punchDelay * overExtendDelay;

    public void DetectCollisions(Transform thrower)
    {
        Vector3 gloveTravel = transform.position - thrower.position;
        if (Active && Physics.SphereCast(thrower.position, gloveCollider.radius * 0.5f, gloveTravel, out var hit, gloveTravel.magnitude, hitLayer))
        {
            PlayerRef player = hit.collider.gameObject.GetComponent<PlayerRef>();
            if (player != null && player.PlayerMovement.Crouching) return;

            AudioManager.Instance.PlayOnce(punchClips, transform.position);
            Instantiate(hitSpark, hit.point, Quaternion.identity);

            SetGlove(false);
            endPunchPos = transform.position;
            hitSomething = true;

            Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(-hit.normal * punchForce, ForceMode.Impulse);
            
            IBoxer boxer = hit.collider.GetComponent<IBoxer>();
            if (boxer != null)
            {
                if (boxer.Punching)
                {
                    boxer.Health.Damage(punchDamage * 1.25f);
                    boxer.Health.Counter();
                    return;
                }

                if (boxer.Block.Blocking)
                {
                    boxer.Health.Damage(punchDamage * 0.25f);
                    return;
                }

                boxer.Health.Damage(punchDamage);
                boxer.Stun.Stun((hit.point - thrower.position).normalized, punchStun);
            }
        }
    }

    Vector3 endPunchPos = Vector3.zero;
    public void HandleGloves(Transform handPosition, Vector3 forward)
    {
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

            endPunchPos = handPosition.position + forward * punchRange;

            transform.position = Vector3.Lerp(handPosition.position, endPunchPos, EaseInQuad(punchElapsed / punchForwardTime));
            return;
        }

        transform.position = Vector3.Lerp(endPunchPos, handPosition.position, punchElapsed / punchBackTime);
    }

    public void SetGlove(bool active = true, float punchElapsed = 0f, StaminaController stamina = null)
    {
        this.punchElapsed = punchElapsed;
        endPunchPos = transform.position;
        Active = active;

        if (active && stamina != null)
        {
            stamina.TakeStamina(staminaCost);
        }
    }

    float EaseInQuad(float x)
    {
        return x * x;
    }
}
