using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] Camera cam;

    [SerializeField] private Damageable health;
    [SerializeField] private StaminaController stamina;
    [SerializeField] private BlockController block;

    [SerializeField] private TextMeshProUGUI entityName;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private SpecialConditionFX counterFX;
    [SerializeField] private SpecialConditionFX staminaFX;
    [SerializeField] Image shieldDisplay;
    
    void Update()
    {
        if (health) healthSlider.value = health.SliderValue;
        if (stamina) staminaSlider.value = stamina.SliderValue;

        if (block)
        {
            shieldDisplay.rectTransform.position = cam.WorldToScreenPoint(block.transform.position);
            shieldDisplay.color = Color.Lerp(shieldDisplay.color, block.Blocking ? Color.white : Color.clear, Time.deltaTime * 10f);
        }
        
    }

    public void SetHealthAndStamina(GameObject subject)
    {
        if (this.health) this.health.OnPlayerCounter.RemoveListener(counterFX.Trigger);
        if (this.stamina) this.stamina.OnStaminaBreakEffects.RemoveListener(staminaFX.Trigger);

        Damageable health = subject.GetComponent<Damageable>();
        StaminaController stamina = subject.GetComponent<StaminaController>();
        BlockController block = subject.GetComponent<BlockController>();

        healthSlider.gameObject.SetActive(health);
        staminaSlider.gameObject.SetActive(stamina);
        shieldDisplay.gameObject.SetActive(block);

        if (health)
        {
            stamina.OnStaminaBreakEffects.AddListener(staminaFX.Trigger);
            staminaFX.SetTarget(subject.transform);
        }
        if (stamina)
        {
            health.OnPlayerCounter.AddListener(counterFX.Trigger);
            counterFX.SetTarget(subject.transform);
        }

        this.health = health;
        this.stamina = stamina;
        this.block = block;
        entityName.text = subject.name;
    }
}
