using UnityEngine;
using TMPro;

public class SpecialConditionFX : MonoBehaviour
{
    [SerializeField] private PlayerRef player;
    [SerializeField] private PostProcessController postProcess;

    [SerializeField] AudioClip clip;
    [SerializeField] private GameObject effect;

    [Header("Text Effects")]
    [SerializeField] private string effectName;
    [SerializeField] private Color effectColor;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private CameraShaker textShake;

    public void Trigger(float power)
    {
        if (effectText)
        {
            effectText.text = effectName;
            effectText.color = effectColor;
        }

        if (effect) Instantiate(effect, transform.position, Quaternion.identity);
        if (player) player.CameraBody.CamShaker.ShakeOnce(new PerlinShake(ShakeData.Create(20f, 6f, 0.7f, 10f)));
        if (textShake) textShake.ShakeOnce(new PerlinShake(ShakeData.Create(40f, 6f, 0.7f, 10f)));
        if (postProcess)
        {
            postProcess.FocusDistance.SetValue(65f, 3f, true);
            postProcess.VignetteIntensity.SetValue(0.4f, 3f, true);
        }

        AudioManager.Instance.PlayOnce(clip, transform.position );
    }
}
