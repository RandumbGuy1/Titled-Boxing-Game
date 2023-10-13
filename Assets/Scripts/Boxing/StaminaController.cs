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

    public bool RanOutofStamina => staminaHp <= 0f;

    void Start()
    {
        staminaHp = maxstaminaHp;
    }

    void Update()
    {
        staminaHp += Time.deltaTime * staminaMulti;
        staminaMulti = Mathf.Lerp(staminaMulti, 1f, Time.deltaTime * 5f);

        slider.value = staminaHp / maxstaminaHp;
    }

    public void TakeStamina(float stamina)
    {
        staminaHp = Mathf.Max(0f, staminaHp -= stamina);
        staminaMulti = 0f;
    }
}
