using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StaminaController : MonoBehaviour
{
    [SerializeField] private float maxStaminaRecharge = 100f;
    [SerializeField] private float staminaBreakRegainSpeed = 0f;
    [SerializeField] private float maxstaminaHp;
    [SerializeField] private Slider slider;
    private float staminaHp = 0f;
    private float staminaMulti = 1f;

    public bool RanOutofStamina { get; private set; } = false;
    public UnityEvent<float> OnStaminaBreakEffects;

    void Start()
    {
        staminaHp = maxstaminaHp;
    }

    float smoothStamina = 100f;
    public float SliderValue => staminaHp == 0f ? 0f : smoothStamina / maxstaminaHp;
    void Update()
    {
        staminaHp = Mathf.Min(staminaHp + Time.deltaTime * staminaMulti * (RanOutofStamina ? staminaBreakRegainSpeed : 1f), maxstaminaHp);
        staminaMulti = RanOutofStamina ? 1f : Mathf.Lerp(staminaMulti, maxStaminaRecharge, Time.deltaTime * 0.5f);
        smoothStamina = Mathf.Lerp(smoothStamina, staminaHp, 15f * Time.deltaTime);

        if (slider) slider.value = SliderValue;

        if (RanOutofStamina)
        {
            if (SliderValue >= 0.99f) RanOutofStamina = false;
        }
    }

    public void TakeStamina(float stamina)
    {
        staminaHp = staminaHp -= stamina;
        staminaMulti = 0f;

        if (staminaHp <= 0f)
        {
            staminaHp = 0f;
            RanOutofStamina = true;
            OnStaminaBreakEffects?.Invoke(1f);
        }
    }
}
