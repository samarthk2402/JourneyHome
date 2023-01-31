using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Gun : MonoBehaviour
{
    [SerializeField] List<GunObject> weapons = new List<GunObject>();
    public TMP_Text ammoText;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    // public GameObject impactEffect;
    public MeshFilter gunMeshFilter;

    PlayerInput playerInput;
    InputAction shootAction;
    InputAction reloadAction;
    InputAction scrollAction;

    private float nextTimeToFire = 0f;
    private bool canShoot = true;
    private bool isReloading = false;
    private List<int> ammo = new List<int>();
    private int weaponIndex = 0;
    private IEnumerator reload = null;

    void Awake(){
        playerInput = GetComponentInParent<PlayerInput>();
        shootAction = playerInput.actions["fire"];
        reloadAction = playerInput.actions["reload"];
        scrollAction = playerInput.actions["scroll"];
    }

    void Start()
    {
        for(int i = 0; i < weapons.Count; i++){
            ammo.Add(weapons[i].maxAmmo);
        }
    }

    void Update()
    {   if (isReloading){
            ammoText.text = "Reloading...";
        }else{
            ammoText.text = "Ammo: " + ammo[weaponIndex].ToString();
        }

        var scrollInput = scrollAction.ReadValue<float>();
        if (scrollInput>0){
            if(isReloading){
                StopCoroutine(reload);
                ammo[weaponIndex] = 0;
                isReloading = false;
            }

            if (weaponIndex < weapons.Count-1){
                weaponIndex += 1;
            }else{
                weaponIndex = 0;
            }
        }else if (scrollInput<0){
            if(isReloading){
                StopCoroutine(reload);
                ammo[weaponIndex] = 0;
                isReloading = false;
            }

            if (weaponIndex > 0){
                weaponIndex -= 1;
            }else{
                weaponIndex = weapons.Count-1;
            }
        }

        gunMeshFilter.mesh = weapons[weaponIndex].weaponMesh;

    }

    void FixedUpdate()
    {
        var shootInput = shootAction.ReadValue<float>();
        var reloadInput = reloadAction.ReadValue<float>();

        if (shootInput>0 && Time.time >= nextTimeToFire && canShoot && !isReloading)
        {
            if (ammo[weaponIndex]>0){
                nextTimeToFire = Time.time + 1f / weapons[weaponIndex].fireRate;
                Shoot();
                if (!weapons[weaponIndex].autoFire){
                    canShoot = false;
                    StartCoroutine(waitForKeyRelease());
                }
            }else{
                isReloading = true;
                reload = Reload();
                StartCoroutine(reload);
            }

        }

        if (reloadInput>0 && ammo[weaponIndex]<weapons[weaponIndex].maxAmmo && !isReloading){
            isReloading = true;
            reload = Reload();
            StartCoroutine(reload);
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(weapons[weaponIndex].reloadSpeed);
        isReloading = false;
        ammo[weaponIndex] = weapons[weaponIndex].maxAmmo;
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
        ammo[weaponIndex] -= 1;
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, weapons[weaponIndex].range))
        {

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(weapons[weaponIndex].damage);
            }

            // GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            // Destroy(impactGO, 2f);
        }
    }

}
