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

    public float healDelay; 
    public float healSpeed;
    private bool canHeal = false;
    IEnumerator waitToHeal;
    private bool takingDamage = false;
    private bool isWaitingToHeal = false;

    private bool canShowDamage = true;
    IEnumerator showDamage;

    void Start()
    {
        canvas.SetActive(false);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        damagePanel.SetActive(false);
        canHeal = false;
    }

    void Update(){
        if(canHeal){
            Heal();
        }

        if(!isWaitingToHeal && currentHealth<maxHealth){
            StartCoroutine(WaitToHeal());
        }

        // if(currentHealth<maxHealth){
        //     waitToHeal = WaitToHeal();
        //     StartCoroutine(waitToHeal);

        //     if(canShowDamage){
        //         StopCoroutine(waitToHeal);
        //         canHeal = false;
        //     }
        // }
    }

    public void TakeDamage(float damage)
    {
        takingDamage = true;

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
        takingDamage = false;
    }

    IEnumerator WaitToHeal(){
        canHeal = false;
        isWaitingToHeal = true;
        yield return new WaitForSeconds(healDelay);
        isWaitingToHeal = false;
        canHeal = true;
    }

    private void Heal(){
        while(maxHealth-currentHealth>maxHealth/healSpeed){
            currentHealth += maxHealth/healSpeed; 
            UpdateHealthBar();
        }

        canHeal = false;
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
