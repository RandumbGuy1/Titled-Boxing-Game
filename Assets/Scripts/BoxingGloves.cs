using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloves : MonoBehaviour
{
    [SerializeField] Transform[] handPositions = new Transform[0];
    [SerializeField] Transform[] gloves = new Transform[0];
    [SerializeField] float[] punchCooldownTimes = new float[0];

    Vector3[] startHandPositions = new Vector3[2];

    float[] punchCooldowns = new float[2];
    [SerializeField] float[] punchForces = new float[0];
    [SerializeField] float handSpeed;
    [SerializeField] PlayerRef player;

    void Start()
    {
        player.PlayerInput.OnMouseButtonDownInput += HandlePunching;

        for (int i = 0; i < handPositions.Length; i++)
        {
            startHandPositions[i] = handPositions[i].localPosition;
        }
    }

    void HandlePunching(int button)
    {
        for (int i = 0; i < punchCooldowns.Length && i < punchCooldownTimes.Length; i++)
        {
            punchCooldowns[i] -= Time.deltaTime;
        }

        if (button >= 2 || button < 0) return;
        if (punchCooldowns[button] > 0f) return;

        handPositions[button].localPosition += Vector3.forward * punchForces[button];
        punchCooldowns[button] = punchCooldownTimes[button];

    }

    void FixedUpdate()
    {
        for (int i = 0; i < handPositions.Length && i < gloves.Length; i++)
        {
            gloves[i].position = Vector3.Lerp(gloves[i].position, handPositions[i].position, Time.deltaTime * handSpeed);
            handPositions[i].localPosition = Vector3.Lerp(handPositions[i].localPosition, startHandPositions[i], Time.deltaTime * handSpeed * 0.5f);
        }
    }
}
