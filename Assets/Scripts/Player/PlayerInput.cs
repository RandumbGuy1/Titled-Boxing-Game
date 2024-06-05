using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public bool Enabled { get; set; } = true;

    public delegate void ReceieveVector2Input(Vector2 input);
    public delegate void ReceieveBoolInput(bool input);
    public delegate void ReceieveIntInput(int input);
    public delegate void ReceieveFloatInput(float input);

    public FrameInput PlayerFrameInput { get; private set; } = new FrameInput();
    public Vector2 MouseInput { get; private set; }
    public bool LockOnInput { get; private set; }
    public bool PauseToggle { get; private set; }

    public UnityEvent<FrameInput> OnFrameInput;
    public event ReceieveVector2Input OnMouseInput;
    public event ReceieveBoolInput OnLockInput;
    public event ReceieveBoolInput OnPauseToggle;
    public event ReceieveBoolInput OnInteractInput;

    [Header("Boxing Keybinds")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode rollKey;
    [SerializeField] private List<KeyCode> slipKeys = new List<KeyCode>();
    [SerializeField] private KeyCode blockKey;

    [Header("Interact Keybinds")]
    [SerializeField] private KeyCode interactKey;

    [Header("UI Keybinds")]
    [SerializeField] private KeyCode toggleLockOnKey;
    [SerializeField] private KeyCode pauseMenuKey;

    [Header("Refrences")]
    [SerializeField] private Transform orientation;

    public KeyCode JumpKey => jumpKey;
    public KeyCode RollKey => rollKey;
    public KeyCode BlockKey => blockKey;
    public KeyCode ToggleLockOnKey => toggleLockOnKey;
    public KeyCode PauseMenuKey => pauseMenuKey;
    public KeyCode InteractKey => interactKey;

    void Update()
    {
        PauseToggle = Input.GetKeyDown(pauseMenuKey);
        OnPauseToggle?.Invoke(PauseToggle);

        if (!Enabled) return;

        LockOnInput = Input.GetKeyDown(toggleLockOnKey);
        OnLockInput?.Invoke(LockOnInput);

        MouseInput = new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        OnMouseInput?.Invoke(MouseInput);
        OnInteractInput?.Invoke(Input.GetKeyDown(interactKey));

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        PlayerFrameInput.SetInput(
            orientation.TransformDirection(x, 0, y),
            new Vector2(x, y),
            Input.GetKeyDown(jumpKey),
            Input.GetKey(jumpKey),
            Input.GetKeyDown(rollKey),
            IterateKeyDowns(slipKeys),
            Input.GetKey(blockKey),
            MouseButtonDown(),
            MouseButton());

        OnFrameInput?.Invoke(PlayerFrameInput);
    }

    int IterateKeyDowns(List<KeyCode> keys)
    {
        foreach (KeyCode key in keys) if (Input.GetKeyDown(key)) return keys.IndexOf(key);
        return -1;
    }

    int IterateKeys(List<KeyCode> keys)
    {
        foreach (KeyCode key in keys) if (Input.GetKey(key)) return keys.IndexOf(key);
        return -1;
    }

    int MouseButton()
    {
        if (Input.GetMouseButton(0)) return 0;
        if (Input.GetMouseButton(1)) return 1;
        if (Input.GetMouseButton(2)) return 2;
        return -1;
    }

    int MouseButtonDown()
    {
        if (Input.GetMouseButtonDown(0)) return 0;
        if (Input.GetMouseButtonDown(1)) return 1;
        if (Input.GetMouseButtonDown(2)) return 2;
        return -1;
    }
}

public class FrameInput {
    public Vector3 MoveDir { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpHoldInput { get; private set; }
    public bool RollInput { get; private set; }
    public int SlipInput { get; private set; }
    public bool BlockInput { get; private set; }
    public int PunchInput { get; private set; }
    public int PunchHoldInput { get; private set; }

    public void SetInput(
        Vector3 moveDir,
        Vector2 moveInput,
        bool jumpInput,
        bool jumpHoldInput,
        bool rollInput,
        int slipInput,
        bool blockInput,
        int punchInput,
        int punchHoldInput)
    {
        MoveDir = moveDir;
        MoveInput = moveInput;
        JumpInput = jumpInput;
        JumpHoldInput = jumpHoldInput;
        RollInput = rollInput;
        SlipInput = slipInput;
        BlockInput = blockInput;
        PunchInput = punchInput;
        PunchHoldInput = punchHoldInput;
    }
}
