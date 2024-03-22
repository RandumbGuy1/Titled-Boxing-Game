using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoxer
{
    public BoxerMoveState MoveState { get; }
    public BoxerAttackState AttackState { get; }

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

