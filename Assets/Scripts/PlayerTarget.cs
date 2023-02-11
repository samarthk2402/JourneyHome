using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

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
        UpdateHealthBar();
    }

    // public IEnumerator TakeDamageOverTime(float damage, float hit_num){
    //     for(int i = 0; i < hit_num+1; i++){
    //         if(currentHealth-damage<=0f){
    //             yield break;
    //         }
    //         TakeDamage(damage);
    //         yield return new WaitForSeconds(0.5f);
    //     }
    // }

    void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }
}
