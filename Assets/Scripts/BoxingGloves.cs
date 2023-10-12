using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloves : MonoBehaviour
{
    [SerializeField] Transform[] handPositions = new Transform[2];
    [SerializeField] GloveCollision[] gloves = new GloveCollision[2];

    [SerializeField] PlayerRef player;

    void Start()
    {
        player.PlayerInput.OnMouseButtonDownInput += HandlePunching;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
        {
            gloves[i].DetectCollisions(transform);
        }
    }

    void HandlePunching(int button)
    {
        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
        {
            gloves[i].HandleGloves(handPositions[i], player.Orientation.forward);
        }

        if (button >= 2 || button < 0 || !gloves[button].CanPunch) return;

        gloves[button].SetGlove(true);
    }
}
