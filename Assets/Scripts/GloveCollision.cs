using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveCollision : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] ParticleSystem hitSpark;
    [SerializeField] SphereCollider gloveCollider;
    [SerializeField] float punchForce;

    [Header("Attack Settings")]
    [SerializeField] float punchRange;
    [SerializeField] float punchForwardTime;
    [SerializeField] float punchBackTime;
    [SerializeField] float punchDelay;

    //Data to modify during runtime
    float punchElapsed;

    public bool Active { get; set; } = false;
    public bool CanPunch => punchElapsed >= punchBackTime + punchDelay;

    public void DetectCollisions(Transform thrower)
    {
        Vector3 gloveTravel = transform.position - thrower.position;
        if (Active && Physics.SphereCast(thrower.position, gloveCollider.radius * 0.5f, gloveTravel, out var hit, gloveTravel.magnitude, hitLayer))
        {
            PlayerRef player = hit.collider.gameObject.GetComponent<PlayerRef>();
            if (player != null)
            {
                if (player.PlayerMovement.Crouching) return;

                player.PlayerMovement.Rb.AddForce(-hit.normal * punchForce, ForceMode.Impulse);
                Instantiate(hitSpark, transform.position, Quaternion.identity);
                punchElapsed = Mathf.Infinity;
                return;
            }

            Instantiate(hitSpark, transform.position, Quaternion.identity);
            punchElapsed = Mathf.Infinity;

            Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
            if (rb == null) return;

            rb.AddForce(-hit.normal * punchForce, ForceMode.Impulse);
        }
    }

    Vector3 endPunchPos = Vector3.zero;
    public void HandleGloves(Transform handPosition, Vector3 forward)
    {
        punchElapsed += Time.deltaTime;

        if (Active)
        {
            if (punchElapsed >= punchForwardTime)
            {
                punchElapsed = 0f;
                Active = false;
                endPunchPos = transform.position;
                return;
            }

            endPunchPos = handPosition.position + forward * punchRange;

            transform.position = Vector3.Lerp(handPosition.position, endPunchPos, EaseInQuad(punchElapsed / punchForwardTime));
            return;
        }

        transform.position = Vector3.Lerp(endPunchPos, handPosition.position, punchElapsed / punchBackTime);
    }

    public void SetGlove(bool active = true, float punchElapsed = 0f)
    {
        this.punchElapsed = punchElapsed;
        Active = active;
    }

    float EaseInQuad(float x)
    {
        return x * x;
    }
}
