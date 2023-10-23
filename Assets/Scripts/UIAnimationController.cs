using UnityEngine;

public class UIAnimationController : MonoBehaviour
{
    [SerializeField] private CameraShaker uiShake;
    [SerializeField] private RectTransform ui;
    [SerializeField] private float animationSmoothing;
    [SerializeField] private Vector3 hideDirection;

    public CameraShaker UIShake => uiShake;
    private Vector3 startPos;
    private Vector3 desiredPositionOffset = Vector3.zero;
    private Vector3 smoothPositionOffset = Vector3.zero;
    private Vector3 positionOffsetVel = Vector3.zero;

    void Awake()
    {
        startPos = ui.localPosition;

        HideUI();
    }

    void Update()
    {
        Vector3 offset = uiShake == null ? Vector3.zero : uiShake.Offset;

        smoothPositionOffset = Vector3.SmoothDamp(smoothPositionOffset, desiredPositionOffset, ref positionOffsetVel, animationSmoothing);
        
        ui.localPosition = startPos + smoothPositionOffset + new Vector3(offset.x, offset.y, 0f);
        ui.localRotation = Quaternion.Euler(ui.localRotation.x, ui.localRotation.y, offset.z + offset.x);
    }

    public void HideUI(bool hide = true) => SetPositionOffset(hide ? hideDirection : Vector3.zero);
    public void SetPositionOffset(Vector3 position) => desiredPositionOffset = position;
    public void SetPositionOffsetRecoil(Vector3 position) => smoothPositionOffset += position;

    public void HideUISnap(bool hide = true)
    {
        desiredPositionOffset = hide ? hideDirection : Vector3.zero;
        smoothPositionOffset = hide ? hideDirection : Vector3.zero;
    }
}
