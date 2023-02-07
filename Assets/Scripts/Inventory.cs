using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] GameObject HUDCanvas;

    PlayerInput playerInput;
    InputAction inventoryAction;

    private bool isInventory = false;
    private bool canSwitch = true;

    [SerializeField] List<GunObject> weapons = new List<GunObject>();
    [SerializeField] List<Scope> scopes = new List<Scope>();
    [SerializeField] List<Magazine> mags = new List<Magazine>();
    [SerializeField] List<Suppressor> suppressors = new List<Suppressor>();

    public List<GunObject> playerWeapons = new List<GunObject>();
    public List<Scope> playerScopes = new List<Scope>();
    public List<Magazine> playerMags = new List<Magazine>();
    public List<Suppressor> playerSuppressors = new List<Suppressor>();

    public int weaponIndex;

    void Awake(){
        playerInput = GetComponentInParent<PlayerInput>();
        inventoryAction = playerInput.actions["inventory"];
    }

    void Update(){
        var inventoryInput = inventoryAction.ReadValue<float>();

        if(inventoryInput>0 && canSwitch){
            if(isInventory){
                isInventory = false;
            }else{
                isInventory = true;
            }

            StartCoroutine(waitForKeyRelease());
        }

        if(isInventory){
            GetComponentInParent<CursorScript>().isCursorLocked = false;
            GetComponentInParent<Player>().enabled = false;
            GetComponent<Gun>().enabled = false;
            inventoryCanvas.SetActive(true);
            HUDCanvas.SetActive(false);
        }else{
            GetComponentInParent<CursorScript>().isCursorLocked = true;
            GetComponentInParent<Player>().enabled = true;
            GetComponent<Gun>().enabled = true;
            inventoryCanvas.SetActive(false);
            HUDCanvas.SetActive(true);
        }
    }

    private IEnumerator waitForKeyRelease()
    {
        canSwitch = false;
        yield return new WaitUntil(InventoryKeyReleased);
        canSwitch = true;
    }

    bool InventoryKeyReleased(){
        if (inventoryAction.ReadValue<float>() <= 0){
            return true;
        }else{
            return false;
        }
    }

    public void SetWeaponIndex(int index){
        weaponIndex = index;
        // GetComponent<Gun>().SwitchWeapon(weaponIndex);
    }

    public void ChangeBody(int index){
        playerWeapons[weaponIndex] = weapons[index];
        
    }

    public void ChangeScope(int index){
        playerScopes[weaponIndex] = scopes[index];
        
    }

    public void ChangeMag(int index){
        playerMags[weaponIndex] = mags[index];
     
    }

    public void ChangeSuppressor(int index){
        playerSuppressors[weaponIndex] = suppressors[index];
    }

}
