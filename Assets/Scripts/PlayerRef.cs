using UnityEngine;

public class PlayerRef : MonoBehaviour
{
    [Header("Central Player Refrences")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CameraBody cameraBody;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform orientation;
    [SerializeField] private MeshRenderer rendering;
    [SerializeField] private CapsuleCollider capsuleCol;

    public CapsuleCollider CapsuleCol => capsuleCol;
    public MeshRenderer Rendering => rendering;
    public PlayerMovement PlayerMovement => playerMovement;
    public PlayerInput PlayerInput => playerInput;
    public CameraBody CameraBody => cameraBody;
    public Camera PlayerCam => playerCam;
    public Transform Orientation => orientation;

    void Awake()
    {
        Application.targetFrameRate = 85;
    }
}
