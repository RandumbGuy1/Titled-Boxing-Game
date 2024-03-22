using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Refrences")]
    [SerializeField] private BoxerMovement movement;
    [SerializeField] private BoxingController controller;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CameraBody cameraBody;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform orientation;
    [SerializeField] private MeshRenderer gfx;
    [SerializeField] private CapsuleCollider capsuleCol;

    public BoxerMovement Movement => movement;
    public BoxingController Controller => controller;
    public CapsuleCollider CapsuleCol => capsuleCol;
    public MeshRenderer Gfx => gfx;
    public PlayerInput Keys => playerInput;
    public CameraBody CameraBody => cameraBody;
    public Camera PlayerCam => playerCam;
    public Transform Orientation => orientation;
}
