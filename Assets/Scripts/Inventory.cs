using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryCanvas;

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

    private int weaponIndex;

    void Start(){
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
        }else{
            GetComponentInParent<CursorScript>().isCursorLocked = true;
            GetComponentInParent<Player>().enabled = true;
            GetComponent<Gun>().enabled = true;
            inventoryCanvas.SetActive(false);
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

    public void SetWeapon1(){
        weaponIndex = 0;
    }

    public void SetWeapon2(){
        weaponIndex = 1;
    }

    public void ChangeBody(int index){
        Debug.Log(index);
        playerWeapons[weaponIndex] = weapons[index];
    }

}
