using UnityEngine;

public class BoxingGloves : MonoBehaviour, IBoxer
{
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

        if (button >= 2 || button < 0) return;
        if (!CanPunch || !CanPreformActions) return;

        gloves[button].SetGlove(true, 0f, stamina);
        player.PlayerMovement.Rb.velocity *= 0f;
    }

    public void ResetGloves(float x)
    {
        foreach (GloveCollision glove in gloves) glove.SetGlove(false);
    }

    public void Disable()
    {
        player.PlayerMovement.GoLimp(100f);
        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++) gloves[i].Ragdoll();
        enabled = false;
    }
}
