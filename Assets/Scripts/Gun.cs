using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Gun : MonoBehaviour
{
    [SerializeField] List<GunObject> weapons = new List<GunObject>();
    [SerializeField] List<Scope> scopes = new List<Scope>();
    [SerializeField] List<Magazine> mags = new List<Magazine>();
    [SerializeField] List<Suppressor> suppressors = new List<Suppressor>();
    public GameObject scope;
    public GameObject suppressor;
    public Transform body;
    public TMP_Text ammoText;

    public Camera fpsCam;
    private CameraFOV camFOV;
    // public GameObject impactEffect;
    public MeshFilter gunMeshFilter;
    public MeshFilter scopeMeshFilter;
    public MeshFilter suppressorMeshFilter;

    PlayerInput playerInput;
    InputAction shootAction;
    InputAction reloadAction;
    InputAction scrollAction;
    InputAction zoomAction;

    private float nextTimeToFire = 0f;
    private bool canShoot = true;
    private bool isReloading = false;
    private List<int> ammo = new List<int>();
    private int weaponIndex = 0;
    private IEnumerator reload = null;
    private float startFOV;
    private float zoomMultiplier;

    void Awake(){
        playerInput = GetComponentInParent<PlayerInput>();
        shootAction = playerInput.actions["fire"];
        reloadAction = playerInput.actions["reload"];
        scrollAction = playerInput.actions["scroll"];
        zoomAction = playerInput.actions["zoom"];
    }

    void Start()
    {
        startFOV = 60;
        camFOV = GetComponentInParent<CameraFOV>();

        for(int i = 0; i < weapons.Count; i++){
            ammo.Add(mags[i].maxAmmo);
        }

        SwitchWeapon(weaponIndex);
    }

    void Update()
    { 
        var scrollInput = scrollAction.ReadValue<float>();

          if (isReloading){
            ammoText.text = "Reloading...";
        }else{
            ammoText.text = "Ammo: " + ammo[weaponIndex].ToString();
        }

        gunMeshFilter.mesh = weapons[weaponIndex].weaponMesh;
        
        try{
            scope.SetActive(true);
            scopeMeshFilter.mesh = scopes[weaponIndex].scopeMesh;
            zoomMultiplier = scopes[weaponIndex].zoomMultiplier;
        }
        catch{
            scope.SetActive(false);
            zoomMultiplier = 1;
        }

        try{
            suppressor.SetActive(true);
            suppressorMeshFilter.mesh = suppressors[weaponIndex].suppressorMesh;
        }
        catch{
            suppressor.SetActive(false);
        }

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

            SwitchWeapon(weaponIndex);

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

            SwitchWeapon(weaponIndex);         
        }

    }

    void SwitchWeapon(int weaponIndex){
        bool isSuppressor = suppressors[weaponIndex] != null;
        var bodyOffset = new Vector3(weapons[weaponIndex].xOffset, weapons[weaponIndex].yOffset, 1);
        body.localPosition = bodyOffset;

        if (suppressors[weaponIndex] != null){
            suppressor.transform.localPosition = weapons[weaponIndex].suppressorPos;
            suppressor.transform.localScale = new Vector3(suppressors[weaponIndex].size, 0.1f, suppressors[weaponIndex].size);
        }
    }
    

    void FixedUpdate()
    {
        var shootInput = shootAction.ReadValue<float>();
        var reloadInput = reloadAction.ReadValue<float>();
        var zoomInput = zoomAction.ReadValue<float>();

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

        if (reloadInput>0 && ammo[weaponIndex]<mags[weaponIndex].maxAmmo && !isReloading){
            isReloading = true;
            reload = Reload();
            StartCoroutine(reload);
        }

        if(zoomInput>0){
            Vector3 pos = transform.localPosition;
            pos.x = -0.5f;
            transform.localPosition = pos;
            camFOV.SetCameraFov(startFOV/zoomMultiplier);
        }else{
            Vector3 pos = transform.localPosition;
            pos.x = 0f;
            transform.localPosition = pos;
            camFOV.SetCameraFov(startFOV);
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(mags[weaponIndex].reloadSpeed);
        isReloading = false;
        ammo[weaponIndex] = mags[weaponIndex].maxAmmo;
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

        if(mags[weaponIndex].shootParticleSystem != null){
            Vector3 particleOffset = new Vector3(0f, 0f, 0.1f);
            ParticleSystem muzzleFlash = Instantiate(mags[weaponIndex].shootParticleSystem, weapons[weaponIndex].suppressorPos, transform.rotation, body);
            muzzleFlash.transform.localPosition = weapons[weaponIndex].suppressorPos;
            muzzleFlash.Play();
            StartCoroutine(DestroyAfterSeconds(mags[weaponIndex].psTime, muzzleFlash.gameObject));
        }

        if(mags[weaponIndex].lineRenderer != null){
                Vector3 endPoint = new Vector3(0, 0, weapons[weaponIndex].range);
                LineRenderer lr = Instantiate(mags[weaponIndex].lineRenderer, weapons[weaponIndex].suppressorPos, transform.rotation, body);
                lr.transform.localPosition =  weapons[weaponIndex].suppressorPos;
                lr.SetPosition(1, lr.transform.localPosition + endPoint);
                StartCoroutine(DestroyAfterSeconds(mags[weaponIndex].psTime, lr.gameObject));
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, weapons[weaponIndex].range))
        {

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                if(mags[weaponIndex].damageOverTime){
                    IEnumerator takedot =  target.TakeDamageOverTime(mags[weaponIndex].damage, mags[weaponIndex].hit_num);
                    StartCoroutine(takedot);
                    if(target.currentHealth-mags[weaponIndex].damage<=0){
                        Destroy(target.gameObject);
                    }
                }else{
                    target.TakeDamage(mags[weaponIndex].damage);
                }
            }

            // GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            // Destroy(impactGO, 2f);
        }
    }

    private IEnumerator DestroyAfterSeconds(float seconds, GameObject muzzleFlash){
        yield return new WaitForSeconds(seconds);
        Destroy(muzzleFlash.gameObject);
    }

}
