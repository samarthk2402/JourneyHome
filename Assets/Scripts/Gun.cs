using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunObject gunScriptableObject;
    public TMP_Text ammoText;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    // public GameObject impactEffect;
    public MeshFilter gunMeshFilter;

    PlayerInput playerInput;
    InputAction shootAction;
    InputAction reloadAction;

    private float nextTimeToFire = 0f;
    private bool canShoot = true;
    private bool isReloading = false;
    private float ammo;


    void Awake(){
        gunMeshFilter.mesh = gunScriptableObject.weaponMesh;
        playerInput = GetComponentInParent<PlayerInput>();
        shootAction = playerInput.actions["fire"];
        reloadAction = playerInput.actions["reload"];
    }

    void Start()
    {
        ammo = gunScriptableObject.maxAmmo;
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
                nextTimeToFire = Time.time + 1f / gunScriptableObject.fireRate;
                Shoot();
                if (!gunScriptableObject.autoFire){
                    canShoot = false;
                    StartCoroutine(waitForKeyRelease());
                }
            }else{
                isReloading = true;
                StartCoroutine(Reload());
            }

        }

        if (reloadInput>0 && ammo<gunScriptableObject.maxAmmo && !isReloading){
            isReloading = true;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(gunScriptableObject.reloadSpeed);
        isReloading = false;
        ammo = gunScriptableObject.maxAmmo;
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
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunScriptableObject.range))
        {

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(gunScriptableObject.damage);
            }

            // GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            // Destroy(impactGO, 2f);
        }
    }

}
