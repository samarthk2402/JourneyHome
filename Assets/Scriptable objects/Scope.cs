using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scope",menuName = "Scriptable objects/Scope")]
public class Scope : ScriptableObject
{
    public Mesh scopeMesh;
    public float zoomMultiplier;
}
