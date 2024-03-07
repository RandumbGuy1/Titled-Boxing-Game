using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingController : MonoBehaviour, IBoxer
{
    public BoxerMoveState MoveState => movement.State;
    public BoxerAttackState AttackState { get; private set; }

    [SerializeField] Damageable health;
    [SerializeField] StaminaController stamina;
    [SerializeField] StunController stun;
    [SerializeField] BlockController block;

    [SerializeField] Transform[] handPositions = new Transform[2];
    [SerializeField] GloveCollision[] gloves = new GloveCollision[2];

    [SerializeField] BoxerMovement movement;
    [SerializeField] Transform orientation;

    public bool CanPreformActions => !movement.Rolling && !stamina.RanOutofStamina && !stun.InStun;
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
            if (block.JustStoppedBlocking || block.Blocking) return false;

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
        }
    }

    public void HandlePunching(FrameInput input)
    {
        int button = input.PunchInput;
        if (!enabled) return;

        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
        {
            gloves[i].HandleGloves(handPositions[i], orientation.forward);
        }

        if (button >= 2 || button < 0) return;
        if (!CanPunch || !CanPreformActions) return;

        gloves[button].SetGlove(true, 0f, stamina);
        movement.Rb.velocity *= 0f;
    }

    public void ResetGloves()
    {
        foreach (GloveCollision glove in gloves) glove.SetGlove(false);
    }

    public void Disable()
    {
        movement.enabled = false;
        movement.Rb.freezeRotation = false;

        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++) gloves[i].Ragdoll();
        enabled = false;
    }
}

