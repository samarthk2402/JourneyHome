using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float reloadSpeed = 1;
    public float ammo = 5;
    public TMP_Text ammoText;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    // public GameObject impactEffect;

    PlayerInput playerInput;
    InputAction shootAction;

    private float nextTimeToFire = 0f;
    private bool canShoot = true;

    void Awake(){
        playerInput = GetComponentInParent<PlayerInput>();
        shootAction = playerInput.actions["fire"];
    }

    void Update()
    {   
        ammoText.text = "Ammo: " + ammo.ToString();
    }

    void FixedUpdate()
    {
        var shootInput = shootAction.ReadValue<float>();
        if (shootInput>0 && Time.time >= nextTimeToFire && canShoot)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            canShoot = false;
            Shoot();
            StartCoroutine(waitForKeyRelease());
        }
    }

    public IEnumerator waitForKeyRelease()
    {
        yield return new WaitUntil(notShooting);
        canShoot = true;
    }

    bool notShooting() {
        if (shootAction.ReadValue<float>() <= 0){
            return true;
        }else{
            return false;
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            // GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            // Destroy(impactGO, 2f);
        }
    }

    // IEnumerator showDamage(float damage){
    //     Instantiate(damageText, new Vector3(110, 0, 0), Quaternion.identity);
    //     damageText.text = System.Convert.ToString(damage);
    //     yield return new WaitForSeconds(0.5f);
    //     DestroyImmediate(damageText, true);
    // }

}
