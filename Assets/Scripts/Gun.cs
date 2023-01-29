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
    public float maxAmmo = 5;
    public bool autoFire;
    public TMP_Text ammoText;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    // public GameObject impactEffect;

    PlayerInput playerInput;
    InputAction shootAction;
    InputAction reloadAction;

    private float nextTimeToFire = 0f;
    private bool canShoot = true;
    private bool isReloading = false;
    private float ammo;


    void Awake(){
        playerInput = GetComponentInParent<PlayerInput>();
        shootAction = playerInput.actions["fire"];
        reloadAction = playerInput.actions["reload"];
    }

    void Start()
    {
        ammo = maxAmmo;
    }

    void Update()
    {   if (isReloading){
            ammoText.text = "Reloading...";
        }else{
            ammoText.text = "Ammo: " + ammo.ToString();
        }
    }

    void FixedUpdate()
    {
        var shootInput = shootAction.ReadValue<float>();
        var reloadInput = reloadAction.ReadValue<float>();

        if (shootInput>0 && Time.time >= nextTimeToFire && canShoot && !isReloading)
        {
            if (ammo>0){
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
                if (!autoFire){
                    canShoot = false;
                    StartCoroutine(waitForKeyRelease());
                }
            }else{
                isReloading = true;
                StartCoroutine(Reload());
            }

        }

        if (reloadInput>0 && ammo<maxAmmo && !isReloading){
            isReloading = true;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadSpeed);
        isReloading = false;
        ammo = maxAmmo;
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
        ammo -= 1;
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

}
