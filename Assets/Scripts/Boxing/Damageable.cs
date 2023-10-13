using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Damageable : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth;
    [SerializeField] private Slider healthBar;
    private float smoothHealth;

    public float Health { get; private set; }
    public UnitState State { get; private set; } = UnitState.Active;

    public delegate void DamageEntity(float damage, float stun);
    public event DamageEntity OnPlayerDamage;
    public UnityEvent<float> OnPlayerDamageEffects;

    void Start()
    {
        Health = maxHealth;
        smoothHealth = maxHealth;

        OnPlayerDamage += (float damage, float stun) =>
        {
            if (State == UnitState.KO || State == UnitState.Inactive) return;

            Health = Mathf.Clamp(Health - damage, 0, maxHealth);
            OnPlayerDamageEffects?.Invoke(damage / maxHealth);

            //Death
            if (Health <= 0f)
            {
                healthBar.value = Health / maxHealth;
                gameObject.SetActive(false);
            }
        };
    }

    void Update()
    {
        if (healthBar == null) return;

        smoothHealth = Mathf.Lerp(smoothHealth, Health, Time.deltaTime * 15f);
        healthBar.value = smoothHealth / maxHealth;
    }

    public void Damage(float damage, float stun)
    {
        OnPlayerDamage?.Invoke(damage, stun);
    }
}

public enum UnitState
{
    Active,
    Inactive,
    KO,
}
