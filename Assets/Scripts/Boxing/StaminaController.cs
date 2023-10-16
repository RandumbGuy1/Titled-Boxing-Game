using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField] private float maxstaminaHp;
    [SerializeField] private Slider slider;
    private float staminaHp = 0f;
    private float staminaMulti = 1f;

    public bool RanOutofStamina => staminaHp <= 5f;

    void Start()
    {
        staminaHp = maxstaminaHp;
    }

    float smoothStamina = 100f;
    void Update()
    {
        staminaHp += Time.deltaTime * staminaMulti;
        staminaMulti = Mathf.Lerp(staminaMulti, 100f, Time.deltaTime * 0.4f);

        smoothStamina = Mathf.Lerp(smoothStamina, staminaHp, 5f * Time.deltaTime);
        slider.value = smoothStamina / maxstaminaHp;
    }

    public void TakeStamina(float stamina)
    {
        staminaHp = Mathf.Max(0f, staminaHp -= stamina);
        staminaMulti = 0f;
    }
}
