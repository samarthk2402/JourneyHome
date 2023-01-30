using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunObject",menuName = "Scriptable objects/GunObject")]
public class GunObject : ScriptableObject
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float reloadSpeed = 1;
    public float maxAmmo = 5;
    public bool autoFire;
    public Mesh weaponMesh; 
}
