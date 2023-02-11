using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public HealthBar healthBar;
    public GameObject canvas;
    public GameObject damagePanel;

    private bool canShowDamage = true;
    IEnumerator showDamage;

    void Start()
    {
        canvas.SetActive(false);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        damagePanel.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (canShowDamage){
            showDamage = ShowDamage();
            StartCoroutine(showDamage);
            canShowDamage = false;
        }else{
            StopCoroutine(showDamage);
            StartCoroutine(showDamage);
        }

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

    IEnumerator ShowDamage(){
        damagePanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        damagePanel.SetActive(false);
        canShowDamage = true;
    }

    void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }
}
