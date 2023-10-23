using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI entityName;
    [SerializeField] private Damageable health;
    [SerializeField] private StaminaController stamina;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;

    void Update()
    {
        if (health) healthSlider.value = health.SliderValue;
        if (stamina) staminaSlider.value = stamina.SliderValue;
    }

    public void SetHealthAndStamina(GameObject subject)
    {
        Damageable health = subject.GetComponent<Damageable>();
        StaminaController stamina = subject.GetComponent<StaminaController>();

        healthSlider.gameObject.SetActive(health);
        staminaSlider.gameObject.SetActive(stamina);

        this.health = health;
        this.stamina = stamina;
        entityName.text = subject.name;
    }
}
