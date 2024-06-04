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

    Vector3[] startHandPositions;
    Vector3[] startHandRotations;
    [SerializeField] Transform[] handPositions = new Transform[2];
    [SerializeField] GloveCollision[] gloves = new GloveCollision[2];
    [SerializeField] Color punchIndicator;

    [Header("Refrences")]
    [SerializeField] Damageable health;
    [SerializeField] StaminaController stamina;
    [SerializeField] BlockController block;
    [SerializeField] BoxerMovement movement;
    [SerializeField] private Transform orientation;
    [SerializeField] CameraShaker camShaker;
    [SerializeField] CameraHeadBob headBob;

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
    private void Start()
    {
        startHandPositions = new Vector3[handPositions.Length];
        startHandRotations = new Vector3[handPositions.Length];
        for (int i = 0; i < handPositions.Length; i++)
        {
            startHandPositions[i] = handPositions[i].localPosition;
            startHandRotations[i] = handPositions[i].localEulerAngles;
        }
    }

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
        {
            if (headBob)
            {
                handPositions[i].localPosition = startHandPositions[i] + headBob.ViewBobOffset * 0.15f * (i % 2 == 0 ? 1 : -0.3f);
                handPositions[i].localRotation = Quaternion.Euler(startHandRotations[i] + headBob.ViewBobOffset * 5f * (i % 2 == 0 ? 1 : -0.3f));
            }
            
            gloves[i].HandleGloves(handPositions[i], orientation.forward);
        }

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

        void PunchShake()
        {
            if (camShaker) camShaker.ShakeOnce(new PerlinShake(ShakeData.Create(3f, 6f, 0.7f, 10f)));
            movement.Rb.AddTorque(orientation.right * 2.5f, ForceMode.Impulse);

            stamina.TakeStamina(gloves[button].StaminaCost);
        }

        switch (movement.MoveState)
        {
            case BoxerMoveState.Moving:
                gloves[button].SetGlove(true, PunchType.Straight, punchIndicator);
                movement.Rb.velocity *= 0f;

                AttackState = BoxerAttackState.Punching;
                PunchShake();
                break;
            case BoxerMoveState.SlippingLeft:
                movement.Rb.velocity *= 0f;

                if (button == 0)
                {
                    gloves[button].SetGlove(true, PunchType.Hook, punchIndicator);
                    SetMoveState(BoxerMoveState.Moving);
                    PunchShake();
                    break;
                }

                gloves[button].SetGlove(true, PunchType.Straight, punchIndicator);
                PunchShake();
                break;
            case BoxerMoveState.SlippingRight:
                movement.Rb.velocity *= 0f;

                if (button == 1)
                {
                    gloves[button].SetGlove(true, PunchType.Hook, punchIndicator);
                    SetMoveState(BoxerMoveState.Moving);
                    PunchShake();
                    break;
                }

                gloves[button].SetGlove(true, PunchType.Straight, punchIndicator);
                PunchShake();
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

