using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{

    public float maxHealth = 100f;
    public float currentHealth;

    public HealthBar healthBar;
    public GameObject canvas;
    private IEnumerator showHp;
    private bool isShowingHP = false;

    void Start()
    {
        canvas.SetActive(false);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(isShowingHP){
            StopCoroutine(showHp);
            showHp = showHealth();
            StartCoroutine(showHp);
        }else{
            showHp = showHealth();
            StartCoroutine(showHp);
        }
        if (currentHealth <= 0f)
        {
            Destroy(this.gameObject);
        }
        UpdateHealthBar();
    }

    public IEnumerator TakeDamageOverTime(float damage, float hit_num){
        for(int i = 0; i < hit_num+1; i++){
            if(currentHealth-damage<=0f){
                yield break;
            }
            TakeDamage(damage);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator showHealth(){
        canvas.SetActive(true);
        isShowingHP = true;
        yield return new WaitForSeconds(1);
        isShowingHP = false;
        canvas.SetActive(false);
    }

    void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }
}
