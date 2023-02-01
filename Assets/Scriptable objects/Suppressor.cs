using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Suppressor",menuName = "Scriptable objects/Suppressor")]
public class Suppressor : ScriptableObject
{
    public Mesh suppressorMesh;
    public float damageOffset;
    public float size;
}
