using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magazine",menuName = "Scriptable objects/Magazine")]
public class Magazine : ScriptableObject
{
    public int maxAmmo;
    public float reloadSpeed;
}
