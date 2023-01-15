using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{

    public float maxHealth = 100f;
    private float currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            Destroy(this.gameObject);
        }
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }
}
