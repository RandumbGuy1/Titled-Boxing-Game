using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloves : MonoBehaviour
{
    [SerializeField] Transform[] handPositions = new Transform[0];
    [SerializeField] Rigidbody[] gloves = new Rigidbody[0];
    [SerializeField] float[] punchCooldownTimes = new float[0];

    float[] punchCooldowns = new float[2];
    [SerializeField] float[] punchForces = new float[0];
    [SerializeField] float handSpeed;
    [SerializeField] PlayerRef player;

    void Start()
    {
        player.PlayerInput.OnMouseButtonDownInput += HandlePunching;
    }

    void HandlePunching(int button)
    {
        for (int i = 0; i < punchCooldowns.Length && i < punchCooldownTimes.Length; i++)
        {
            punchCooldowns[i] -= Time.deltaTime;
        }

        if (button >= 2 || button < 0) return;
        if (punchCooldowns[button] > 0f) return;
         
        gloves[button].velocity = Vector3.zero;
        gloves[button].AddForce(player.Orientation.forward * punchForces[button], ForceMode.Impulse);
        punchCooldowns[button] = punchCooldownTimes[button];

    }

    void FixedUpdate()
    {
        for (int i = 0; i < handPositions.Length && i < gloves.Length; i++)
        {
            Vector3 followVel = (handPositions[i].position - gloves[i].transform.position);
            followVel = Vector3.ClampMagnitude(followVel, 50f);

            gloves[i].drag = (gloves[i].velocity.magnitude / 10f) / Mathf.Clamp((handPositions[i].position - gloves[i].transform.position).sqrMagnitude * 0.5f, 0.05f, 0.6f);

            gloves[i].AddForce(0.1f * handSpeed * followVel, ForceMode.VelocityChange);
        }
    }
}
