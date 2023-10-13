using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitFX : MonoBehaviour
{
    [SerializeField] private PlayerRef player;
    [SerializeField] private AudioClip earRingingClip;
    [SerializeField] private PostProcessController postProcess;

    public void DamageEffect(float damageRatio)
    {
        AudioManager.Instance.PlayOnce(earRingingClip, transform.position, damageRatio);
        player.CameraBody.CamShaker.ShakeOnce(new PerlinShake(ShakeData.Create(20f, 5f, 0.7f, 10f)));

        if (!postProcess) return;

        postProcess.FocusDistance.SetValue(65f, 3f, true);
        postProcess.VignetteIntensity.SetValue(0.4f, 3f, true);
    }
}
