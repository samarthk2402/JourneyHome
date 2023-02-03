using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunObject",menuName = "Scriptable objects/GunObject")]
public class GunObject : ScriptableObject
{
    public float range = 100f;
    public float fireRate = 15f;
    public bool autoFire;
    public Mesh weaponMesh; 
    public float yOffset;
    public float xOffset;
    public Vector3 suppressorPos;
}
