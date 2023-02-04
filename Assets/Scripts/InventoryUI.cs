using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Inventory inventory;
 
    [SerializeField] List<GunObject> weapons = new List<GunObject>();
    [SerializeField] List<Scope> scopes = new List<Scope>();
    [SerializeField] List<Magazine> mags = new List<Magazine>();
    [SerializeField] List<Suppressor> suppressors = new List<Suppressor>();

    private List<GameObject> bodyButtons = new List<GameObject>();
    private List<GameObject> scopeButtons = new List<GameObject>();
    private List<GameObject> suppressorButtons = new List<GameObject>();
    private List<GameObject> magButtons = new List<GameObject>();

    [SerializeField] GameObject bodyTab;
    [SerializeField] GameObject scopeTab;
    [SerializeField] GameObject suppressorTab;
    [SerializeField] GameObject magTab;

    [SerializeField] GameObject attachmentButton;

    void OnEnable(){
        for(int i=0; i<weapons.Count; i++){
            var button = Instantiate(attachmentButton, new Vector3(0, -(i*20), 0), Quaternion.identity, bodyTab.transform);
            button.transform.localPosition = new Vector3(0, -(i*30), 0);
            var name = button.GetComponentInChildren<TMP_Text>();
            name.text = weapons[i].name;
            bodyButtons.Add(button);
        }

        for(int i=0; i<scopes.Count; i++){
            var button = Instantiate(attachmentButton, new Vector3(0, -(i*20), 0), Quaternion.identity, scopeTab.transform);
            button.transform.localPosition = new Vector3(0, -(i*30), 0);
            button.transform.localPosition = new Vector3(0, -(i*30), 0);
            var name = button.GetComponentInChildren<TMP_Text>();
            name.text = scopes[i].name;
        }

        for(int i=0; i<mags.Count; i++){
            var button = Instantiate(attachmentButton, new Vector3(0, -(i*20), 0), Quaternion.identity, magTab.transform);
            button.transform.localPosition = new Vector3(0, -(i*30), 0);
            button.transform.localPosition = new Vector3(0, -(i*30), 0);
            var name = button.GetComponentInChildren<TMP_Text>();
            name.text = mags[i].name;
        }

        for(int i=0; i<suppressors.Count; i++){
            var button = Instantiate(attachmentButton, new Vector3(0, -(i*20), 0), Quaternion.identity, suppressorTab.transform);
            button.transform.localPosition = new Vector3(0, -(i*30), 0);
            button.transform.localPosition = new Vector3(0, -(i*30), 0);
            var name = button.GetComponentInChildren<TMP_Text>();
            name.text = suppressors[i].name;
        }

        for (int i = 0; i<bodyButtons.Count; i++){
                Debug.Log(i);
                bodyButtons[i].GetComponent<Button>().onClick.AddListener(() => inventory.ChangeBody(i-1));
        }
    }

    void OnDisable(){
        bodyButtons = new List<GameObject>();
    }

    public void EnableBodyTab(){
        bodyTab.SetActive(true);
        scopeTab.SetActive(false);
        suppressorTab.SetActive(false);
        magTab.SetActive(false);
    }

    public void EnableScopeTab(){
        bodyTab.SetActive(false);
        scopeTab.SetActive(true);
        suppressorTab.SetActive(false);
        magTab.SetActive(false);
    }

    public void EnableSuppressorTab(){
        bodyTab.SetActive(false);
        scopeTab.SetActive(false);
        suppressorTab.SetActive(true);
        magTab.SetActive(false);
    }

    public void EnableMagTab(){
        bodyTab.SetActive(false);
        scopeTab.SetActive(false);
        suppressorTab.SetActive(false);
        magTab.SetActive(true);
    }

}
