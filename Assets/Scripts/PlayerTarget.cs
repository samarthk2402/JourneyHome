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
    public float waitToHealTime;
    private bool canHeal;

    private bool canShowDamage = true;
    IEnumerator showDamage;
    private float lastTakenDamage;
    private float timePassed;

    void Start()
    {
        canvas.SetActive(false);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        damagePanel.SetActive(false);
        canHeal = true;
    }

    void Update(){

        if(timePassed-lastTakenDamage>=waitToHealTime){
            Heal();
        }

        timePassed += Time.deltaTime;

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
        lastTakenDamage = timePassed;
    }

    private void Heal(){
        //while(currentHealth<maxHealth){
            if(canHeal && currentHealth<maxHealth){
                currentHealth += healSpeed;
                UpdateHealthBar();
                StartCoroutine(HealSpeedDelay());
            }
        //}
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

    IEnumerator HealSpeedDelay(){
        canHeal = false;
        yield return new WaitForSeconds(healDelay);
        canHeal = true;
    }

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
