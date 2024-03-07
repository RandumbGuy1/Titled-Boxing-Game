using UnityEngine;

public class BoxingGloves : MonoBehaviour, IBoxer
{
    public BoxerMoveState MoveState { get; }
    public BoxerAttackState AttackState { get; }

    [SerializeField] Damageable health;
    [SerializeField] StaminaController stamina;
    [SerializeField] StunController stun;
    [SerializeField] BlockController block;

    [SerializeField] Transform[] handPositions = new Transform[2];
    [SerializeField] GloveCollision[] gloves = new GloveCollision[2];

    [SerializeField] PlayerRef player;

    public bool CanPreformActions => !player.PlayerMovement.Crouching && !stamina.RanOutofStamina && !stun.InStun;
    public bool CanDash => !stamina.RanOutofStamina && !stun.InStun;

    public bool Punching
    {
        get
        {
            foreach (GloveCollision glove in gloves) if (glove.Active) return true;
            return false;
        }
    }

    public bool CanPunch { get
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
            gloves[i].HandleGloves(handPositions[i], player.Orientation.forward);
        }

        if (button >= 2 || button < 0) return;
        if (!CanPunch || !CanPreformActions) return;

        gloves[button].SetGlove(true, 0f, stamina);
        player.PlayerMovement.Rb.velocity *= 0f;
    }

    public void ResetGloves()
    {
        foreach (GloveCollision glove in gloves) glove.SetGlove(false);
    }

    public void Disable()
    { 
        player.PlayerMovement.enabled = false;
        player.PlayerMovement.Rb.freezeRotation = false;
        player.PlayerInput.Enabled = false;

        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++) gloves[i].Ragdoll();
        enabled = false;
    }
}
