using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{

    public float maxHealth = 100f;
    private float currentHealth;

    public HealthBar healthBar;
    public GameObject canvas;

    void Start()
    {
        canvas.SetActive(false);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(showHealth());
        if (currentHealth <= 0f)
        {
            Destroy(this.gameObject);
        }
        UpdateHealthBar();
    }

    IEnumerator showHealth(){
        canvas.SetActive(true);
        yield return new WaitForSeconds(1);
        canvas.SetActive(false);
    }

    void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }
}
