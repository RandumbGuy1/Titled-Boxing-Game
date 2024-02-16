using UnityEngine;
using TMPro;

public class SpecialConditionFX : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float shakeIntensity = 20f;
    [SerializeField] private CameraShaker shaker; 
    [SerializeField] private PostProcessController postProcess;

    [SerializeField] AudioClip clip;
    [SerializeField] private GameObject effect;

    [Header("Text Effects")]
    [SerializeField] private string effectName;
    [SerializeField] private Color effectColor;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private CameraShaker textShake;

    public void SetTarget(Transform target) => this.target = target;

    public void Trigger(float power)
    {
        if (target == null) target = transform;

        if (effectText)
        {
            effectText.text = effectName;
            effectText.color = effectColor;
        }

        if (effect) Instantiate(effect, target.position, Quaternion.identity);
        if (shaker) shaker.ShakeOnce(new PerlinShake(ShakeData.Create(shakeIntensity, 6f, 0.7f, 10f)));
        if (textShake) textShake.ShakeOnce(new PerlinShake(ShakeData.Create(40f, 6f, 0.7f, 10f)));
        if (postProcess)
        {
            postProcess.FocusDistance.SetValue(65f, 3f, true);
            postProcess.VignetteIntensity.SetValue(0.4f, 3f, true);
        }

        AudioManager.Instance.PlayOnce(clip, target.position);
    }
}
