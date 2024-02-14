using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoxer
{
    public bool Punching { get; }
    public bool CanPunch { get; }
    public bool CanDash { get;  }
    // sun tzu was here
    public bool CanPreformActions { get; }

    public Damageable Health { get; }
    public StaminaController Stamina { get; }
    public StunController Stun { get; }
    public BlockController Block { get; }

    public void Disable();
}
