using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIgun : MonoBehaviour
{
    [SerializeField] GameObject gun;
    [SerializeField] GameObject body;
    [SerializeField] GameObject scope;
    [SerializeField] GameObject suppressor;
    public MeshFilter gunMeshFilter;
    public MeshFilter scopeMeshFilter;
    public MeshFilter suppressorMeshFilter;

    [SerializeField] GameObject inventory;

    private Gun gunScript;

    void Awake(){
        gunScript = gun.GetComponent<Gun>();
    }

    void Update(){
        gunMeshFilter.mesh = gunScript.inventory.playerWeapons[gun.GetComponent<Inventory>().weaponIndex].weaponMesh;

        try{
            scope.SetActive(true);
            scopeMeshFilter.mesh = gunScript.inventory.playerScopes[gun.GetComponent<Inventory>().weaponIndex].scopeMesh;
        }
        catch{
            scope.SetActive(false);
        }

        try{
            suppressor.SetActive(true);
            suppressorMeshFilter.mesh = gunScript.inventory.playerSuppressors[gun.GetComponent<Inventory>().weaponIndex].suppressorMesh;
        }
        catch{
            suppressor.SetActive(false);
        }

        SwitchWeapon(gun.GetComponent<Inventory>().weaponIndex);
    }

    private void SwitchWeapon(int weaponIndex){
        bool isSuppressor = gunScript.inventory.playerSuppressors[weaponIndex] != null;
        var bodyOffset = new Vector3(gunScript.inventory.playerWeapons[weaponIndex].xOffset, gunScript.inventory.playerWeapons[weaponIndex].yOffset, 1);
        body.transform.localPosition = bodyOffset;

        if (gunScript.inventory.playerSuppressors[weaponIndex] != null){
            suppressor.transform.localPosition = gunScript.inventory.playerWeapons[weaponIndex].suppressorPos;
            suppressor.transform.localScale = new Vector3(gunScript.inventory.playerSuppressors[weaponIndex].size, 0.1f, gunScript.inventory.playerSuppressors[weaponIndex].size);
        }
    }


}
