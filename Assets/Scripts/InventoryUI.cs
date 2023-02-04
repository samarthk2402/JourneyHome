using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] List<GunObject> weapons = new List<GunObject>();
    [SerializeField] List<Scope> scopes = new List<Scope>();
    [SerializeField] List<Magazine> mags = new List<Magazine>();
    [SerializeField] List<Suppressor> suppressors = new List<Suppressor>();

    [SerializeField] GameObject bodyTab;
    [SerializeField] GameObject scopeTab;
    [SerializeField] GameObject suppressorTab;
    [SerializeField] GameObject magTab;

    [SerializeField] GameObject attachmentButton;

    void OnEnable(){
        for(int i=0; i<weapons.Count; i++){
            Instantiate(attachmentButton, new Vector3(0, -(i*20), 0), Quaternion.identity, bodyTab.transform);
        }

        for(int i=0; i<scopes.Count; i++){
            Instantiate(attachmentButton, new Vector3(0, -(i*20), 0), Quaternion.identity, scopeTab.transform);
        }

        for(int i=0; i<mags.Count; i++){
            Instantiate(attachmentButton, new Vector3(0, -(i*20), 0), Quaternion.identity, magTab.transform);
        }

        for(int i=0; i<suppressors.Count; i++){
            Instantiate(attachmentButton, new Vector3(0, -(i*20), 0), Quaternion.identity, suppressorTab.transform);
        }
    }

}
