using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] Camera cam;

    [SerializeField] private Damageable health;
    [SerializeField] private BoxingController boxer;

    [SerializeField] private TextMeshProUGUI entityName;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private SpecialConditionFX counterFX;
    [SerializeField] private SpecialConditionFX staminaFX;
    [SerializeField] Image shieldDisplay;
    
    void Update()
    {
        if (health) healthSlider.value = health.SliderValue;

        if (boxer)
        {
            staminaSlider.value = boxer.Stamina.SliderValue;

            shieldDisplay.rectTransform.position = cam.WorldToScreenPoint(boxer.transform.position);
            shieldDisplay.color = Color.Lerp(shieldDisplay.color, boxer.Block ? Color.white : Color.clear, Time.deltaTime * 10f);
        }
        
    }

    public void SetHealthAndStamina(GameObject subject)
    {
        if (this.health) this.health.OnPlayerCounter.RemoveListener(counterFX.Trigger);
        if (this.boxer) this.boxer.Stamina.OnStaminaBreakEffects.RemoveListener(staminaFX.Trigger);

        Damageable health = subject.GetComponent<Damageable>();
        BoxingController boxer = subject.GetComponent<BoxingController>();

        healthSlider.gameObject.SetActive(health);
        staminaSlider.gameObject.SetActive(boxer);
        shieldDisplay.gameObject.SetActive(boxer);

        if (health)
        {
            health.OnPlayerCounter.AddListener(counterFX.Trigger);
            counterFX.SetTarget(subject.transform);
        }
        if (boxer)
        {
            boxer.Stamina.OnStaminaBreakEffects.AddListener(staminaFX.Trigger);
            staminaFX.SetTarget(subject.transform);
        }

        this.health = health;
        this.boxer = boxer;
        entityName.text = subject.name;
    }
}
