using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public float damage = 20f;
    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<PlayerTarget>().TakeDamage(damage);
            Destroy(this.gameObject);
        }else if(collision.gameObject.layer == 3){
            Destroy(this.gameObject);
        }
    }
    
}
