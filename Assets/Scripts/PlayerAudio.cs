using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour
{
    [Header("Grounded Movement Audio")]
    [SerializeField] private AudioClip footStepClip;
    [SerializeField] private AudioClip footStepClip2;
    [SerializeField] private AudioClip playerLandClip;
    [SerializeField] private float footStepFrequency;
    [SerializeField] private float footStepVolumeMultiplier;
    [Space]
    [SerializeField] private AudioClip playerCrouchClip;
    private float footstepDistance;

    [Header("Refrences")]
    [SerializeField] private Player player;

    void Awake()
    {
        player.Movement.OnGroundHit.AddListener((float magnitude) => { AudioManager.Instance.PlayOnce(playerLandClip, transform.position); });

        player.Movement.OnRoll.AddListener((bool input) =>
        {
            if (!player.Movement.Grounded) return;
            AudioManager.Instance.PlayOnce(playerCrouchClip, transform.position);
        });
    }
    bool toggle = false;
    public void CalculateFootsteps(FrameInput input)
    {
        if (!player.CameraBody.CamHeadBob.Bobbing)
        {
            footstepDistance = 0f;
            return;
        }
        
        float walkMagnitude = player.Movement.Magnitude;
        walkMagnitude = Mathf.Clamp(walkMagnitude, 0f, 20f);

        footstepDistance += walkMagnitude * Time.deltaTime * footStepFrequency;

        if (footstepDistance > 450f)
        {
            toggle = !toggle;
            AudioManager.Instance.PlayOnce(toggle ? footStepClip : footStepClip2, transform.position, footStepVolumeMultiplier);
            footstepDistance = 0f;
        }
    }
}
