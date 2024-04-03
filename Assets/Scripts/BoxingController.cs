using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingController : MonoBehaviour
{
    public BoxerMoveState MoveState => movement.MoveState;
    public BoxerAttackState AttackState { get; set; }

    public void SetMoveState(BoxerMoveState newState) => movement.MoveState = newState;
    public void SetAttackState(BoxerAttackState newState) => AttackState = newState;

    public bool Punching => AttackState == BoxerAttackState.Punching;
    public bool Idle => AttackState == BoxerAttackState.Idle;
    public bool Blocking => AttackState == BoxerAttackState.Blocking;

    [SerializeField] Transform[] handPositions = new Transform[2];
    [SerializeField] GloveCollision[] gloves = new GloveCollision[2];

    [Header("Refrences")]
    [SerializeField] Damageable health;
    [SerializeField] StaminaController stamina;
    [SerializeField] BlockController block;
    [SerializeField] BoxerMovement movement;
    [SerializeField] private Transform orientation;

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
    public BlockController Block => block;
    public BoxerMovement Movement => movement;

    void FixedUpdate()
    {
        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
            gloves[i].DetectCollisions(transform);
    }

    public void HandlePunching(FrameInput input)
    {
        int button = input.PunchInput;
        if (!enabled || gloves.Length <= 0) return;

        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
            gloves[i].HandleGloves(handPositions[i], orientation.forward);

        bool PunchCheck()
        {
            foreach (GloveCollision glove in gloves) if (glove.Active) return true;
            return false;
        }

        if (AttackState == BoxerAttackState.Punching)
        {
            if (!PunchCheck()) AttackState = BoxerAttackState.Idle;
        }

        if (button >= 2 || button < 0 || !CanPunch || !Idle) return;

        switch (movement.MoveState)
        {
            case BoxerMoveState.Moving:
                gloves[button].SetGlove(true, 0f, stamina, true);
                movement.Rb.velocity *= 0f;

                AttackState = BoxerAttackState.Punching;
                break;
            case BoxerMoveState.Slipping:
                movement.Rb.velocity *= 0f;

                //Hook punch
                if (movement.SlipDirection == 1 && button == 0 || movement.SlipDirection == -1 && button == 1)
                {
                    movement.SlipDirection = -movement.SlipDirection;
                    gloves[button].SetGlove(true, 0f, stamina, false);
                }
                else gloves[button].SetGlove(true, 0f, stamina, true);

                AttackState = BoxerAttackState.Punching;
                break;
            default:
                break;
        }
    }

    public void ResetGloves()
    {
        foreach (GloveCollision glove in gloves) glove.SetGlove(false);
        AttackState = BoxerAttackState.Idle;
    }

    public void Disable()
    {
        movement.enabled = false;

        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++) gloves[i].Ragdoll();

        enabled = false;

        AttackState = BoxerAttackState.Idle;
    }
}

