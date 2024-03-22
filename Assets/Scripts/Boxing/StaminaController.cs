using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StaminaController : MonoBehaviour
{
    [SerializeField] private BoxingController boxer;
    [SerializeField] private float maxStaminaRecharge = 100f;
    [SerializeField] private float staminaBreakRegainSpeed = 0f;
    [SerializeField] private float maxstaminaHp;
    [SerializeField] private Slider slider;
    private float staminaHp = 0f;
    private float staminaMulti = 1f;
    private float vel = 0f;

    public UnityEvent<float> OnStaminaBreakEffects;

    float smoothStamina = 100f;
    public float SliderValue => staminaHp == 0f ? 0f : smoothStamina / maxstaminaHp;

    void Start()
    {
        staminaHp = maxstaminaHp;
    }

    void Update()
    {
        staminaHp = Mathf.Min(staminaHp + Time.deltaTime * staminaMulti * (boxer.Movement.RanOutofStamina ? staminaBreakRegainSpeed : 1f), maxstaminaHp);
        staminaMulti = boxer.Movement.RanOutofStamina ? 1f : Mathf.SmoothDamp(staminaMulti, maxStaminaRecharge, ref vel, 5f);
        smoothStamina = Mathf.Lerp(smoothStamina, staminaHp, 15f * Time.deltaTime);

        if (slider) slider.value = SliderValue;

        if (boxer.Movement.RanOutofStamina && SliderValue >= 0.99f) boxer.SetMoveState(BoxerMoveState.Moving);
    }

    public void TakeStamina(float stamina, bool dontStun = false)
    {
        staminaHp = staminaHp -= stamina;
        staminaMulti = 0f;

        if (staminaHp <= 0f)
        {
            if (dontStun)
            {
                staminaHp = 1f;
                return;
            }

            staminaHp = 0f;
            OnStaminaBreakEffects?.Invoke(1f);

            boxer.SetMoveState(BoxerMoveState.StaminaDepleted);
        }
    }
}
