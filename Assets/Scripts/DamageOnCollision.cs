using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public float damage = 100f;
    public int hit_num;
    public bool isDot;
    public GameObject puddle;
    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.layer == 6)
        {
            
            collision.gameObject.GetComponent<PlayerTarget>().TakeDamage(damage);
            Destroy(this.gameObject);
               
        }else if(collision.gameObject.layer == 3){
            ContactPoint cp = collision.GetContact(0);
            Instantiate(puddle, cp.point+new Vector3(0, 0, 0.3f), Quaternion.FromToRotation (Vector3.up, cp.normal));
            Destroy(this.gameObject);

        }
    }
    
}
