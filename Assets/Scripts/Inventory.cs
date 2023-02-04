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
            GetComponentInParent<Player>().enabled = false;
            GetComponent<Gun>().enabled = false;
            inventoryCanvas.SetActive(true);
        }else{
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

}
