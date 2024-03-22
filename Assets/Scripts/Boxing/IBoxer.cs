using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoxer
{
    public BoxerMoveState MoveState { get; }
    public void SetMoveState(BoxerMoveState newState);

    public BoxerAttackState AttackState { get; }
    public void SetAttackState(BoxerAttackState newState);

    public bool Punching => AttackState == BoxerAttackState.Punching;
    public bool Idle => AttackState == BoxerAttackState.Idle;
    public bool Blocking => AttackState == BoxerAttackState.Blocking;

    public BoxerMovement Movement { get; }
    public Damageable Health { get; }
    public StaminaController Stamina { get; }
    public StunController Stun { get; }
    public BlockController Block { get; }

    public void Disable();
}

public enum BoxerMoveState
{
    Moving,
    Slipping,
    Rolling,
    Stunned,
    StaminaDepleted,
}

public enum BoxerAttackState
{
    Punching,
    Blocking,
    Idle,
}

